using UnityEngine;

namespace ml_lme
{
    public class LeapMotionExtention : MelonLoader.MelonMod
    {
        static readonly Quaternion ms_hmdRotationFix = new Quaternion(0f, 0.7071068f, 0.7071068f, 0f);

        static LeapMotionExtention ms_instance = null;

        bool m_quit = false;

        Leap.Controller m_leapController = null;
        static GestureMatcher.GesturesData ms_gesturesData = new GestureMatcher.GesturesData();

        LeapTracked m_localTracked = null;

        public override void OnApplicationStart()
        {
            if(ms_instance == null)
                ms_instance = this;

            DependenciesHandler.ExtractDependencies();
            Settings.LoadSettings();

            m_leapController = new Leap.Controller();
            m_leapController.Device += this.OnLeapDeviceInitialized;

            // Events
            VRChatUtilityKit.Utilities.VRCUtils.OnUiManagerInit += this.OnUiManagerInit;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomJoined += this.OnRoomJoined;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomLeft += this.OnRoomLeft;
        }

        void OnUiManagerInit()
        {
            if(Utils.IsInVRMode())
            {
                // Patches
                HarmonyInstance.Patch(
                    typeof(HandGestureController).GetMethod(nameof(HandGestureController.Update)),
                    new HarmonyLib.HarmonyMethod(typeof(LeapMotionExtention), nameof(HGC_Update_Prefix)),
                    new HarmonyLib.HarmonyMethod(typeof(LeapMotionExtention), nameof(HGC_Update_Postfix))
                );
            }

            ApplySettings();
        }

        public override void OnApplicationQuit()
        {
            if(!m_quit) // This is not a joke
            {
                m_quit = true;

                ms_instance = null;

                m_localTracked = null;

                m_leapController.StopConnection();
                m_leapController.Dispose();
                m_leapController = null;
            }
        }

        public override void OnPreferencesSaved()
        {
            if(!m_quit) // This is not a joke
            {
                Settings.ReloadSettings();

                if(Settings.IsAnyEntryUpdated())
                    ApplySettings();
            }
        }

        public override void OnUpdate()
        {
            if(Settings.Enabled)
            {
                for(int i = 0; i < GestureMatcher.GesturesData.ms_handsCount; i++)
                    ms_gesturesData.m_handsPresenses[i] = false;

                if((m_leapController != null) && m_leapController.IsConnected)
                {
                    Leap.Frame l_frame = m_leapController.Frame();
                    if(l_frame != null)
                        GestureMatcher.GetGestures(l_frame, ref ms_gesturesData);
                }

                if(m_localTracked != null)
                    m_localTracked.UpdateFromGestures(ms_gesturesData);
            }
        }

        public override void OnLateUpdate()
        {
            if(Settings.Enabled && (m_localTracked != null))
                m_localTracked.UpdateFromGestures(ms_gesturesData);
        }

        void ApplySettings()
        {
            if(m_leapController != null)
            {
                if(Settings.Enabled)
                {
                    m_leapController.StartConnection();
                    m_leapController.ClearPolicy(Leap.Controller.PolicyFlag.POLICY_OPTIMIZE_SCREENTOP);
                    if(Settings.LeapHmdMode)
                        m_leapController.SetPolicy(Leap.Controller.PolicyFlag.POLICY_OPTIMIZE_HMD);
                    else
                        m_leapController.ClearPolicy(Leap.Controller.PolicyFlag.POLICY_OPTIMIZE_HMD);
                }
                else
                    m_leapController.StopConnection();
            }

            if(m_localTracked != null)
            {
                m_localTracked.SetEnabled(Settings.Enabled);
                m_localTracked.ReapplyTracking();
            }
        }

        void OnRoomJoined()
        {
            MelonLoader.MelonCoroutines.Start(CreateLocalTracked());
        }
        System.Collections.IEnumerator CreateLocalTracked()
        {
            while(Utils.GetLocalPlayer() == null)
                yield return null;

            m_localTracked = Utils.GetLocalPlayer().gameObject.AddComponent<LeapTracked>();
            m_localTracked.SetEnabled(Settings.Enabled);
        }

        void OnRoomLeft()
        {
            m_localTracked = null;
        }

        void OnLeapDeviceInitialized(object p_sender, Leap.DeviceEventArgs p_args)
        {
            if(!m_quit && (m_leapController != null))
            {
                m_leapController.ClearPolicy(Leap.Controller.PolicyFlag.POLICY_OPTIMIZE_SCREENTOP);
                if(Settings.LeapHmdMode)
                    m_leapController.SetPolicy(Leap.Controller.PolicyFlag.POLICY_OPTIMIZE_HMD);
                else
                    m_leapController.ClearPolicy(Leap.Controller.PolicyFlag.POLICY_OPTIMIZE_HMD);
            }
        }

        static void HGC_Update_Prefix(ref HandGestureController __instance) => ms_instance?.OnHandGestureControllerUpdatePrefix(__instance);
        void OnHandGestureControllerUpdatePrefix(HandGestureController p_controller)
        {
            if(Settings.Enabled && !Utils.AreHandsTracked() && Utils.IsNonVRInput(Utils.GetCurrentInput()))
            {
                if(m_localTracked != null)
                    m_localTracked.ForceDesktopTracking(p_controller);
            }
        }

        static void HGC_Update_Postfix(ref HandGestureController __instance) => ms_instance?.OnHandGestureControllerUpdatePostfix(__instance);
        void OnHandGestureControllerUpdatePostfix(HandGestureController p_controller)
        {
            if(Settings.Enabled && (Utils.GetCurrentInput() != VRCInputManager.InputMethod.Index))
            {
                if(m_localTracked != null)
                    m_localTracked.ForceIndexTracking(p_controller);
            }
        }
    }
}
