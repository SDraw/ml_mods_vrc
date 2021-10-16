using System;
using System.Runtime.InteropServices;

namespace ml_kte
{
    public static class KinectHandlerV2
    {
        [DllImport("ml_kte_cpp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void LaunchKinect();

        [DllImport("ml_kte_cpp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TerminateKinect();

        [DllImport("ml_kte_cpp.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetKinectData(IntPtr f_positions, IntPtr f_rotations); // bool, 75 floats, 100 floats
    }
}
