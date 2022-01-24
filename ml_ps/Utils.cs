using System.Linq;

namespace ml_ps
{
    static class Utils
    {
        public static VRCTrackingManager GetVRCTrackingManager() => VRCTrackingManager.field_Private_Static_VRCTrackingManager_0;
        public static VRCTrackingSteam GetVRCTrackingSteam() => GetVRCTrackingManager().field_Private_List_1_VRCTracking_0[0].TryCast<VRCTrackingSteam>();
        public static UnityEngine.Camera GetMainCamera() => GetVRCTrackingSteam().field_Private_SteamVR_Camera_0.field_Private_Camera_0;
        public static string GetCurrentWorldName() => RoomManager.field_Internal_Static_ApiWorld_0.name;
        public static UnityEngine.GameObject GetStreamCamera() => VRC.UserCamera.UserCameraController.field_Internal_Static_UserCameraController_0.field_Private_Camera_0.gameObject;
        public static UnityEngine.GameObject GetPhotoCamera() => VRC.UserCamera.UserCameraController.field_Internal_Static_UserCameraController_0.field_Private_Camera_1.gameObject;
        public static UnityEngine.GameObject GetUserCamera() => VRC.UserCamera.UserCameraController.field_Internal_Static_UserCameraController_0.gameObject;

        public static string CleanupAsFilename(string p_string)
        {
            char[] l_invalidChars = System.IO.Path.GetInvalidFileNameChars();
            return new string(p_string.Where(x => !l_invalidChars.Contains(x)).ToArray());
        }
    }
}
