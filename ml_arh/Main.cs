namespace ml_arh
{
    public class AvatarRealHeight : MelonLoader.MelonMod
    {
        HeightAdjuster m_localAdjuster = null;

        public override void OnApplicationStart()
        {
            Settings.Load();

            VRChatUtilityKit.Utilities.NetworkEvents.OnPlayerJoined += this.OnPlayerJoined;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomLeft += this.OnRoomLeft;
        }

        public override void OnPreferencesSaved()
        {
            Settings.Reload();

            if(Settings.IsAnyEntryUpdated() && (m_localAdjuster != null))
            {
                m_localAdjuster.SetEnabled(Settings.Enabled && VRChatUtilityKit.Utilities.VRCUtils.AreRiskyFunctionsAllowed);
                m_localAdjuster.SetPoseHeight(Settings.PoseHeight);
            }
        }

        void OnPlayerJoined(VRC.Player p_player)
        {
            if(p_player == Utils.GetLocalPlayer()._player)
                OnRoomJoined();
        }

        void OnRoomJoined()
        {
            m_localAdjuster = Utils.GetLocalPlayer().gameObject.AddComponent<HeightAdjuster>();
            m_localAdjuster.SetEnabled(Settings.Enabled && VRChatUtilityKit.Utilities.VRCUtils.AreRiskyFunctionsAllowed);
            m_localAdjuster.SetPoseHeight(Settings.PoseHeight);
        }

        void OnRoomLeft()
        {
            m_localAdjuster = null;
        }
    }
}
