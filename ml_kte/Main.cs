using System.Runtime.InteropServices;
using UnityEngine;

namespace ml_kte
{
    public class Main : MelonLoader.MelonMod
    {
        enum TrackedPoint
        {
            Head = 0,
            Hips,
            LeftLeg,
            RightLeg,
            LeftHand,
            RightHand,

            Count
        }

        static readonly int[] g_trackedPointBones = { 3, 0, 14, 18, 7, 11 }; // Kinect V1 and V2 have same bones IDs, almost

        static readonly Quaternion[] g_rotationFixes =
        {
            Quaternion.identity,
            Quaternion.Euler(0f,180f,0f),
            Quaternion.identity,
            Quaternion.identity,
            Quaternion.identity,
            Quaternion.identity
        };

        KinectTracked m_localTracked = null;
        GameObject m_trackedRoot = null;
        GameObject[] m_trackedPoints = null;

        float[] m_positionFloats = null;
        GCHandle m_positionFloatsAlloc;
        float[] m_rotationFloats = null;
        GCHandle m_rotationFloatsAlloc;

        bool m_quit = false;

        public override void OnApplicationStart()
        {
            DependenciesHandler.ExtractDependencies();
            Settings.Load();

            m_trackedPoints = new GameObject[(int)TrackedPoint.Count];

            m_positionFloats = new float[75];
            m_positionFloatsAlloc = GCHandle.Alloc(m_positionFloats, GCHandleType.Pinned);
            m_rotationFloats = new float[100];
            m_rotationFloatsAlloc = GCHandle.Alloc(m_rotationFloats, GCHandleType.Pinned);

            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomJoined += this.OnRoomJoined;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomLeft += this.OnRoomLeft;
            VRChatUtilityKit.Utilities.NetworkEvents.OnAvatarInstantiated += this.OnAvatarInstantiated;
            VRChatUtilityKit.Utilities.VRCUtils.OnUiManagerInit += this.OnUiManagerInit;

            OnPreferencesSaved();
        }

        public override void OnApplicationQuit()
        {
            m_quit = true;

            if(Settings.Enabled)
            {
                switch(Settings.DeviceVersion)
                {
                    case Settings.KinectVersion.V1:
                        KinectHandlerV1.Terminate();
                        break;
                    case Settings.KinectVersion.V2:
                        KinectHandlerV2.Terminate();
                        break;
                }
            }

            m_positionFloatsAlloc.Free();
            m_rotationFloatsAlloc.Free();

            m_trackedPoints = null;
            m_trackedRoot = null;
        }

        public override void OnPreferencesSaved()
        {
            if(!m_quit)
            {
                switch(Settings.DeviceVersion)
                {
                    case Settings.KinectVersion.V1:
                        KinectHandlerV1.Check();
                        break;
                    case Settings.KinectVersion.V2:
                        KinectHandlerV2.Check();
                        break;
                }

                bool l_oldState = Settings.Enabled;
                Settings.Reload();

                if(Settings.Enabled)
                {
                    switch(Settings.DeviceVersion)
                    {
                        case Settings.KinectVersion.V1:
                            KinectHandlerV1.Launch();
                            break;
                        case Settings.KinectVersion.V2:
                            KinectHandlerV2.Launch();
                            break;
                    }
                }
                else
                {
                    switch(Settings.DeviceVersion)
                    {
                        case Settings.KinectVersion.V1:
                            KinectHandlerV1.Terminate();
                            break;
                        case Settings.KinectVersion.V2:
                            KinectHandlerV2.Terminate();
                            break;
                    }

                    if(l_oldState && (Utils.GetLocalPlayer() != null))
                        VRChatUtilityKit.Utilities.VRCUtils.ReloadAvatar(Utils.GetLocalPlayer());
                }

                if(m_trackedRoot != null)
                {
                    m_trackedRoot.transform.localPosition = new Vector3(Settings.OffsetX, Settings.OffsetY, Settings.OffsetZ);
                    m_trackedRoot.transform.localRotation = Quaternion.Euler(Settings.OffsetRX, Settings.OffsetRY, Settings.OffsetRZ);
                }
            }
        }

