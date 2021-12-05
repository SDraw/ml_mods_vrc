using System.Linq;
using UnityEngine;

namespace ml_lme
{
    public class LeapMotionExtention : MelonLoader.MelonMod
    {
        static readonly Quaternion ms_hmdRotationFix = new Quaternion(0f, 0.7071068f, 0.7071068f, 0f);

        bool m_quit = false;

        Leap.Controller m_leapController = null;
        GestureMatcher.GesturesData m_gesturesData = null;

        GameObject m_leapTrackingRoot = null;
        GameObject[] m_leapHands = null;

        LeapTracked m_localTracked = null;

        public override void OnApplicationStart()
        {
            DependenciesHandler.ExtractDependencies();
            MethodsResolver.ResolveMethods();
            Settings.LoadSettings();

            m_leapController = new Leap.Controller();
            m_leapController.Device += this.OnLeapDeviceInitialized;

            m_gesturesData = new GestureMatcher.GesturesData();
            m_leapHands = new GameObject[GestureMatcher.GesturesData.ms_handsCount];

            // Events
            VRChatUtilityKit.Utilities.VRCUtils.OnUiManagerInit += this.OnUiManagerInit;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomJoined += this.OnRoomJoined;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomLeft += this.OnRoomLeft;

            // Patches
            var l_patchMethod = new HarmonyLib.HarmonyMethod(typeof(LeapMotionExtention), nameof(VRCIM_ControllersType));
            typeof(VRCInputManager).GetMethods().Where(x =>
                    x.Name.StartsWith("Method_Public_Static_Boolean_InputMethod_")
                ).ToList().ForEach(m => HarmonyInstance.Patch(m, l_patchMethod));
        }

        void OnUiManagerInit()
        {
            MelonLoader.MelonCoroutines.Start(CreateLeapObjects());
        }
        System.Collections.IEnumerator CreateLeapObjects()
        {
            while(Utils.GetSteamVRControllerManager() == null)
                yield return null;

            m_leapTrackingRoot = new GameObject("LeapTrackingRoot");
            m_leapTrackingRoot.transform.parent = Utils.GetSteamVRControllerManager().transform;
            Object.DontDestroyOnLoad(m_leapTrackingRoot);

            for(int i = 0; i < GestureMatcher.GesturesData.ms_handsCount; i++)
            {
                m_leapHands[i] = new GameObject("LeapHand" + i);
                m_leapHands[i].transform.parent = m_leapTrackingRoot.transform;
                Object.DontDestroyOnLoad(m_leapHands[i]);
            }

            OnPreferencesSaved();
        }

        public override void OnApplicationQuit()
        {
            m_quit = true;

            m_leapTrackingRoot = null;
            m_localTracked = null;

            m_leapController.StopConnection();
            m_leapController.Dispose();
            m_leapController = null;
        }

        public override void OnPreferencesSaved()
        {
            if(!m_quit) // This is not a joke
            {
                Settings.ReloadSettings();

                // Update Leap controller
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
                    m_leapTrackingRoot.transform.localPosition = new Vector3(0f, (Settings.HeadRoot ? Settings.HmdOffsetY : Settings.DesktopOffsetY), (Settings.HeadRoot ? Settings.HmdOffsetZ : Settings.DesktopOffsetZ));
                    m_leapTrackingRoot.transform.localRotation = Quaternion.identity;
                }

                if(m_localTracked != null)
                {
                    m_localTracked.FingersOnly = Settings.FingersTracking;
                    m_localTracked.Sdk3Parameters = Settings.SDK3Parameters;
                    if(!Settings.Enabled)
                        m_localTracked.ResetTracking();
                }
            }
        }

        public override void OnUpdate()
        {
            if(Settings.Enabled && m_leapController.IsConnected)
            {
                var l_frame = m_leapController.Frame();
                if(l_frame != null)
                {
                    GestureMatcher.GetGestures(l_frame, ref m_gesturesData);
                    if(m_localTracked != null)
                        m_localTracked.UpdateFromGestures(m_gesturesData);

                    // Update transforms
                    for(int i = 0; i < GestureMatcher.GesturesData.ms_handsCount; i++)
                    {
                        if(m_gesturesData.m_handsPresenses[i] && (m_leapHands[i] != null))
                        {
                            Vector3 l_pos = m_gesturesData.m_handsPositons[i];
                            Quaternion l_rot = m_gesturesData.m_handsRotations[i];
                            ReorientateLeapToUnity(ref l_pos, ref l_rot, Settings.LeapHmdMode);
                            m_leapHands[i].transform.localPosition = l_pos;
                            m_leapHands[i].transform.localRotation = l_rot;
                        }
                    }
                }
            }
        }

        public override void OnLateUpdate()
        {
            if(Settings.Enabled && (m_localTracked != null))
            {
                m_localTracked.UpdateHandsPositions(m_gesturesData, m_leapHands[0].transform, m_leapHands[1].transform);
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
            m_localTracked.FingersOnly = Settings.FingersTracking;
            m_localTracked.Sdk3Parameters = Settings.SDK3Parameters;
        }

        void OnRoomLeft()
        {
            m_localTracked = null;
        }

        void OnLeapDeviceInitialized(object f_sender, Leap.DeviceEventArgs f_args)
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

        static void ReorientateLeapToUnity(ref Vector3 f_pos, ref Quaternion f_rot, bool f_hmd)
        {
            f_pos *= 0.001f;
            f_pos.z *= -1f;
            f_rot.x *= -1f;
            f_rot.y *= -1f;

            if(f_hmd)
            {
                f_pos.x *= -1f;
                Utils.Swap(ref f_pos.y, ref f_pos.z);
                f_rot = (ms_hmdRotationFix * f_rot);
            }
        }

        static bool VRCIM_ControllersType(ref bool __result, VRCInputManager.InputMethod __0)
        {
            // Need to account presence of VR controllers to allow mouse input for controler-less VR
            if(Settings.Enabled && (bool)MethodsResolver.IsInVR?.Invoke(null, null))
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
    }
}
