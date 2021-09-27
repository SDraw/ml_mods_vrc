using System.Linq;

namespace ml_lme
{
    static class MethodsResolver
    {
        static System.Reflection.MethodInfo ms_isInVR = null;
        static System.Reflection.MethodInfo ms_setAvatarIntParam = null;
        static System.Reflection.MethodInfo ms_setAvatarFloatParam = null;
        static System.Reflection.MethodInfo ms_setAvatarBoolParam = null;

        public static void ResolveMethods()
        {
            // static bool VRCTrackingManager.IsInVR()
            if(ms_isInVR == null)
            {
                var l_methodsList = typeof(VRCTrackingManager).GetMethods()
                    .Where(m => m.Name.StartsWith("Method_Public_Static_Boolean_") && m.ReturnType == typeof(bool) && m.GetParameters().Count() == 0 && UnhollowerRuntimeLib.XrefScans.XrefScanner.UsedBy(m)
                    .Where(x => x.Type == UnhollowerRuntimeLib.XrefScans.XrefType.Method && x.TryResolve()?.DeclaringType == typeof(AvatarDebugConsole)).Count() > 1 && UnhollowerRuntimeLib.XrefScans.XrefScanner.UsedBy(m)
                    .Where(x => x.Type == UnhollowerRuntimeLib.XrefScans.XrefType.Method && x.TryResolve()?.DeclaringType == typeof(VRCFlowManager)).Count() >= 1 && UnhollowerRuntimeLib.XrefScans.XrefScanner.UsedBy(m)
                    .Where(x => x.Type == UnhollowerRuntimeLib.XrefScans.XrefType.Method && x.TryResolve()?.DeclaringType == typeof(SpawnManager)).Count() >= 1);

                if(l_methodsList.Count() != 0)
                {
                    ms_isInVR = l_methodsList.First();
                    MelonLoader.MelonDebug.Msg("VRCTrackingManager.IsInVR -> VRCTrackingManager." + ms_isInVR.Name);
                }
                else
                    MelonLoader.MelonLogger.Warning("Can't resolve VRCTrackingManager.IsInVR");
            }

            // void AvatarPlayableController.SetAvatarIntParam(int paramHash, int val)
            if(ms_setAvatarIntParam == null)
            {
                var l_methodsList = typeof(AvatarPlayableController).GetMethods()
                    .Where(m => m.Name.StartsWith("Method_Public_Void_Int32_Int32_") && m.ReturnType == typeof(void) && m.GetParameters().Count() == 2 && UnhollowerRuntimeLib.XrefScans.XrefScanner.XrefScan(m)
                    .Where(x => x.Type == UnhollowerRuntimeLib.XrefScans.XrefType.Method && x.TryResolve()?.DeclaringType == typeof(VRC.Playables.AvatarParameter)).Count() > 0 && UnhollowerRuntimeLib.XrefScans.XrefScanner.UsedBy(m)
                    .Where(x => x.Type == UnhollowerRuntimeLib.XrefScans.XrefType.Method && x.TryResolve()?.DeclaringType == typeof(AvatarAnimParamController)).Count() > 0 && UnhollowerRuntimeLib.XrefScans.XrefScanner.UsedBy(m)
                    .Where(x => x.Type == UnhollowerRuntimeLib.XrefScans.XrefType.Method && x.TryResolve()?.DeclaringType == typeof(AvatarPlayableController)).Count() > 0 && UnhollowerRuntimeLib.XrefScans.XrefScanner.UsedBy(m)
                    .Where(x => x.Type == UnhollowerRuntimeLib.XrefScans.XrefType.Method && x.TryResolve()?.DeclaringType == typeof(JawController)).Count() > 0 && UnhollowerRuntimeLib.XrefScans.XrefScanner.UsedBy(m)
                    .Where(x => x.Type == UnhollowerRuntimeLib.XrefScans.XrefType.Method && x.TryResolve()?.DeclaringType == typeof(OVRLipSyncContextPlayableParam)).Count() > 0);

                if(l_methodsList.Count() != 0)
                {
                    ms_setAvatarIntParam = l_methodsList.First();
                    MelonLoader.MelonDebug.Msg("AvatarPlayableController.SetAvatarIntParam -> AvatarPlayableController." + ms_setAvatarIntParam.Name);
                }
                else
                    MelonLoader.MelonLogger.Warning("Can't resolve AvatarPlayableController.SetAvatarIntParam");
            }

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

            // void AvatarPlayableController.SetAvatarBoolParam(int paramHash, bool val)
            if(ms_setAvatarBoolParam == null)
            {

                var l_methodsList = typeof(AvatarPlayableController).GetMethods()
                    .Where(m => m.Name.StartsWith("Method_Public_Void_Int32_Boolean_") && m.ReturnType == typeof(void) && m.GetParameters().Count() == 2 && UnhollowerRuntimeLib.XrefScans.XrefScanner.XrefScan(m)
                    .Where(x => x.Type == UnhollowerRuntimeLib.XrefScans.XrefType.Method && x.TryResolve()?.DeclaringType == typeof(UnityEngine.Playables.PlayableExtensions)).Count() == 0 && UnhollowerRuntimeLib.XrefScans.XrefScanner.XrefScan(m)
                    .Where(x => x.Type == UnhollowerRuntimeLib.XrefScans.XrefType.Method && x.TryResolve()?.DeclaringType == typeof(VRC.Playables.AvatarParameter)).Count() > 0 && UnhollowerRuntimeLib.XrefScans.XrefScanner.UsedBy(m)
                    .Where(x => x.Type == UnhollowerRuntimeLib.XrefScans.XrefType.Method && x.TryResolve()?.DeclaringType == typeof(AvatarAnimParamController)).Count() > 0 && UnhollowerRuntimeLib.XrefScans.XrefScanner.UsedBy(m)
                    .Where(x => x.Type == UnhollowerRuntimeLib.XrefScans.XrefType.Method && x.TryResolve()?.DeclaringType == typeof(AvatarPlayableController)).Count() > 0);

                if(l_methodsList.Count() != 0)
                {
                    ms_setAvatarBoolParam = l_methodsList.First();
                    MelonLoader.MelonDebug.Msg("AvatarPlayableController.SetAvatarBoolParam -> AvatarPlayableController." + ms_setAvatarBoolParam.Name);
                }
                else
                    MelonLoader.MelonLogger.Warning("Can't resolve AvatarPlayableController.SetAvatarBoolParam");
            }
        }

        public static System.Reflection.MethodInfo IsInVR
        {
            get => ms_isInVR;
        }

        public static System.Reflection.MethodInfo SetAvatarIntParam
        {
            get => ms_setAvatarIntParam;
        }

        public static System.Reflection.MethodInfo SetAvatarFloatParam
        {
            get => ms_setAvatarFloatParam;
        }

        public static System.Reflection.MethodInfo SetAvatarBoolParam
        {
            get => ms_setAvatarBoolParam;
        }
    }
}
