using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ml_lat
{
    static class MethodsResolver
    {
        static MethodInfo ms_checkPoseChange = null;

        public static void Resolve()
        {
            // void VRCVrIkController.CheckPoseChange(bool force = false)
            if(ms_checkPoseChange == null)
            {
                var l_methods = typeof(VRCVrIkController).GetMethods().Where(m =>
                    m.Name.StartsWith("Method_Private_Void_Boolean_") && 
                    (m.GetParameters().Count() == 1) && 
                    m.GetParameters()[0].IsOptional
                );

                if(l_methods.Any())
                {
                    ms_checkPoseChange = l_methods.First();
                    Logger.DebugMessage("VRCVrIkController.CheckPoseChange -> VRCVrIkController." + ms_checkPoseChange.Name);
                }
                else
                    Logger.Warning("Can't resolve VRCVrIkController.CheckPoseChange");
            }
        }

        public static MethodInfo CheckPoseChange
        {
            get => ms_checkPoseChange;
        }
    }
}
