// dllmain.cpp : Определяет точку входа для приложения DLL.
#include "stdafx.h"
#include "CCore.h"

BOOL APIENTRY DllMain(HMODULE /*hModule*/, DWORD /*ul_reason_for_call*/, LPVOID /*lpReserved*/)
{
    return TRUE;
}

CCore *g_core = nullptr;

extern "C" __declspec(dllexport) void LaunchKinect()
{
    if (!g_core)
    {
        g_core = new CCore();
        g_core->Initialize();
    }
}

extern "C" __declspec(dllexport) void TerminateKinect()
{
    if (g_core)
    {
        g_core->Terminate();
        delete g_core;
        g_core = nullptr;
    }
}

extern "C" __declspec(dllexport) void GetKinectData(float *f_positions, float *f_rotations)
{
    if (g_core) g_core->GetTrackingData(f_positions, f_rotations);
}
