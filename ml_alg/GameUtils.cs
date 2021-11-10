using System;
using System.Linq;
using System.Reflection;
using UnhollowerRuntimeLib.XrefScans;
using VRC.Core;

namespace ml_alg
{
    // Replacement of VRChatUtilityKit
    static class GameUtils
    {
        public static Action OnRoomJoined;
        public static Action OnRoomLeft;
        public static Action<UnityEngine.GameObject> OnAvatarInstantiated;
        public static Action<VRC.Player> OnPlayerJoined;
        public static Action<VRC.Player> OnPlayerLeft;
        public static Action<APIUser> OnFriended;
        public static Action<string> OnUnfriended;

        public static void Initialize(HarmonyLib.Harmony f_instance)
        {
            f_instance.Patch(typeof(NetworkManager).GetMethod(nameof(NetworkManager.OnJoinedRoom)), null, new HarmonyLib.HarmonyMethod(typeof(GameUtils).GetMethod(nameof(Patch_OnRoomJoined), BindingFlags.NonPublic | BindingFlags.Static)));
            f_instance.Patch(typeof(NetworkManager).GetMethod(nameof(NetworkManager.OnLeftRoom)), null, new HarmonyLib.HarmonyMethod(typeof(GameUtils).GetMethod(nameof(Patch_OnRoomLeft), BindingFlags.NonPublic | BindingFlags.Static)));
            f_instance.Patch(typeof(VRCPlayer).GetMethod(nameof(VRCPlayer.Awake)), null, new HarmonyLib.HarmonyMethod(typeof(GameUtils).GetMethod(nameof(Patch_VRCPlayerAwake), BindingFlags.NonPublic | BindingFlags.Static)));
            f_instance.Patch(typeof(APIUser).GetMethod(nameof(APIUser.LocalAddFriend)), null, new HarmonyLib.HarmonyMethod(typeof(GameUtils).GetMethod(nameof(Patch_LocalAddFriend), BindingFlags.NonPublic | BindingFlags.Static)));
            f_instance.Patch(typeof(APIUser).GetMethod(nameof(APIUser.UnfriendUser)), null, new HarmonyLib.HarmonyMethod(typeof(GameUtils).GetMethod(nameof(Patch_UnfriendUser), BindingFlags.NonPublic | BindingFlags.Static)));

            MelonLoader.MelonCoroutines.Start(GetJoinLeftEvents());
        }

        static System.Collections.IEnumerator GetJoinLeftEvents()
        {
            while(VRCUiManager.prop_VRCUiManager_0 == null)
                yield return null;
            NetworkManager.field_Internal_Static_NetworkManager_0.field_Internal_VRCEventDelegate_1_Player_0.field_Private_HashSet_1_UnityAction_1_T_0.Add(new System.Action<VRC.Player>((p) =>
            {
                if(p != null)
                {
                    OnPlayerJoined?.Invoke(p);
                }
            })
            );
            NetworkManager.field_Internal_Static_NetworkManager_0.field_Internal_VRCEventDelegate_1_Player_1.field_Private_HashSet_1_UnityAction_1_T_0.Add(new System.Action<VRC.Player>((p) =>
            {
                if(p != null)
                {
                    OnPlayerLeft?.Invoke(p);
                }
            })
            );
        }

        public static bool IsSafeWorld()
        {
            bool l_result = true;
            foreach(string l_tag in RoomManager.field_Internal_Static_ApiWorld_0.tags)
            {
                if(l_tag.ToLower().Contains("game") || l_tag.ToLower().Contains("club"))
                {
                    l_result = false;
                    break;
                }
            }
            return l_result;
        }

        static void Patch_OnRoomJoined()
        {
            OnRoomJoined?.Invoke();
        }

        static void Patch_OnRoomLeft()
        {
            OnRoomLeft?.Invoke();
        }

        static void Patch_VRCPlayerAwake(VRCPlayer __instance)
        {
            if(__instance != null)
            {
                __instance.field_Private_OnAvatarIsReady_0 += new Action(() =>
                {
                    if((__instance.field_Private_VRCAvatarManager_0 != null) && (__instance.field_Private_ApiAvatar_0 != null) && (__instance.field_Internal_GameObject_0 != null))
                    {
                        OnAvatarInstantiated?.Invoke(__instance.field_Internal_GameObject_0);
                    }
                });
            }
        }

        static void Patch_LocalAddFriend(APIUser __0)
        {
            if(__0 != null)
            {
                OnFriended?.Invoke(__0);
            }
        }

        static void Patch_UnfriendUser(string __0)
        {
            if((__0 != null) && __0.Length != 0)
            {
                OnUnfriended?.Invoke(__0);
            }
        }
    }
}
