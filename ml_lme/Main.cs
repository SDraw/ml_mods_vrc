using UnityEngine;

namespace ml_lme
{
    public class LeapMotionExtention : MelonLoader.MelonMod
    {
        static readonly Quaternion ms_hmdRotationFix = new Quaternion(0f, 0.7071068f, 0.7071068f, 0f);
        static readonly Quaternion ms_screentopRotationFix = new Quaternion(0f, 0f, -1f, 0f);

        static LeapMotionExtention ms_instance = null;

        bool m_quit = false;

        Leap.Controller m_leapController = null;
        GestureMatcher.GesturesData m_gesturesData = new GestureMatcher.GesturesData();

        GameObject m_leapTrackingRoot = null;
        GameObject[] m_leapHands = null;
        GameObject m_leapControllerModel = null;

        LeapTracked m_localTracked = null;

        public override void OnApplicationStart()
        {
            if(ms_instance == null)
                ms_instance = this;

            DependenciesHandler.ExtractDependencies();
            Settings.LoadSettings();

            m_leapController = new Leap.Controller();
            m_leapController.Device += this.OnLeapDeviceInitialized;

            m_leapHands = new GameObject[GestureMatcher.GesturesData.ms_handsCount];

            // Events
            VRChatUtilityKit.Utilities.VRCUtils.OnUiManagerInit += this.OnUiManagerInit;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomJoined += this.OnRoomJoined;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomLeft += this.OnRoomLeft;

            // Patches
            HarmonyInstance.Patch(
                typeof(RootMotion.FinalIK.IKSolverVR).GetMethod(nameof(RootMotion.FinalIK.IKSolverVR.VrcLateSolve)),
                new HarmonyLib.HarmonyMethod(typeof(LeapMotionExtention), nameof(IKSolverVR_VrcLateSolve_Prefix))
            );
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

            MelonLoader.MelonCoroutines.Start(CreateLeapObjects());
        }
        System.Collections.IEnumerator CreateLeapObjects()
        {
            while(Utils.GetVRCTrackingManager() == null)
                yield return null;
            while(Utils.GetVRCTrackingSteam() == null)
                yield return null;
            while(Utils.GetSteamVRControllerManager() == null)
                yield return null;

            AssetsHandler.Load();

            m_leapTrackingRoot = new GameObject("LeapTrackingRoot");
            m_leapTrackingRoot.transform.parent = Utils.GetSteamVRControllerManager().transform;
            m_leapTrackingRoot.transform.localPosition = Vector3.zero;
            m_leapTrackingRoot.transform.localRotation = Quaternion.identity;
            Object.DontDestroyOnLoad(m_leapTrackingRoot);

            for(int i = 0; i < GestureMatcher.GesturesData.ms_handsCount; i++)
            {
                m_leapHands[i] = new GameObject("LeapHand" + i);
                m_leapHands[i].transform.parent = m_leapTrackingRoot.transform;
                m_leapHands[i].transform.localPosition = Vector3.zero;
                m_leapHands[i].transform.localRotation = Quaternion.identity;
                Object.DontDestroyOnLoad(m_leapHands[i]);
            }

            m_leapControllerModel = AssetsHandler.GetAsset("assets/models/leapmotion/leap_motion_1_0.obj");
            if(m_leapControllerModel != null)
            {
                m_leapControllerModel.name = "LeapModel";
                m_leapControllerModel.transform.parent = m_leapTrackingRoot.transform;
                m_leapControllerModel.transform.localPosition = Vector3.zero;
                m_leapControllerModel.transform.localRotation = Quaternion.identity;
            }

            ApplySettings();
        }

        public override void OnApplicationQuit()
        {
            if(!m_quit) // This is not a joke
            {
                m_quit = true;

                ms_instance = null;

                m_leapControllerModel = null;
                m_leapTrackingRoot = null;
                m_localTracked = null;

                m_leapController.StopConnection();
                m_leapController.Dispose();
                m_leapController = null;

                AssetsHandler.Unload();
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
                    m_gesturesData.m_handsPresenses[i] = false;

                if((m_leapController != null) && m_leapController.IsConnected)
                {
                    Leap.Frame l_frame = m_leapController.Frame();
                    if(l_frame != null)
                    {
                        GestureMatcher.GetGestures(l_frame, ref m_gesturesData);

                        for(int i = 0; i < GestureMatcher.GesturesData.ms_handsCount; i++)
                        {
                            if(m_gesturesData.m_handsPresenses[i] && (m_leapHands[i] != null))
                            {
                                Vector3 l_pos = m_gesturesData.m_handsPositons[i];
                                Quaternion l_rot = m_gesturesData.m_handsRotations[i];
                                ReorientateLeapToUnity(ref l_pos, ref l_rot, Settings.TrackingMode);
                                m_leapHands[i].transform.localPosition = l_pos;
                                m_leapHands[i].transform.localRotation = l_rot;
                            }
                        }
                    }
                }

                if(m_localTracked != null)
                    m_localTracked.UpdateFingers(m_gesturesData);
            }
        }

