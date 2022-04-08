namespace ml_lat
{
    public class LegsAnimationTweaker : MelonLoader.MelonMod
    {
        LegsTweak m_localTweak = null;

        public override void OnApplicationStart()
        {
            Settings.Load();

            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomJoined += this.OnRoomJoin;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomLeft += this.OnRoomLeft;
        }

        public override void OnPreferencesSaved()
        {
            Settings.Reload();

            if(Settings.IsAnyEntryUpdated() && (m_localTweak != null))
            {
                m_localTweak.SetLegsAnimation(Settings.LegsAnimation);
                m_localTweak.SetLegsAutostep(Settings.LegsAutostep);
            }
        }

        void OnRoomJoin()
        {
            MelonLoader.MelonCoroutines.Start(CreateLocalTweaker());
        }
        System.Collections.IEnumerator CreateLocalTweaker()
        {
            while(Utils.GetLocalPlayer() == null)
                yield return null;

            m_localTweak = Utils.GetLocalPlayer().gameObject.AddComponent<LegsTweak>();
            m_localTweak.SetLegsAnimation(Settings.LegsAnimation);
            m_localTweak.SetLegsAutostep(Settings.LegsAutostep);
        }

        void OnRoomLeft()
        {
            m_localTweak = null;
        }
    }
}
