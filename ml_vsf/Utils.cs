namespace ml_vsf
{
    static class Utils
    {
        public static VRCPlayer GetLocalPlayer() => VRCPlayer.field_Internal_Static_VRCPlayer_0;
        public static VRCTrackingManager GetVRCTrackingManager() => VRCTrackingManager.field_Private_Static_VRCTrackingManager_0;
        public static VRCTrackingSteam GetVRCTrackingSteam() => GetVRCTrackingManager().field_Private_List_1_VRCTracking_0[0].TryCast<VRCTrackingSteam>();
        public static SteamVR_ControllerManager GetSteamVRControllerManager() => GetVRCTrackingSteam().field_Private_SteamVR_ControllerManager_0;

        public static UnityEngine.Vector3 GetAvatarViewPoint() => VRCTrackingManager.field_Private_Static_Vector3_0;
        public static UnityEngine.Vector3 GetAvatarHeadToViewPoint() => VRCTrackingManager.field_Private_Static_Vector3_1;
    }
}
