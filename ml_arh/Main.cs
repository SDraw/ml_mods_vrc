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
                    m_localAdjuster.UpdateHeight(Utils.GetTrackingHeight());
                else
                    m_localAdjuster.UpdateHeight(1.65f); // Default VRChat value
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
