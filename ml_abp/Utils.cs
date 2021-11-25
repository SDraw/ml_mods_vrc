using System.Collections.Generic;

namespace ml_abp
{
    static class Utils
    {
        public static float QuadraticEaseOut(float f_value)
        {
            return (1f - UnityEngine.Mathf.Pow(1f - f_value, 2f));
        }

        // VRChat related
        static VRC.UI.Elements.QuickMenu ms_quickMenu = null;
        public static VRC.Player GetPlayerQM() // Thanks, now I hate this new menu
        {
            VRC.Player l_result = null;
            if(ms_quickMenu == null)
                ms_quickMenu = UnityEngine.GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)").GetComponent<VRC.UI.Elements.QuickMenu>();
            if((ms_quickMenu != null) && (ms_quickMenu.field_Private_UIPage_1 != null) && ms_quickMenu.field_Private_UIPage_1.isActiveAndEnabled)
            {
                var l_selectedUserQM = ms_quickMenu.field_Private_UIPage_1.TryCast<VRC.UI.Elements.Menus.SelectedUserMenuQM>();
                if((l_selectedUserQM != null) && (l_selectedUserQM.field_Private_IUser_0 != null))
                {
                    l_result = GetPlayerWithId(l_selectedUserQM.field_Private_IUser_0.prop_String_0);
                }
            }
            return l_result;
        }
        public static VRC.Player GetLocalPlayer() => VRC.Player.prop_Player_0;

        public static bool IsFriend(VRC.Player f_player) => VRC.Core.APIUser.IsFriendsWith(f_player.prop_String_0);
        public static Il2CppSystem.Collections.Generic.List<VRC.Player> GetPlayers() => VRC.PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0;

        public static VRC.Player GetPlayerWithId(string f_id)
        {
            return (VRC.Player)MethodsResolver.GetPlayerById?.Invoke(null, new object[] { f_id });
        }

        public static List<VRC.Player> GetFriendsInInstance()
        {
            List<VRC.Player> l_result = new List<VRC.Player>();
            var l_remotePlayers = GetPlayers();
            if(l_remotePlayers != null)
            {
                foreach(var l_remotePlayer in l_remotePlayers)
                {
                    if((l_remotePlayer != null) && IsFriend(l_remotePlayer))
                        l_result.Add(l_remotePlayer);
                }
            }
            return l_result;
        }

        // Extensions
        public static void SetAvatarFloatParamEx(this AvatarPlayableController controller, int paramHash, float val, bool debug = false)
        {
            MethodsResolver.SetAvatarFloatParam?.Invoke(controller, new object[] { paramHash, val, debug });
            controller.field_Private_Boolean_3 = true; // bool requiresNetworkSync;
        }
    }
}
