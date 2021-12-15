using System;
using System.Runtime.InteropServices;

namespace ml_kte
{
    public static class KinectHandlerV2
    {
        static bool ms_valid = false;

        [DllImport("ml_kte_cpp_v2.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void CheckLibrary();

        [DllImport("ml_kte_cpp_v2.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void LaunchKinect();

        [DllImport("ml_kte_cpp_v2.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void TerminateKinect();

        [DllImport("ml_kte_cpp_v2.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void GetKinectData(IntPtr f_positions, IntPtr f_rotations); // 75 floats, 100 floats

        public static void Check()
        {
            if(!ms_valid)
            {
                try
                {
                    CheckLibrary();
                    ms_valid = true;

                    Logger.Message("Kinect 2.0 Runtime/SDK detected");
                }
                catch(Exception)
                {
                    Logger.Warning("Kinect 2.0 Runtime/SDK isn't installed correctly, no tracking data will be provided");
                }
            }
        }

        public static void Launch()
        {
            if(ms_valid) LaunchKinect();
        }
        public static void Terminate()
        {
            if(ms_valid) TerminateKinect();
        }
        public static void GetTrackingData(IntPtr f_positions, IntPtr f_rotations)
        {
            if(ms_valid) GetKinectData(f_positions, f_rotations);
        }
    }
}
