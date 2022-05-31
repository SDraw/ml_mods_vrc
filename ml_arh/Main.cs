﻿namespace ml_arh
{
    public class AvatarRealHeight : MelonLoader.MelonMod
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

            if(Settings.IsAnyEntryUpdated() && (m_localAdjuster != null))
            {
                m_localAdjuster.SetEnabled(Settings.Enabled && VRChatUtilityKit.Utilities.VRCUtils.AreRiskyFunctionsAllowed);
                m_localAdjuster.SetPoseHeight(Settings.PoseHeight);
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
            m_localAdjuster.SetEnabled(Settings.Enabled && VRChatUtilityKit.Utilities.VRCUtils.AreRiskyFunctionsAllowed);
            m_localAdjuster.SetPoseHeight(Settings.PoseHeight);
        }

        void OnRoomLeft()
        {
            m_localAdjuster = null;
        }
    }
}
