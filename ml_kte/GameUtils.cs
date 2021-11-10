using System;
using System.Linq;
using System.Reflection;
using UnhollowerRuntimeLib.XrefScans;

namespace ml_kte
{
    // Replacement of VRChatUtilityKit
    static class GameUtils
    {
        public static Action OnRoomJoined;
        public static Action OnRoomLeft;
        public static Action<UnityEngine.GameObject> OnAvatarInstantiated;
        public static Action OnUiManagerInit;

        static MethodInfo ms_reloadAvatarMethod = null;

        public static void Initialize(HarmonyLib.Harmony f_instance)
        {
            try
            {
                ms_reloadAvatarMethod = typeof(VRCPlayer).GetMethods().
                    Where(m => m.Name.StartsWith("Method_Private_Void_Boolean_") && m.ReturnType == typeof(void) && m.GetParameters().Count() == 1 && m.GetParameters()[0].IsOptional &&
                        XrefScanner.UsedBy(m).Where(um => um.Type == XrefType.Method && um.TryResolve()?.DeclaringType == typeof(VRCPlayer) && um.TryResolve().Name.Contains(nameof(VRCPlayer.ReloadAvatarNetworkedRPC))).Any()
                ).First();
            }
            catch { };

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

        public static void ReloadAvatar(VRCPlayer player)
        {
            ms_reloadAvatarMethod?.Invoke(player, new object[] { true });
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