        public override void OnLateUpdate()
        {
            if(Settings.Enabled && (m_localTracked != null))
                m_localTracked.UpdateFingers(m_gesturesData);
        }

        void ApplySettings()
        {
            if(m_leapController != null)
            {
                if(Settings.Enabled)
                {
                    m_leapController.StartConnection();
                    UpdateDeviceTrackingMode();
                }
                else
                    m_leapController.StopConnection();
            }

            // Update tracking transforms
            if(m_leapTrackingRoot != null)
            {
                m_leapTrackingRoot.transform.parent = (Settings.HeadRoot ? Utils.GetCamera().transform : Utils.GetSteamVRControllerManager().transform);
                m_leapTrackingRoot.transform.localPosition = new Vector3(0f, (Settings.HeadRoot ? Settings.HeadOffsetY : Settings.DesktopOffsetY), (Settings.HeadRoot ? Settings.HeadOffsetZ : Settings.DesktopOffsetZ));
                m_leapTrackingRoot.transform.localRotation = Quaternion.Euler(Settings.RootRotation, 0f, 0f);
            }

            if(m_leapControllerModel != null)
            {
                switch(Settings.TrackingMode)
                {
                    case Settings.LeapTrackingMode.Screentop:
                        m_leapControllerModel.transform.localRotation = Quaternion.Euler(0, 0f, 180f);
                        break;
                    case Settings.LeapTrackingMode.Desktop:
                        m_leapControllerModel.transform.localRotation = Quaternion.identity;
                        break;
                    case Settings.LeapTrackingMode.Hmd:
                        m_leapControllerModel.transform.localRotation = Quaternion.Euler(270f, 180f, 0f);
                        break;
                }
                m_leapControllerModel.active = Settings.ShowModel;
            }

            if(m_localTracked != null)
            {
                m_localTracked.Enabled = Settings.Enabled;
                m_localTracked.FingersOnly = Settings.FingersTracking;
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
            m_localTracked.Enabled = Settings.Enabled;
            m_localTracked.FingersOnly = Settings.FingersTracking;
        }

        void OnRoomLeft()
        {
            m_localTracked = null;
        }

        void OnLeapDeviceInitialized(object p_sender, Leap.DeviceEventArgs p_args)
        {
            if(!m_quit && (m_leapController != null))
                UpdateDeviceTrackingMode();
        }

        void UpdateDeviceTrackingMode()
        {
            m_leapController.ClearPolicy(Leap.Controller.PolicyFlag.POLICY_OPTIMIZE_SCREENTOP);
            m_leapController.ClearPolicy(Leap.Controller.PolicyFlag.POLICY_OPTIMIZE_HMD);

            switch(Settings.TrackingMode)
            {
                case Settings.LeapTrackingMode.Screentop:
                    m_leapController.SetPolicy(Leap.Controller.PolicyFlag.POLICY_OPTIMIZE_SCREENTOP);
                    break;
                case Settings.LeapTrackingMode.Hmd:
                    m_leapController.SetPolicy(Leap.Controller.PolicyFlag.POLICY_OPTIMIZE_HMD);
                    break;
            }
        }

        static void ReorientateLeapToUnity(ref Vector3 p_pos, ref Quaternion p_rot, Settings.LeapTrackingMode p_mode)
        {
            p_pos *= 0.001f;
            p_pos.z *= -1f;
            p_rot.x *= -1f;
            p_rot.y *= -1f;

            switch(p_mode)
            {
                case Settings.LeapTrackingMode.Screentop:
                {
                    p_pos.x *= -1f;
                    p_pos.y *= -1f;
                    p_rot = (ms_screentopRotationFix * p_rot);
                }
                break;

                case Settings.LeapTrackingMode.Hmd:
                {
                    p_pos.x *= -1f;
                    Utils.Swap(ref p_pos.y, ref p_pos.z);
                    p_rot = (ms_hmdRotationFix * p_rot);
                }
                break;
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

        static void IKSolverVR_VrcLateSolve_Prefix(ref RootMotion.FinalIK.IKSolverVR __instance) => ms_instance?.OnVrcLateIKSolve(__instance);
        void OnVrcLateIKSolve(RootMotion.FinalIK.IKSolverVR p_solver)
        {
            if(Settings.Enabled && (m_localTracked != null))
                m_localTracked.LateUpdateIK(p_solver, m_gesturesData, m_leapHands[0].transform, m_leapHands[1].transform);
        }
    }
}
