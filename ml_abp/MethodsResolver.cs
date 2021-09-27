using System.Linq;
using System.Reflection;

namespace ml_abp
{
    static class MethodsResolver
    {
        static MethodInfo ms_setAvatarFloatParam = null;

        public static void ResolveMethods()
        {
            // void AvatarPlayableController.SetAvatarFloatParam(int paramHash, float val, bool debug = false)
            if(ms_setAvatarFloatParam == null)
            {
                var l_methodsList = typeof(AvatarPlayableController).GetMethods()
                   .Where(m => m.Name.StartsWith("Method_Public_Void_Int32_Single_Boolean_") && m.ReturnType == typeof(void) && m.GetParameters().Count() == 3 && UnhollowerRuntimeLib.XrefScans.XrefScanner.XrefScan(m)
                   .Where(x => x.Type == UnhollowerRuntimeLib.XrefScans.XrefType.Method && x.TryResolve()?.DeclaringType == typeof(VRC.Playables.AvatarParameter)).Count() > 0 && UnhollowerRuntimeLib.XrefScans.XrefScanner.UsedBy(m)
                    .Where(x => x.Type == UnhollowerRuntimeLib.XrefScans.XrefType.Method && x.TryResolve()?.DeclaringType == typeof(AvatarAnimParamController)).Count() > 0 && UnhollowerRuntimeLib.XrefScans.XrefScanner.UsedBy(m)
                    .Where(x => x.Type == UnhollowerRuntimeLib.XrefScans.XrefType.Method && x.TryResolve()?.DeclaringType == typeof(AvatarPlayableController)).Count() > 0);

                if(l_methodsList.Count() != 0)
                {
                    ms_setAvatarFloatParam = l_methodsList.First();
                    MelonLoader.MelonDebug.Msg("AvatarPlayableController.SetAvatarFloatParam -> AvatarPlayableController." + ms_setAvatarFloatParam.Name);
                }
                else
                    MelonLoader.MelonLogger.Warning("Can't resolve AvatarPlayableController.SetAvatarFloatParam");
            }
        }

        public static MethodInfo SetAvatarFloatParam
        {
            get => ms_setAvatarFloatParam;
        }
    }
}
