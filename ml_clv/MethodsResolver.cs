﻿using System;
using System.Reflection;
using System.Linq;
using HarmonyLib;

namespace ml_clv
{
    static class MethodsResolver
    {
        static MethodInfo ms_prepareForCalibration = null;
        static MethodInfo ms_restoreTrackingAfterCalibration = null;
        static MethodInfo ms_calibrate = null; // IKTweaks
        static MethodInfo ms_applyStoredCalibration = null; // IKTweaks

        public static void ResolveMethods()
        {
            // void VRCTrackingManager.PrepareForCalibration()
            if(ms_prepareForCalibration == null)
            {
                var l_methods = typeof(VRCTrackingManager).GetMethods()
                .Where(m => (m.Name.StartsWith("Method_Public_Static_Void_") && m.ReturnType == typeof(void) && m.GetParameters().Count() == 0 && UnhollowerRuntimeLib.XrefScans.XrefScanner.XrefScan(m)
                .Where(x => (x.Type == UnhollowerRuntimeLib.XrefScans.XrefType.Global && x.ReadAsObject().ToString().Contains("trying to calibrate"))).Any() && UnhollowerRuntimeLib.XrefScans.XrefScanner.UsedBy(m)
                .Where(x => (x.Type == UnhollowerRuntimeLib.XrefScans.XrefType.Method && x.TryResolve()?.DeclaringType == typeof(VRCFbbIkController))).Any()));

                if(l_methods.Count() != 0)
                {
                    ms_prepareForCalibration = l_methods.First();
                    MelonLoader.MelonDebug.Msg("VRCTrackingManager.PrepareForCalibration -> VRCTrackingManager." + ms_prepareForCalibration.Name);
                }
                else
                    MelonLoader.MelonLogger.Warning("Can't resolve VRCTrackingManager.PrepareForCalibration");
            }

            // void VRCTracking.RestoreTrackingAfterCalibration()
            if(ms_restoreTrackingAfterCalibration == null)
            {
                var l_methods = typeof(VRCTrackingManager).GetMethods()
                       .Where(m => (m.Name.StartsWith("Method_Public_Static_Void_") && m.Name != ms_prepareForCalibration?.Name && m.ReturnType == typeof(void) && m.GetParameters().Count() == 0 && UnhollowerRuntimeLib.XrefScans.XrefScanner.UsedBy(m)
                       .Where(x => (x.Type == UnhollowerRuntimeLib.XrefScans.XrefType.Method && x.TryResolve()?.DeclaringType == typeof(VRCFbbIkController))).Any()));

                if(l_methods.Count() != 0)
                {
                    ms_restoreTrackingAfterCalibration = l_methods.First();
                    MelonLoader.MelonDebug.Msg("VRCTrackingManager.RestoreTrackingAfterCalibration -> VRCTrackingManager." + ms_restoreTrackingAfterCalibration.Name);
                }
                else
                    MelonLoader.MelonLogger.Warning("Can't resolve VRCTrackingManager.RestoreTrackingAfterCalibration");
            }

            // void IKTweaks.CalibrationManager.Calibrate(GameObject avatarRoot)
            if(ms_calibrate == null)
            {
                foreach(var l_mod in MelonLoader.MelonHandler.Mods)
                {
                    if(l_mod.Info.Name == "IKTweaks")
                    {
                        Type l_cbType = null;
                        l_mod.Assembly.GetTypes().DoIf(t => t.Name == "CalibrationManager", t => l_cbType = t);
                        if(l_cbType != null)
                        {
                            ms_calibrate = l_cbType.GetMethod("Calibrate");
                            MelonLoader.MelonDebug.Msg("IKTweaks.CalibrationManager.Calibrate " + ((ms_calibrate != null) ? "found" : "not found"));
                            break;
                        }
                    }
                }
            }

            // Task IKTweaks.CalibrationManager.ApplyStoredCalibration(GameObject avatarRoot, string avatarId)
            if(ms_applyStoredCalibration == null)
            {
                foreach(var l_mod in MelonLoader.MelonHandler.Mods)
                {
                    if(l_mod.Info.Name == "IKTweaks")
                    {
                        Type l_cbType = null;
                        l_mod.Assembly.GetTypes().DoIf(t => t.Name == "CalibrationManager", t => l_cbType = t);
                        if(l_cbType != null)
                        {
                            ms_applyStoredCalibration = l_cbType.GetMethod("ApplyStoredCalibration", BindingFlags.NonPublic | BindingFlags.Static);
                            MelonLoader.MelonDebug.Msg("IKTweaks.CalibrationManager.ApplyStoredCalibration " + ((ms_applyStoredCalibration != null) ? "found" : "not found"));
                            break;
                        }
                    }
                }
            }
        }

        public static MethodInfo PrepareForCalibration
        {
            get => ms_prepareForCalibration;
        }
        public static MethodInfo RestoreTrackingAfterCalibration
        {
            get => ms_restoreTrackingAfterCalibration;
        }
        public static MethodInfo IKTweaks_Calibrate
        {
            get => ms_calibrate;
        }
        public static MethodInfo IKTweaks_ApplyStoredCalibration
        {
            get => ms_applyStoredCalibration;
        }
    }
}