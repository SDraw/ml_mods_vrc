namespace ml_vsf
{
    public class VsfExtension : MelonLoader.MelonMod
    {
        bool m_quit = false;

        MemoryMapReader m_mapReader = null;
        byte[] m_buffer = null;
        VsfTracked m_localTracked = null;
        UnityEngine.GameObject m_headTracker = null;
        float m_heightOffset = 0f;

        public override void OnApplicationStart()
        {
            Settings.Load();

            m_mapReader = new MemoryMapReader();
            m_mapReader.Open("ml_vsf/data");

            m_buffer = new byte[1024];

            VRChatUtilityKit.Utilities.VRCUtils.OnUiManagerInit += this.OnUiManagerInit;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomJoined += this.OnRoomJoined;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomLeft += this.OnRoomLeft;
        }

        public override void OnApplicationQuit()
        {
            if(!m_quit)
            {
                m_quit = true;

                m_buffer = null;

                m_mapReader?.Close();
                m_mapReader = null;
            }
        }

        public override void OnPreferencesSaved()
        {
            if(!m_quit)
            {
                Settings.Reload();
                if(Settings.IsEnableEntryUpdated() && !Settings.Enabled && (m_localTracked != null))
                    VRChatUtilityKit.Utilities.VRCUtils.ReloadAvatar(Utils.GetLocalPlayer());
            }
        }

        public override void OnUpdate()
        {
            if(Settings.Enabled && m_mapReader.Read(ref m_buffer) && (m_localTracked != null))
            {
                FaceData l_faceData = FaceData.ToObject(m_buffer);

                m_headTracker.transform.localPosition = UnityEngine.Vector3.Lerp(new UnityEngine.Vector3(Settings.Mirroring ? -l_faceData.m_headPositionX : l_faceData.m_headPositionX, l_faceData.m_headPositionY + m_heightOffset, l_faceData.m_headPositionZ), m_headTracker.transform.localPosition, Settings.Blending);
                m_headTracker.transform.localRotation = UnityEngine.Quaternion.Slerp(new UnityEngine.Quaternion(l_faceData.m_headRotationX, Settings.Mirroring ? -l_faceData.m_headRotationY : l_faceData.m_headRotationY, Settings.Mirroring ? -l_faceData.m_headRotationZ : l_faceData.m_headRotationZ, l_faceData.m_headRotationW), m_headTracker.transform.localRotation, Settings.Blending);
            }
        }

        public override void OnLateUpdate()
        {
            if(Settings.Enabled && (m_localTracked != null))
            {
                m_localTracked.UpdateHeadTransform(m_headTracker.transform);
            }
        }

        void OnUiManagerInit()
        {
            UIExpansionKit.API.ExpansionKitApi.GetExpandedMenu(UIExpansionKit.API.ExpandedMenu.QuickMenu).AddSimpleButton("VSF height reset", this.OnHeightReset);

            MelonLoader.MelonCoroutines.Start(CreateHeadTracker());
        }
        System.Collections.IEnumerator CreateHeadTracker()
        {
            while(Utils.GetVRCTrackingManager() == null)
                yield return null;
            while(Utils.GetVRCTrackingSteam() == null)
                yield return null;
            while(Utils.GetSteamVRControllerManager() == null)
                yield return null;

            m_headTracker = new UnityEngine.GameObject("VSF_HeadTracker");
            m_headTracker.transform.parent = Utils.GetSteamVRControllerManager().transform;
            m_headTracker.transform.localPosition = UnityEngine.Vector3.zero;
            m_headTracker.transform.localRotation = UnityEngine.Quaternion.identity;
        }

        void OnRoomJoined()
        {
            MelonLoader.MelonCoroutines.Start(CreateLocalTracked());
        }
        System.Collections.IEnumerator CreateLocalTracked()
        {
            while(Utils.GetLocalPlayer() == null)
                yield return null;

            m_localTracked = Utils.GetLocalPlayer().gameObject.AddComponent<VsfTracked>();
        }

        void OnRoomLeft()
        {
            m_localTracked = null;
        }

        void OnHeightReset()
        {
            if(Settings.Enabled && (m_localTracked != null))
            {
                float l_bindHeight = m_localTracked.transform.position.y + m_localTracked.HeadHeight;
                UnityEngine.Vector3 l_prevLocal = m_headTracker.transform.localPosition;
                UnityEngine.Vector3 l_prevGloval = m_headTracker.transform.position;
                m_headTracker.transform.position = new UnityEngine.Vector3(l_prevGloval.x, l_bindHeight, l_prevGloval.z);
                m_heightOffset = m_headTracker.transform.localPosition.y - l_prevLocal.y;
                m_headTracker.transform.localPosition = new UnityEngine.Vector3(l_prevLocal.x, l_prevLocal.y + m_heightOffset, l_prevLocal.z);
            }
        }
    }
}
