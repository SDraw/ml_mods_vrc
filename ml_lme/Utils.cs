namespace ml_lme
{
    static class Utils
    {
        public static VRCPlayer GetLocalPlayer() => VRCPlayer.field_Internal_Static_VRCPlayer_0;

        public static bool AreHandsTracked() => (UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.LeftHand).isValid || UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.RightHand).isValid);
        public static bool IsInVRMode() => UnityEngine.XR.XRDevice.isPresent;

        public static bool GetGesturesToggle() => (UnityEngine.PlayerPrefs.GetInt(HandGestureController.field_Private_Static_String_0) == 1);
        public static VRCInputManager.InputMethod GetCurrentInput() => VRCInputManager.field_Private_Static_InputMethod_0;
        public static bool IsNonVRInput(VRCInputManager.InputMethod p_input)
        {
            bool l_result = false;
            switch(p_input)
            {
                case VRCInputManager.InputMethod.Mouse:
                case VRCInputManager.InputMethod.Keyboard:
                case VRCInputManager.InputMethod.Controller:
                case VRCInputManager.InputMethod.Osc:
                case VRCInputManager.InputMethod.Count:
                    l_result = true;
                    break;
            }
            return l_result;
        }
    }
}
