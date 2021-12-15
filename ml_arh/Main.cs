namespace ml_arh
{
    public class Main : MelonLoader.MelonMod
    {
        HeightAdjuster m_localAdjuster = null;

        public override void OnApplicationStart()
        {
            MethodsResolver.Resolve();
            Settings.Load();

            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomJoined += this.OnRoomJoined;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomLeft += this.OnRoomLeft;
        }

        public override void OnPreferencesSaved()
        {
            Settings.Reload();

            if(m_localAdjuster != null)
            {
                m_localAdjuster.Enabled = (Settings.Enabled && VRChatUtilityKit.Utilities.VRCUtils.AreRiskyFunctionsAllowed);
                m_localAdjuster.PoseHeight = (Settings.Enabled && Settings.PoseHeight && VRChatUtilityKit.Utilities.VRCUtils.AreRiskyFunctionsAllowed);

                m_localAdjuster.UpdateHeight((Settings.Enabled && VRChatUtilityKit.Utilities.VRCUtils.AreRiskyFunctionsAllowed) ? Utils.GetTrackingHeight() : 1.65f);
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
            m_localAdjuster.Enabled = (Settings.Enabled && VRChatUtilityKit.Utilities.VRCUtils.AreRiskyFunctionsAllowed);
            m_localAdjuster.PoseHeight = (Settings.Enabled && Settings.PoseHeight && VRChatUtilityKit.Utilities.VRCUtils.AreRiskyFunctionsAllowed);
        }

        void OnRoomLeft()
        {
            m_localAdjuster = null;
        }
    }
}
