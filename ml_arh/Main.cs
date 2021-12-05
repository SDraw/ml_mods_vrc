namespace ml_ahr
{
    public class Main : MelonLoader.MelonMod
    {
        HeightAdjuster m_localAdjuster = null;

        public override void OnApplicationStart()
        {
            Settings.Load();

            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomJoined += this.OnRoomJoined;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomLeft += this.OnRoomLeft;
        }

        public override void OnPreferencesSaved()
        {
            Settings.Reload();

            if(m_localAdjuster != null)
            {
                m_localAdjuster.Enabled = Settings.Enabled;
                m_localAdjuster.PoseHeight = (Settings.Enabled && Settings.PoseHeight);

                if(Settings.Enabled)
                {
                    float l_height = Utils.GetTrackingHeight();
                    m_localAdjuster.UpdateHeights(l_height, l_height * 0.515151f);
                }
                else
                {
                    m_localAdjuster.UpdateHeights(1.65f, 0.85f); // Default VRChat values
                }
            }
        }

        void OnRoomJoined()
        {
            MelonLoader.MelonCoroutines.Start(CreateLocalAdjuster());
        }
        System.Collections.IEnumerator CreateLocalAdjuster()
        {
            while(Utils.GetLocalPlayer() == null)
                yield return null;
            m_localAdjuster = Utils.GetLocalPlayer().gameObject.AddComponent<HeightAdjuster>();
            m_localAdjuster.Enabled = Settings.Enabled;
            m_localAdjuster.PoseHeight = (Settings.Enabled && Settings.PoseHeight);
        }

        void OnRoomLeft()
        {
            m_localAdjuster = null;
        }
    }
}
