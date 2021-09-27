namespace ml_clv
{
    static class Utils
    {
        public static VRCPlayer GetLocalPlayer() => VRCPlayer.field_Internal_Static_VRCPlayer_0;

        public static SteamVR_ControllerManager GetSteamVRControllerManager()
        {
            SteamVR_ControllerManager l_result = null;
            if(VRCInputManager.field_Private_Static_Dictionary_2_EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique_VRCInputProcessor_0?.Count > 0)
            {
                VRCInputProcessor l_input = null;
                l_input = VRCInputManager.field_Private_Static_Dictionary_2_EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique_VRCInputProcessor_0[VRCInputManager.EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique.Vive];
                if(l_input != null)
                {
                    var l_viveInput = l_input.TryCast<VRCInputProcessorVive>();
                    if(l_viveInput != null)
                        l_result = l_viveInput.field_Private_SteamVR_ControllerManager_0;
                }
            }
            return l_result;
        }

        public static VRCTrackingManager GetVRCTrackingManager() => VRCTrackingManager.field_Private_Static_VRCTrackingManager_0;
    }
}
