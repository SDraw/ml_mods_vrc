using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ml_arh
{
    static class Utils
    {
        public static VRCPlayer GetLocalPlayer() => VRCPlayer.field_Internal_Static_VRCPlayer_0;
        public static float GetTrackingHeight() => VRCTrackingManager.field_Private_Static_Vector3_0.y;
        public static float GetUprightAmount() => ((MethodsResolver.PlayerUprightAmount != null) ? (float)MethodsResolver.PlayerUprightAmount.Invoke(null, null) : 1f);
    }
}
