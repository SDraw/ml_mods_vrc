using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using UnhollowerRuntimeLib.XrefScans;

namespace ml_alg
{
    static class MethodsResolver
    {
        static MethodInfo ms_getPlayerById = null;
        static MethodInfo ms_preSetupVrIk = null; // IKTweaks

        public static void ResolveMethods()
        {
            // VRC.Player VRC.PlayerManager.GetPlayer(string userId)
            if(ms_getPlayerById == null)
            {
                var l_methodsList = typeof(VRC.PlayerManager).GetMethods().Where(m =>
                    m.Name.StartsWith("Method_Public_Static_Player_String_") && (m.ReturnType == typeof(VRC.Player)) && (m.GetParameters().Count() == 1) &&
                    XrefScanner.UsedBy(m).Where(x => (x.Type == XrefType.Method) && (x.TryResolve()?.DeclaringType == typeof(VRC.Management.ModerationManager))).Any() &&
                    XrefScanner.UsedBy(m).Where(x => (x.Type == XrefType.Method) && (x.TryResolve()?.DeclaringType == typeof(VRC.UI.PageUserInfo))).Any()
                );

                if(l_methodsList.Any())
                {
                    ms_getPlayerById = l_methodsList.First();
                    Logger.DebugMessage("VRC.PlayerManager.GetPlayer -> VRC.PlayerManager." + ms_getPlayerById.Name);
                }
                else
                    Logger.Warning("Can't resolve VRC.PlayerManager.GetPlayer");
            }

            if(ms_preSetupVrIk == null)
            {
                foreach(MelonLoader.MelonMod l_mod in MelonLoader.MelonHandler.Mods)
                {
                    if(l_mod.Info.Name == "IKTweaks")
                    {
                        Type l_fbhType = null;
                        l_mod.Assembly.GetTypes().DoIf(t => t.Name == "FullBodyHandling", t => l_fbhType = t);
                        if(l_fbhType != null)
                        {
                            ms_preSetupVrIk = l_fbhType.GetMethod("PreSetupVrIk", BindingFlags.Static | BindingFlags.NonPublic);
                            Logger.DebugMessage("IKTweaks.FullBodyHandling.PreSetupVrIk " + ((ms_preSetupVrIk != null) ? "found" : "not found"));
                        }
                        break;
                    }
                }
            }
        }

        public static MethodInfo GetPlayerById
        {
            get => ms_getPlayerById;
        }

        public static MethodInfo PreSetupVRIK
        {
            get => ms_preSetupVrIk;
        }
    }
}
