using System;
using System.Runtime.InteropServices;

struct FaceData
{
    public float m_headPositionX;
    public float m_headPositionY;
    public float m_headPositionZ;
    public float m_headRotationX;
    public float m_headRotationY;
    public float m_headRotationZ;
    public float m_headRotationW;

    static public byte[] ToBytes(FaceData p_faceData)
    {
        int l_size = Marshal.SizeOf(p_faceData);
        byte[] l_arr = new byte[l_size];

        IntPtr ptr = Marshal.AllocHGlobal(l_size);
        Marshal.StructureToPtr(p_faceData, ptr, true);
        Marshal.Copy(ptr, l_arr, 0, l_size);
        Marshal.FreeHGlobal(ptr);
        return l_arr;
    }

    static public FaceData ToObject(byte[] p_buffer)
    {
        FaceData l_faceData = new FaceData();

        int l_size = Marshal.SizeOf(l_faceData);
        IntPtr l_ptr = Marshal.AllocHGlobal(l_size);

        Marshal.Copy(p_buffer, 0, l_ptr, l_size);

        l_faceData = (FaceData)Marshal.PtrToStructure(l_ptr, l_faceData.GetType());
        Marshal.FreeHGlobal(l_ptr);

        return l_faceData;
    }
}
