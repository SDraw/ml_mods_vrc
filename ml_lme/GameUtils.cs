using System;
using System.Linq;
using System.Reflection;
using UnhollowerRuntimeLib.XrefScans;
using VRC.Core;

namespace ml_lme
{
    // Replacement of VRChatUtilityKit
    static class GameUtils
    {
        public static Action OnRoomJoined;
        public static Action OnRoomLeft;
        public static Action<UnityEngine.GameObject> OnAvatarInstantiated;
        public static Action OnUiManagerInit;

        public static void Initialize(HarmonyLib.Harmony f_instance)
        {
            f_instance.Patch(typeof(NetworkManager).GetMethod(nameof(NetworkManager.OnJoinedRoom)), null, new HarmonyLib.HarmonyMethod(typeof(GameUtils).GetMethod(nameof(Patch_OnRoomJoined), BindingFlags.NonPublic | BindingFlags.Static)));
            f_instance.Patch(typeof(NetworkManager).GetMethod(nameof(NetworkManager.OnLeftRoom)), null, new HarmonyLib.HarmonyMethod(typeof(GameUtils).GetMethod(nameof(Patch_OnRoomLeft), BindingFlags.NonPublic | BindingFlags.Static)));
            f_instance.Patch(typeof(VRCPlayer).GetMethod(nameof(VRCPlayer.Awake)), null, new HarmonyLib.HarmonyMethod(typeof(GameUtils).GetMethod(nameof(Patch_VRCPlayerAwake), BindingFlags.NonPublic | BindingFlags.Static)));

            MelonLoader.MelonCoroutines.Start(AwaitUiManager());
        }

        static System.Collections.IEnumerator AwaitUiManager()
        {
            while(VRCUiManager.prop_VRCUiManager_0 == null)
                yield return null;
            OnUiManagerInit?.Invoke();
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
    }
}
