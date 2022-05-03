namespace ml_lme
{
    static class Utils
    {
        public static VRCPlayer GetLocalPlayer() => VRCPlayer.field_Internal_Static_VRCPlayer_0;
        public static VRCTrackingManager GetVRCTrackingManager() => VRCTrackingManager.field_Private_Static_VRCTrackingManager_0;
        public static VRCTrackingSteam GetVRCTrackingSteam() => GetVRCTrackingManager().field_Private_List_1_VRCTracking_0[0].TryCast<VRCTrackingSteam>();
        public static SteamVR_Camera GetCamera() => GetVRCTrackingSteam().field_Private_SteamVR_Camera_0;
        public static UnityEngine.Transform GetTrackingLeftController() => GetVRCTrackingSteam().field_Private_SteamVR_ControllerManager_0.field_Public_GameObject_0.transform;
        public static UnityEngine.Transform GetTrackingRightController() => GetVRCTrackingSteam().field_Private_SteamVR_ControllerManager_0.field_Public_GameObject_1.transform;
        public static SteamVR_ControllerManager GetSteamVRControllerManager() => GetVRCTrackingSteam().field_Private_SteamVR_ControllerManager_0;

        public static bool AreHandsTracked() => (UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.LeftHand).isValid || UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.RightHand).isValid);
        public static bool IsInVRMode() => UnityEngine.XR.XRDevice.isPresent;

        public static bool GetGesturesToggle() => (UnityEngine.PlayerPrefs.GetInt(HandGestureController.field_Private_Static_String_0) == 1);
        public static VRCInputManager.InputMethod GetCurrentInput() => VRCInputManager.field_Private_Static_InputMethod_0;

        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
    }
}
