using System.Runtime.InteropServices;
using UnityEngine;

namespace ml_kte
{
    public class KinectTrackingExtension : MelonLoader.MelonMod
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

        // Kinect V1 and V2 have same bones IDs, almost
        static readonly int[] ms_positionBones = { 3, 0, 14, 18, 7, 11 };
        static readonly int[] ms_rotationBones = { 2, 0, 14, 18, 7, 11 };

        static readonly Quaternion[] ms_globalRotations =
        {
            Quaternion.Euler(0f,180f,0f),
            Quaternion.Euler(0f,180f,0f),
            Quaternion.identity,
            Quaternion.identity,
            Quaternion.identity,
            Quaternion.identity
        };
        static readonly Quaternion[] ms_localRotations =
        {
            Quaternion.identity,
            Quaternion.identity,
            Quaternion.Euler(270f,270f,0f),
            Quaternion.Euler(270f,90f,0f),
            Quaternion.Euler(270f,90f,0f),
            Quaternion.Euler(270f,270f,0f),
        };
        static readonly Vector4[] ms_rotationNegations =
        {
            Vector4.one,
            Vector4.one,
            new Vector4(-1f,1f,-1f,1f),
            new Vector4(-1f,1f,-1f,1f),
            new Vector4(-1f,1f,-1f,1f),
            new Vector4(-1f,1f,-1f,1f)
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

            VRChatUtilityKit.Utilities.VRCUtils.OnUiManagerInit += this.OnUiManagerInit;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomJoined += this.OnRoomJoined;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomLeft += this.OnRoomLeft;

            OnPreferencesSaved();
        }

        public override void OnApplicationQuit()
        {
            if(!m_quit) // This is not a joke
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
        }

        public override void OnPreferencesSaved()
        {
            if(!m_quit)
            {
                bool l_oldState = Settings.Enabled;
                Settings.Reload();

                if(Settings.IsAnyEntryUpdated())
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

                        for(int i = 0; i < (int)TrackedPoint.Count; i++)
                        {
                            m_trackedPoints[i].GetComponent<MeshRenderer>().enabled = (Settings.Enabled && Settings.ShowPoints);
                        }
                    }

                    if(m_localTracked != null)
                    {
                        m_localTracked.TrackHead = Settings.TrackHead;
                        m_localTracked.TrackHips = Settings.TrackHips;
                        m_localTracked.TrackLegs = Settings.TrackLegs;
                        m_localTracked.TrackHands = Settings.TrackHands;
                        m_localTracked.RotateHead = Settings.RotateHead;
                        m_localTracked.RotateHips = Settings.RotateHips;
                        m_localTracked.RotateLegs = Settings.RotateLegs;
                        m_localTracked.RotateHands = Settings.RotateHands;
                    }
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
                    int l_boneIndex = ms_positionBones[i];
                    m_trackedPoints[i].transform.localPosition = new Vector3(-m_positionFloats[l_boneIndex * 3], m_positionFloats[l_boneIndex * 3 + 1], m_positionFloats[l_boneIndex * 3 + 2]);

                    l_boneIndex = ms_rotationBones[i];
                    m_trackedPoints[i].transform.localRotation = ms_globalRotations[i] * (new Quaternion(-m_rotationFloats[l_boneIndex * 4] * ms_rotationNegations[i].x, -m_rotationFloats[l_boneIndex * 4 + 1] * ms_rotationNegations[i].y, m_rotationFloats[l_boneIndex * 4 + 2] * ms_rotationNegations[i].z, m_rotationFloats[l_boneIndex * 4 + 3] * ms_rotationNegations[i].w) * ms_localRotations[i]);
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
                m_trackedPoints[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                m_trackedPoints[i].name = "TrackedPoint" + i;
                m_trackedPoints[i].transform.parent = m_trackedRoot.transform;
                m_trackedPoints[i].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                m_trackedPoints[i].layer = LayerMask.NameToLayer("Player");
                m_trackedPoints[i].GetComponent<MeshRenderer>().enabled = (Settings.Enabled && Settings.ShowPoints);
                Object.DontDestroyOnLoad(m_trackedPoints[i]);
                Object.Destroy(m_trackedPoints[i].GetComponent<SphereCollider>());
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
            m_localTracked.TrackHead = Settings.TrackHead;
            m_localTracked.TrackHips = Settings.TrackHips;
            m_localTracked.TrackLegs = Settings.TrackLegs;
            m_localTracked.TrackHands = Settings.TrackHands;
            m_localTracked.RotateHead = Settings.RotateHead;
            m_localTracked.RotateHips = Settings.RotateHips;
            m_localTracked.RotateLegs = Settings.RotateLegs;
            m_localTracked.RotateHands = Settings.RotateHands;
        }

        void OnRoomLeft()
        {
            m_localTracked = null;
        }
    }
}
