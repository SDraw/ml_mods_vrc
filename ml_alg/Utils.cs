using UnityEngine;

namespace ml_alg
{
    static class Utils
    {
        public static readonly Vector4 gs_pointVec4 = new Vector4(0f, 0f, 0f, 1f);

        // VRChat related
        static VRC.UI.Elements.QuickMenu ms_quickMenu = null;
        public static VRC.Player GetPlayerQM() // Thanks, now I hate this new menu
        {
            VRC.Player l_result = null;
            if(ms_quickMenu == null)
                ms_quickMenu = UnityEngine.GameObject.FindObjectOfType<VRC.UI.Elements.QuickMenu>();
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

        public static System.Collections.Generic.List<VRC.Player> GetFriendsInInstance()
        {
            System.Collections.Generic.List<VRC.Player> l_result = new System.Collections.Generic.List<VRC.Player>();
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

        // RootMotion.FinalIK.IKSolverVR extensions
        public static void SetLegIKWeight(this RootMotion.FinalIK.IKSolverVR f_solver, HumanBodyBones f_leg, float f_weight, bool f_rotate)
        {
            var l_leg = (f_leg == HumanBodyBones.LeftFoot) ? f_solver.leftLeg : f_solver.rightLeg;
            if(l_leg != null)
            {
                l_leg.positionWeight = f_weight;
                if(f_rotate)
                    l_leg.rotationWeight = f_weight;
            }
        }

        // UnityEngine.MonoBehaviour extensions
        public static object ConvertToRuntimeType(this MonoBehaviour f_component, System.Type f_type)
        {
            return System.Convert.ChangeType(UnhollowerBaseLib.Runtime.ClassInjectorBase.GetMonoObjectFromIl2CppPointer(f_component.Pointer), f_type);
        }

        // Math extensions
        public static Matrix4x4 GetMatrix(this Transform f_transform, bool f_pos = true, bool f_rot = true, bool f_scl = false)
        {
            return Matrix4x4.TRS(f_pos ? f_transform.position : Vector3.zero, f_rot ? f_transform.rotation : Quaternion.identity, f_scl ? f_transform.localScale : Vector3.one);
        }
        public static Matrix4x4 AsMatrix(this Quaternion f_quat)
        {
            return Matrix4x4.Rotate(f_quat);
        }

        // Just here, no reason, not used
        public static T AsReal<T>(this T obj) where T : UnityEngine.Object
        {
            return (obj != null) ? obj : (T)null;
        }
    }
}
