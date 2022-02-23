using System.Linq;
using System.Reflection;
using UnhollowerRuntimeLib.XrefScans;

namespace ml_abp
{
    static class MethodsResolver
    {
        static MethodInfo ms_setAvatarFloatParam = null;
        static MethodInfo ms_getPlayerById = null;

        public static void ResolveMethods()
        {
            // void AvatarPlayableController.SetAvatarFloatParam(int paramHash, float val, bool debug = false)
            if(ms_setAvatarFloatParam == null)
            {
                var l_methodsList = typeof(AvatarPlayableController).GetMethods().Where(m =>
                    m.Name.StartsWith("Method_Public_Void_Int32_Single_Boolean_") && (m.ReturnType == typeof(void)) && (m.GetParameters().Count() == 3) &&
                    XrefScanner.XrefScan(m).Where(x => (x.Type == XrefType.Method) && (x.TryResolve()?.DeclaringType == typeof(VRC.Playables.AvatarParameter))).Any() &&
                    XrefScanner.UsedBy(m).Where(x => (x.Type == XrefType.Method) && (x.TryResolve()?.DeclaringType == typeof(AvatarAnimParamController))).Any() &&
                    XrefScanner.UsedBy(m).Where(x => (x.Type == XrefType.Method) && (x.TryResolve()?.DeclaringType == typeof(AvatarPlayableController))).Any()
                );

                if(l_methodsList.Any())
                {
                    ms_setAvatarFloatParam = l_methodsList.First();
                    Logger.DebugMessage("AvatarPlayableController.SetAvatarFloatParam -> AvatarPlayableController." + ms_setAvatarFloatParam.Name);
                }
                else
                    Logger.Warning("Can't resolve AvatarPlayableController.SetAvatarFloatParam");
            }

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
        }

        public static MethodInfo SetAvatarFloatParam
        {
            get => ms_setAvatarFloatParam;
        }
        public static MethodInfo GetPlayerById
        {
            get => ms_getPlayerById;
        }
    }
}
