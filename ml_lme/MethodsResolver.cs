using System.Linq;
using UnhollowerRuntimeLib.XrefScans;

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
                var l_methodsList = typeof(VRCTrackingManager).GetMethods().Where(m =>
                    m.Name.StartsWith("Method_Public_Static_Boolean_") && (m.ReturnType == typeof(bool)) && !m.GetParameters().Any() &&
                    (XrefScanner.UsedBy(m).Where(x => (x.Type == XrefType.Method) && (x.TryResolve()?.DeclaringType == typeof(AvatarDebugConsole))).Count() > 1) &&
                    XrefScanner.UsedBy(m).Where(x => (x.Type == XrefType.Method) && (x.TryResolve()?.DeclaringType == typeof(VRCFlowManager))).Any() &&
                    XrefScanner.UsedBy(m).Where(x => (x.Type == XrefType.Method) && (x.TryResolve()?.DeclaringType == typeof(SpawnManager))).Any()
                );

                if(l_methodsList.Any())
                {
                    ms_isInVR = l_methodsList.First();
                    Logger.DebugMessage("VRCTrackingManager.IsInVR -> VRCTrackingManager." + ms_isInVR.Name);
                }
                else
                    Logger.Warning("Can't resolve VRCTrackingManager.IsInVR");
            }

            // void AvatarPlayableController.SetAvatarIntParam(int paramHash, int val)
            if(ms_setAvatarIntParam == null)
            {
                var l_methodsList = typeof(AvatarPlayableController).GetMethods().Where(m =>
                    m.Name.StartsWith("Method_Public_Void_Int32_Int32_") && (m.ReturnType == typeof(void)) && (m.GetParameters().Count() == 2) &&
                    XrefScanner.XrefScan(m).Where(x => (x.Type == XrefType.Method) && (x.TryResolve()?.DeclaringType == typeof(VRC.Playables.AvatarParameter))).Any() &&
                    XrefScanner.UsedBy(m).Where(x => (x.Type == XrefType.Method) && (x.TryResolve()?.DeclaringType == typeof(AvatarAnimParamController))).Any() &&
                    XrefScanner.UsedBy(m).Where(x => (x.Type == XrefType.Method) && (x.TryResolve()?.DeclaringType == typeof(AvatarPlayableController))).Any() &&
                    XrefScanner.UsedBy(m).Where(x => (x.Type == XrefType.Method) && (x.TryResolve()?.DeclaringType == typeof(JawController))).Any() &&
                    XrefScanner.UsedBy(m).Where(x => (x.Type == XrefType.Method) && (x.TryResolve()?.DeclaringType == typeof(OVRLipSyncContextPlayableParam))).Any()
                );

                if(l_methodsList.Any())
                {
                    ms_setAvatarIntParam = l_methodsList.First();
                    Logger.DebugMessage("AvatarPlayableController.SetAvatarIntParam -> AvatarPlayableController." + ms_setAvatarIntParam.Name);
                }
                else
                    Logger.Warning("Can't resolve AvatarPlayableController.SetAvatarIntParam");
            }

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

            // void AvatarPlayableController.SetAvatarBoolParam(int paramHash, bool val)
            if(ms_setAvatarBoolParam == null)
            {
                var l_methodsList = typeof(AvatarPlayableController).GetMethods().Where(m =>
                    m.Name.StartsWith("Method_Public_Void_Int32_Boolean_") && (m.ReturnType == typeof(void)) && (m.GetParameters().Count() == 2) &&
                    !XrefScanner.XrefScan(m).Where(x => (x.Type == XrefType.Method) && (x.TryResolve()?.DeclaringType == typeof(UnityEngine.Playables.PlayableExtensions))).Any() &&
                    XrefScanner.XrefScan(m).Where(x => (x.Type == XrefType.Method) && (x.TryResolve()?.DeclaringType == typeof(VRC.Playables.AvatarParameter))).Any() &&
                    XrefScanner.UsedBy(m).Where(x => (x.Type == XrefType.Method) && (x.TryResolve()?.DeclaringType == typeof(AvatarAnimParamController))).Any() &&
                    XrefScanner.UsedBy(m).Where(x => (x.Type == XrefType.Method) && (x.TryResolve()?.DeclaringType == typeof(AvatarPlayableController))).Any()
                );

                if(l_methodsList.Any())
                {
                    ms_setAvatarBoolParam = l_methodsList.First();
                    Logger.DebugMessage("AvatarPlayableController.SetAvatarBoolParam -> AvatarPlayableController." + ms_setAvatarBoolParam.Name);
                }
                else
                    Logger.Warning("Can't resolve AvatarPlayableController.SetAvatarBoolParam");
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
