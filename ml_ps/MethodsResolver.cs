using System.Linq;
using System.Reflection;
using UnhollowerRuntimeLib.XrefScans;

namespace ml_ps
{
    static class MethodsResolver
    {
        static MethodInfo ms_playSound = null;

        public static void ResolveMethods()
        {
            // static void VRCUiSoundPlayer.PlaySound(AudioClip clip, float vol = 1f)
            if(ms_playSound == null)
            {
                var l_methodsList = typeof(VRCUiSoundPlayer).GetMethods().Where(m =>
                    m.Name.StartsWith("Method_Public_Static_Void_AudioClip_Single_") && (m.ReturnType == typeof(void)) && (m.GetParameters().Count() == 2) &&
                    XrefScanner.UsedBy(m).Where(x => (x.Type == XrefType.Method) && (x.TryResolve()?.DeclaringType == typeof(ActionMenu))).Any() &&
                    XrefScanner.UsedBy(m).Where(x => (x.Type == XrefType.Method) && (x.TryResolve()?.DeclaringType == typeof(AxisPuppetMenu))).Any() &&
                    XrefScanner.UsedBy(m).Where(x => (x.Type == XrefType.Method) && (x.TryResolve()?.DeclaringType == typeof(HudVoiceIndicator))).Any() &&
                    XrefScanner.UsedBy(m).Where(x => (x.Type == XrefType.Method) && (x.TryResolve()?.DeclaringType == typeof(UserIconCameraMenu))).Any() &&
                    XrefScanner.UsedBy(m).Where(x => (x.Type == XrefType.Method) && (x.TryResolve()?.DeclaringType == typeof(VRCUiPage))).Any() &&
                    XrefScanner.UsedBy(m).Where(x => (x.Type == XrefType.Method) && (x.TryResolve()?.DeclaringType == typeof(VRC.UserCamera.UserCameraController))).Any() &&
                    XrefScanner.UsedBy(m).Where(x => (x.Type == XrefType.Method) && (x.TryResolve()?.DeclaringType == typeof(RadialPuppetMenu))).Any()
                );

                if(l_methodsList.Any())
                {
                    ms_playSound = l_methodsList.First();
                    Logger.DebugMessage("VRCUiSoundPlayer.PlaySound -> VRCUiSoundPlayer." + ms_playSound.Name);
                }
                else
                    Logger.Warning("Can't resolve VRCUiSoundPlayer.PlaySound");
            }
        }

        public static MethodInfo PlaySound
        {
            get => ms_playSound;
        }
    }
}
