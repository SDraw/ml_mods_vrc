using System.Linq;
using System.Reflection;
using UnhollowerRuntimeLib.XrefScans;

namespace ml_arh
{
    static class MethodsResolver
    {
        static MethodInfo ms_uprightAmount = null;

        public static void Resolve()
        {
            // static float VRCTrackingManager.GetPlayerUprightAmount()
            if(ms_uprightAmount == null)
            {
                var l_methodsList = typeof(VRCTrackingManager).GetMethods().Where(m =>
                    m.Name.StartsWith("Method_Public_Static_Single_") && (m.ReturnType == typeof(float)) && !m.GetParameters().Any() &&
                    XrefScanner.UsedBy(m).Where(x => (x.Type == XrefType.Method) && (x.TryResolve()?.DeclaringType == typeof(IkController))).Any() &&
                    XrefScanner.XrefScan(m).Where(x => (x.Type == XrefType.Method) && (x.TryResolve()?.DeclaringType == typeof(VRCPlayer))).Any()
                );

                if(l_methodsList.Any())
                {
                    ms_uprightAmount = l_methodsList.First();
                    Logger.DebugMessage("VRCTrackingManager.GetPlayerUprightAmount ->  VRCTrackingManager." + ms_uprightAmount.Name);
                }
                else
                    Logger.Warning("Can't resolve VRCTrackingManager.GetPlayerUprightAmount");
            }
        }

        public static MethodInfo PlayerUprightAmount
        {
            get => ms_uprightAmount;
        }
    }
}