        public override void OnUpdate()
        {
            if(Settings.Enabled && (m_trackedRoot != null))
            {
                switch(Settings.DeviceVersion)
                {
                    case Settings.KinectVersion.V1:
                        KinectHandlerV1.GetTrackingData(m_positionFloatsAlloc.AddrOfPinnedObject(), m_rotationFloatsAlloc.AddrOfPinnedObject());
                        break;
                    case Settings.KinectVersion.V2:
                        KinectHandlerV2.GetTrackingData(m_positionFloatsAlloc.AddrOfPinnedObject(), m_rotationFloatsAlloc.AddrOfPinnedObject());
                        break;
                }
                for(int i = 0; i < (int)TrackedPoint.Count; i++)
                {
                    int l_boneIndex = g_trackedPointBones[i];
                    m_trackedPoints[i].transform.localPosition = new Vector3(-m_positionFloats[l_boneIndex * 3], m_positionFloats[l_boneIndex * 3 + 1], m_positionFloats[l_boneIndex * 3 + 2]);
                    m_trackedPoints[i].transform.localRotation = g_rotationFixes[i] * new Quaternion(-m_rotationFloats[l_boneIndex * 4], -m_rotationFloats[l_boneIndex * 4 + 1], m_rotationFloats[l_boneIndex * 4 + 2], m_rotationFloats[l_boneIndex * 4 + 3]);
                }
            }
        }

        public override void OnLateUpdate()
        {
            if(Settings.Enabled && (m_localTracked != null) && (m_trackedRoot != null))
            {
                m_localTracked.LateUpdateTransforms(
                    m_trackedPoints[(int)TrackedPoint.Head].transform, m_trackedPoints[(int)TrackedPoint.Hips].transform,
                    m_trackedPoints[(int)TrackedPoint.LeftLeg].transform, m_trackedPoints[(int)TrackedPoint.RightLeg].transform,
                     m_trackedPoints[(int)TrackedPoint.LeftHand].transform, m_trackedPoints[(int)TrackedPoint.RightHand].transform
                );
            }
        }

        void OnUiManagerInit()
        {
            MelonLoader.MelonCoroutines.Start(CreateTrackedPoints());
        }
        System.Collections.IEnumerator CreateTrackedPoints()
        {
            while(Utils.GetVRCTrackingSteam() == null)
                yield return null;

            m_trackedRoot = new GameObject("KinectRoot");
            m_trackedRoot.transform.parent = Utils.GetVRCTrackingSteam().transform;
            m_trackedRoot.transform.localPosition = new Vector3(Settings.OffsetX, Settings.OffsetY, Settings.OffsetZ);
            m_trackedRoot.transform.localRotation = Quaternion.Euler(Settings.OffsetRX, Settings.OffsetRY, Settings.OffsetRZ);
            Object.DontDestroyOnLoad(m_trackedRoot);

            for(int i = 0; i < (int)TrackedPoint.Count; i++)
            {
                m_trackedPoints[i] = new GameObject("TrackedPoint" + i);
                m_trackedPoints[i].transform.parent = m_trackedRoot.transform;
                Object.DontDestroyOnLoad(m_trackedPoints[i]);
            }
        }

        void OnRoomJoined()
        {
            MelonLoader.MelonCoroutines.Start(CreateLocalLifted());
        }
        System.Collections.IEnumerator CreateLocalLifted()
        {
            while(Utils.GetLocalPlayer() == null)
                yield return null;

            m_localTracked = Utils.GetLocalPlayer().gameObject.AddComponent<KinectTracked>();
        }

        void OnRoomLeft()
        {
            m_localTracked = null;
        }

        void OnAvatarInstantiated(VRCAvatarManager f_avatarManager, VRC.Core.ApiAvatar f_apiAvatar, GameObject f_avatarObject)
        {
            var l_player = f_avatarObject.transform.root.GetComponent<VRCPlayer>();
            if((l_player != null) && (l_player == Utils.GetLocalPlayer()) && (m_localTracked != null))
                m_localTracked.RecacheComponents();
        }
    }
}
