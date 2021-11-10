using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ml_ht
{
    static class GameUtils
    {
        static public Action OnRoomJoined;
        static public Action OnRoomLeft;

        public static void Initialize(HarmonyLib.Harmony f_instance)
        {
            f_instance.Patch(typeof(NetworkManager).GetMethod(nameof(NetworkManager.OnJoinedRoom)), null, new HarmonyLib.HarmonyMethod(typeof(GameUtils).GetMethod(nameof(Patch_OnRoomJoined), BindingFlags.NonPublic | BindingFlags.Static)));
            f_instance.Patch(typeof(NetworkManager).GetMethod(nameof(NetworkManager.OnLeftRoom)), null, new HarmonyLib.HarmonyMethod(typeof(GameUtils).GetMethod(nameof(Patch_OnRoomLeft), BindingFlags.NonPublic | BindingFlags.Static)));
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
