using System.Linq;
using System.Reflection;

namespace ml_alg
{
    static class MethodsResolver
    {
        static MethodInfo ms_getPlayerById = null;

        public static void ResolveMethods()
        {
            // VRC.Player VRC.PlayerManager.GetPlayer(string userId)
            if(ms_getPlayerById == null)
            {
                var l_methodsList = typeof(VRC.PlayerManager).GetMethods()
                    .Where(m => m.Name.StartsWith("Method_Public_Static_Player_String_") && m.ReturnType == typeof(VRC.Player) && m.GetParameters().Count() == 1 && UnhollowerRuntimeLib.XrefScans.XrefScanner.UsedBy(m)
                    .Where(x => x.Type == UnhollowerRuntimeLib.XrefScans.XrefType.Method && x.TryResolve()?.DeclaringType == typeof(VRC.Management.ModerationManager)).Any() && UnhollowerRuntimeLib.XrefScans.XrefScanner.UsedBy(m)
                    .Where(x => x.Type == UnhollowerRuntimeLib.XrefScans.XrefType.Method && x.TryResolve()?.DeclaringType == typeof(VRC.UI.PageUserInfo)).Any());
                if(l_methodsList.Count() != 0)
                {
                    ms_getPlayerById = l_methodsList.First();
                    MelonLoader.MelonDebug.Msg("VRC.PlayerManager.GetPlayer -> VRC.PlayerManager." + ms_getPlayerById.Name);
                }
                else
                    MelonLoader.MelonLogger.Warning("Can't resolve VRC.PlayerManager.GetPlayer");
            }
        }

        public static MethodInfo GetPlayerById
        {
            get => ms_getPlayerById;
        }
    }
}
