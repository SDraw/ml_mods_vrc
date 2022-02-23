using System;
using System.Runtime.InteropServices;

namespace ml_kte
{
    public static class KinectHandlerV1
    {
        static bool ms_valid = false;

        [DllImport("ml_kte_cpp_v1.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void CheckLibrary();

        [DllImport("ml_kte_cpp_v1.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void LaunchKinect();

        [DllImport("ml_kte_cpp_v1.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void TerminateKinect();

        [DllImport("ml_kte_cpp_v1.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void GetKinectData(IntPtr p_positions, IntPtr p_rotations); // 60 floats, 80 floats

        public static void Check()
        {
            if(!ms_valid)
            {
                try
                {
                    CheckLibrary();
                    ms_valid = true;

                    Logger.Message("Kinect 1.x Runtime/SDK detected");
                }
                catch(Exception)
                {
                    Logger.Warning("Kinect 1.x Runtime/SDK isn't installed correctly, no tracking data will be provided");
                }
            }
        }

        public static void Launch()
        {
            if(ms_valid)
                LaunchKinect();
        }
        public static void Terminate()
        {
            if(ms_valid)
                TerminateKinect();
        }
        public static void GetTrackingData(IntPtr p_positions, IntPtr p_rotations)
        {
            if(ms_valid)
                GetKinectData(p_positions, p_rotations);
        }
    }
}
