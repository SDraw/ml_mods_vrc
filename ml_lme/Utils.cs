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

        public static SteamVR_ControllerManager GetSteamVRControllerManager()
        {
            SteamVR_ControllerManager l_result = null;
            if(VRCInputManager.field_Private_Static_Dictionary_2_InputMethod_VRCInputProcessor_0?.Count > 0)
            {
                VRCInputProcessor l_input = null;
                l_input = VRCInputManager.field_Private_Static_Dictionary_2_InputMethod_VRCInputProcessor_0[VRCInputManager.InputMethod.Vive];
                if(l_input != null)
                {
                    VRCInputProcessorVive l_viveInput = l_input.TryCast<VRCInputProcessorVive>();
                    if(l_viveInput != null)
                        l_result = l_viveInput.field_Private_SteamVR_ControllerManager_0;
                }
            }
            return l_result;
        }

        public static bool AreHandsTracked()
        {
            return (VRCTrackingManager.Method_Public_Static_Boolean_ID_0(VRCTracking.ID.HandTracker_LeftWrist) || VRCTrackingManager.Method_Public_Static_Boolean_ID_0(VRCTracking.ID.HandTracker_RightWrist));
        }

        public static bool IsInVRMode()
        {
            return (bool)MethodsResolver.IsInVRMode?.Invoke(null, null);
        }

        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        // Extensions
        public static void SetAvatarIntParamEx(this AvatarPlayableController controller, int paramHash, int val)
        {
            MethodsResolver.SetAvatarIntParam?.Invoke(controller, new object[] { paramHash, val });
            controller.field_Private_Boolean_3 = true; // bool requiresNetworkSync;
        }
        public static void SetAvatarBoolParamEx(this AvatarPlayableController controller, int paramHash, bool val)
        {
            MethodsResolver.SetAvatarBoolParam?.Invoke(controller, new object[] { paramHash, val });
            controller.field_Private_Boolean_3 = true; // bool requiresNetworkSync;
        }
        public static void SetAvatarFloatParamEx(this AvatarPlayableController controller, int paramHash, float val, bool debug = false)
        {
            MethodsResolver.SetAvatarFloatParam?.Invoke(controller, new object[] { paramHash, val, debug });
            controller.field_Private_Boolean_3 = true; // bool requiresNetworkSync;
        }
    }
}
