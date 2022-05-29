namespace ml_clv
{
    static class Utils
    {
        public static VRCPlayer GetLocalPlayer() => VRCPlayer.field_Internal_Static_VRCPlayer_0;

        public static VRCTrackingManager GetVRCTrackingManager() => VRCTrackingManager.field_Private_Static_VRCTrackingManager_0;
        public static VRCTrackingSteam GetVRCTrackingSteam() => GetVRCTrackingManager().field_Private_List_1_VRCTracking_0[0].TryCast<VRCTrackingSteam>();
        public static SteamVR_ControllerManager GetSteamVRControllerManager() => GetVRCTrackingSteam().field_Private_SteamVR_ControllerManager_0;

        public static float GetAvatarScale(VRCPlayer p_player)
        {
            float l_result = 1f;
            if(p_player.field_Private_VRC_AnimationController_0.field_Private_VRCVrIkController_0 != null)
                l_result = p_player.field_Private_VRC_AnimationController_0.field_Private_VRCVrIkController_0.field_Private_Single_4;
            return l_result;
        }
    }
}
