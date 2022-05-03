using System.Linq;
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

        GameObject m_leapTrackingRoot = null;
        GameObject[] m_leapHands = null;
        GameObject m_leapControllerModel = null;

        LeapTracked m_localTracked = null;

        public override void OnApplicationStart()
        {
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
            HarmonyLib.HarmonyMethod l_patchMethod = new HarmonyLib.HarmonyMethod(typeof(LeapMotionExtention), nameof(VRCIM_ControllersType));
            typeof(VRCInputManager).GetMethods().Where(m =>
                m.Name.StartsWith("Method_Public_Static_Boolean_InputMethod_") && (m.ReturnType == typeof(bool)) && (m.GetParameters().Count() == 1)
            ).ToList().ForEach(m => HarmonyInstance.Patch(m, l_patchMethod));

            HarmonyInstance.Patch(
                typeof(VRCInputProcessorIndex).GetMethod(nameof(VRCInputProcessorIndex.Method_Public_Static_Void_Boolean_ArrayOf_FingerGestureState_0)),
                null,
                new HarmonyLib.HarmonyMethod(typeof(LeapMotionExtention), nameof(VRCIPI_GetGestureStates))
            );

            HarmonyInstance.Patch(
                typeof(HandGestureController).GetMethod(nameof(HandGestureController.Update)),
                null,
                new HarmonyLib.HarmonyMethod(typeof(LeapMotionExtention), nameof(HGC_Update))
            );
        }

        void OnUiManagerInit()
        {
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
                    ms_gesturesData.m_handsPresenses[i] = false;

                if((m_leapController != null) && m_leapController.IsConnected)
                {
                    Leap.Frame l_frame = m_leapController.Frame();
                    if(l_frame != null)
                    {
                        GestureMatcher.GetGestures(l_frame, ref ms_gesturesData);

                        for(int i = 0; i < GestureMatcher.GesturesData.ms_handsCount; i++)
                        {
                            if(ms_gesturesData.m_handsPresenses[i] && (m_leapHands[i] != null))
                            {
                                Vector3 l_pos = ms_gesturesData.m_handsPositons[i];
                                Quaternion l_rot = ms_gesturesData.m_handsRotations[i];
                                ReorientateLeapToUnity(ref l_pos, ref l_rot, Settings.LeapHmdMode);
                                m_leapHands[i].transform.localPosition = l_pos;
                                m_leapHands[i].transform.localRotation = l_rot;
                            }
                        }
                    }
                }

                if(m_localTracked != null)
                    m_localTracked.UpdateFromGestures(ms_gesturesData);
            }
        }

        public override void OnLateUpdate()
        {
            if(Settings.Enabled && (m_localTracked != null))
                m_localTracked.UpdateTracking(ms_gesturesData, m_leapHands[0].transform, m_leapHands[1].transform);
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

            // Update tracking transforms
            if(m_leapTrackingRoot != null)
            {
                m_leapTrackingRoot.transform.parent = (Settings.HeadRoot ? Utils.GetCamera().transform : Utils.GetSteamVRControllerManager().transform);
                m_leapTrackingRoot.transform.localPosition = new Vector3(0f, (Settings.HeadRoot ? Settings.HeadOffsetY : Settings.DesktopOffsetY), (Settings.HeadRoot ? Settings.HeadOffsetZ : Settings.DesktopOffsetZ));
                m_leapTrackingRoot.transform.localRotation = Quaternion.Euler(Settings.RootRotation, 0f, 0f);
            }

            if(m_leapControllerModel != null)
            {
                m_leapControllerModel.transform.localRotation = (Settings.LeapHmdMode ? Quaternion.Euler(270f, 180f, 0f) : Quaternion.identity);
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
            {
                m_leapController.ClearPolicy(Leap.Controller.PolicyFlag.POLICY_OPTIMIZE_SCREENTOP);
                if(Settings.LeapHmdMode)
                    m_leapController.SetPolicy(Leap.Controller.PolicyFlag.POLICY_OPTIMIZE_HMD);
                else
                    m_leapController.ClearPolicy(Leap.Controller.PolicyFlag.POLICY_OPTIMIZE_HMD);
            }
        }

        static void ReorientateLeapToUnity(ref Vector3 p_pos, ref Quaternion p_rot, bool p_hmd)
        {
            p_pos *= 0.001f;
            p_pos.z *= -1f;
            p_rot.x *= -1f;
            p_rot.y *= -1f;

            if(p_hmd)
            {
                p_pos.x *= -1f;
                Utils.Swap(ref p_pos.y, ref p_pos.z);
                p_rot = (ms_hmdRotationFix * p_rot);
            }
        }

        static bool VRCIM_ControllersType(ref bool __result, VRCInputManager.InputMethod __0)
        {
            if(Settings.Enabled && Settings.LeapGestures && Utils.IsInVRMode() && Utils.AreHandsTracked() && (Utils.GetCurrentInput() != VRCInputManager.InputMethod.Index))
            {
                if(__0 == VRCInputManager.InputMethod.Index)
                {
                    __result = true;
                    return false;
                }
                else
                {
                    __result = false;
                    return false;
                }
            }
            else
                return true;
        }

        static void VRCIPI_GetGestureStates(bool __0, UnhollowerBaseLib.Il2CppStructArray<VRCInputProcessorIndex.FingerGestureState> __1)
        {
            if(Settings.Enabled && Settings.LeapGestures && Utils.IsInVRMode() && (Utils.GetCurrentInput() != VRCInputManager.InputMethod.Index))
            {
                if(ms_gesturesData.m_handsPresenses[__0 ? 1 : 0])
                {
                    for(int i = 0; i < 5; i++)
                        __1[i] = ((1f - (__0 ? ms_gesturesData.m_rightFingersBends[i] : ms_gesturesData.m_leftFingersBends[i]) > 0.25f) ? VRCInputProcessorIndex.FingerGestureState.StretchedOut : VRCInputProcessorIndex.FingerGestureState.CurledIn);
                }
            }
        }

        static void HGC_Update(ref HandGestureController __instance) => ms_instance?.OnHandGestureControllerUpdate(__instance);
        void OnHandGestureControllerUpdate(HandGestureController p_controller)
        {
            if(Settings.Enabled && !Settings.LeapGestures && Utils.IsInVRMode() && (Utils.GetCurrentInput() != VRCInputManager.InputMethod.Index))
            {
                if(m_localTracked != null)
                    m_localTracked.ForceIndexTracking(p_controller);
            }
        }
    }
}
