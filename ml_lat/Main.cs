namespace ml_lat
{
    public class LegsAnimationTweaker : MelonLoader.MelonMod
    {
        static LegsAnimationTweaker ms_instance = null;

        LegsTweak m_localTweak = null;

        public override void OnApplicationStart()
        {
            if(ms_instance == null)
                ms_instance = this;

            MethodsResolver.Resolve();
            Settings.Load();

            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomJoined += this.OnRoomJoin;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomLeft += this.OnRoomLeft;

            // Patches
            if(MethodsResolver.CheckPoseChange != null)
                HarmonyInstance.Patch(MethodsResolver.CheckPoseChange, null, new HarmonyLib.HarmonyMethod(typeof(LegsAnimationTweaker).GetMethod(nameof(OnCheckPoseChange_Postfix), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)));
        }

        public override void OnPreferencesSaved()
        {
            Settings.Reload();

            if(Settings.IsAnyEntryUpdated() && (m_localTweak != null))
            {
                m_localTweak.SetLegsAnimation(Settings.LegsAnimation);
                m_localTweak.SetLegsAutostep(Settings.LegsAutostep);
                m_localTweak.SetForwardKnees(Settings.LegsForwardKnees);
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
            m_localTweak.SetForwardKnees(Settings.LegsForwardKnees);
        }

        void OnRoomLeft()
        {
            m_localTweak = null;
        }

        static void OnCheckPoseChange_Postfix(ref VRCVrIkController __instance)
        {
            if(__instance != null)
                ms_instance?.OnCheckPoseChange(__instance);
        }
        void OnCheckPoseChange(VRCVrIkController p_vrIkController)
        {
            if(m_localTweak != null)
                m_localTweak.CheckPoseChange(p_vrIkController);
        }
    }
}
