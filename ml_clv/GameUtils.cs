using System;
using System.Linq;
using System.Reflection;
using UnhollowerRuntimeLib.XrefScans;
using VRC.Core;

namespace ml_clv
{
    // Replacement of VRChatUtilityKit
    static class GameUtils
    {
        public static Action OnRoomJoined;
        public static Action OnRoomLeft;
        public static Action OnUiManagerInit;

        public static void Initialize(HarmonyLib.Harmony f_instance)
        {
            f_instance.Patch(typeof(NetworkManager).GetMethod(nameof(NetworkManager.OnJoinedRoom)), null, new HarmonyLib.HarmonyMethod(typeof(GameUtils).GetMethod(nameof(Patch_OnRoomJoined), BindingFlags.NonPublic | BindingFlags.Static)));
            f_instance.Patch(typeof(NetworkManager).GetMethod(nameof(NetworkManager.OnLeftRoom)), null, new HarmonyLib.HarmonyMethod(typeof(GameUtils).GetMethod(nameof(Patch_OnRoomLeft), BindingFlags.NonPublic | BindingFlags.Static)));

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
    }
}
