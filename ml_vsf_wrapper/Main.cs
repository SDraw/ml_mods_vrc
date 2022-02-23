namespace ml_vsf_wrapper
{
    public class VsfWrapper : MelonLoader.MelonMod
    {
        bool m_quit = false;

        MemoryMapWritter m_mapWritter = null;
        FaceData m_faceData;
        UnityEngine.GameObject m_trackingRoot = null;

        public override void OnApplicationStart()
        {
            m_mapWritter = new MemoryMapWritter();
            m_mapWritter.Open("ml_vsf/data");

            m_faceData = new FaceData();

            MelonLoader.MelonCoroutines.Start(SearchTrackingRoot());
        }
        System.Collections.IEnumerator SearchTrackingRoot()
        {
            while(m_trackingRoot == null)
            {
                m_trackingRoot = UnityEngine.GameObject.Find("VSeeFace/Tracking/IKTargetWrapper/BasicIKTarget");
                yield return null;
            }
        }

        public override void OnApplicationQuit()
        {
            if(!m_quit)
            {
                m_quit = true;

                m_mapWritter.Close();
                m_mapWritter = null;
            }
        }

        public override void OnUpdate()
        {
            if(m_trackingRoot != null)
            {
                m_faceData.m_headPositionX = m_trackingRoot.transform.position.x;
                m_faceData.m_headPositionY = m_trackingRoot.transform.position.y;
                m_faceData.m_headPositionZ = m_trackingRoot.transform.position.z;
                m_faceData.m_headRotationX = m_trackingRoot.transform.rotation.x;
                m_faceData.m_headRotationY = m_trackingRoot.transform.rotation.y;
                m_faceData.m_headRotationZ = m_trackingRoot.transform.rotation.z;
                m_faceData.m_headRotationW = m_trackingRoot.transform.rotation.w;

                m_mapWritter.Write(FaceData.ToBytes(m_faceData));
            }
        }
    }
}
