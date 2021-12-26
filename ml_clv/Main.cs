using System.Collections.Generic;
using UnityEngine;

namespace ml_clv
{
    public class Main : MelonLoader.MelonMod
    {
        static Main ms_instance = null;
        bool m_quit = false;

        List<TrackerBoneLine> m_trackerLines = null;
        bool m_calibrationInProgress = false;

        public override void OnApplicationStart()
        {
            ms_instance = this;

            MethodsResolver.ResolveMethods();
            Settings.LoadSettings();

            m_trackerLines = new List<TrackerBoneLine>();

            // Events
            VRChatUtilityKit.Utilities.VRCUtils.OnUiManagerInit += this.OnUiManagerInit;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomJoined += this.OnRoomJoined;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomLeft += this.OnRoomLeft;

            // Patches
            if(MethodsResolver.PrepareForCalibration != null)
                HarmonyInstance.Patch(MethodsResolver.PrepareForCalibration, null, new HarmonyLib.HarmonyMethod(typeof(Main), nameof(VRCTrackingManager_PrepareForCalibration)));

            if(MethodsResolver.RestoreTrackingAfterCalibration != null)
                HarmonyInstance.Patch(MethodsResolver.RestoreTrackingAfterCalibration, null, new HarmonyLib.HarmonyMethod(typeof(Main), nameof(VRCTrackingManager_RestoreTrackingAfterCalibration)));

            if(MethodsResolver.IKTweaks_Calibrate != null)
                HarmonyInstance.Patch(MethodsResolver.IKTweaks_Calibrate, new HarmonyLib.HarmonyMethod(typeof(Main), nameof(VRCTrackingManager_PrepareForCalibration)), null);

            if(MethodsResolver.IKTweaks_ApplyStoredCalibration != null)
                HarmonyInstance.Patch(MethodsResolver.IKTweaks_ApplyStoredCalibration, new HarmonyLib.HarmonyMethod(typeof(Main), nameof(VRCTrackingManager_RestoreTrackingAfterCalibration)), null);
        }

        public override void OnApplicationQuit()
        {
            m_quit = true;
        }

        public override void OnPreferencesSaved()
        {
            if(!m_quit) // This is not a joke
            {
                Settings.ReloadSettings();

                if(TrackerBoneLine.GetLineMaterial() != null)
                    TrackerBoneLine.GetLineMaterial().color = new Color(Settings.ColorR, Settings.ColorG, Settings.ColorB);

                if(m_trackerLines.Count != 0)
                {
                    foreach(TrackerBoneLine l_trackerLine in m_trackerLines)
                        l_trackerLine.gameObject.active = (Settings.Enabled && m_calibrationInProgress);
                }
            }
        }

        void OnUiManagerInit()
        {
            MelonLoader.MelonCoroutines.Start(CreateTrackerLines());
        }
        System.Collections.IEnumerator CreateTrackerLines()
        {
            while(Utils.GetSteamVRControllerManager() == null) yield return null;

            TrackerBoneLine.ControllerManager = Utils.GetSteamVRControllerManager();
            if(TrackerBoneLine.GetLineMaterial() != null)
                TrackerBoneLine.GetLineMaterial().color = new Color(Settings.ColorR, Settings.ColorG, Settings.ColorB);

            var l_puckArray = Utils.GetSteamVRControllerManager().field_Public_ArrayOf_GameObject_0;
            for(int i = 0; i < l_puckArray.Length - 2; i++)
            {
                GameObject l_obj = new GameObject("BoneLine");
                l_obj.active = false;
                l_obj.layer = LayerMask.NameToLayer("Player");
                l_obj.transform.parent = l_puckArray[i + 2].transform;
                l_obj.transform.localPosition = Vector3.zero;
                l_obj.transform.localRotation = Quaternion.identity;
                Object.DontDestroyOnLoad(l_obj);

                l_obj.AddComponent<LineRenderer>();
                TrackerBoneLine l_boneLine = l_obj.AddComponent<TrackerBoneLine>();
                l_boneLine.Index = i + 2;

                m_trackerLines.Add(l_boneLine);
            }
        }

        void OnRoomJoined()
        {
            MelonLoader.MelonCoroutines.Start(AssignPlayerToTrackerLines());
        }
        System.Collections.IEnumerator AssignPlayerToTrackerLines()
        {
            while(Utils.GetLocalPlayer() == null) yield return null;

            foreach(TrackerBoneLine l_trackerLine in m_trackerLines)
                l_trackerLine.Player = Utils.GetLocalPlayer();
        }

        void OnRoomLeft()
        {
            OnCalibrationEnd();

            if(m_trackerLines.Count != 0)
            {
                foreach(TrackerBoneLine l_trackerLine in m_trackerLines)
                    l_trackerLine.Player = null;
            }
        }

        static public void VRCTrackingManager_PrepareForCalibration() => ms_instance?.OnCalibrationBegin();
        void OnCalibrationBegin()
        {
            if(Settings.Enabled && !m_calibrationInProgress)
            {
                m_calibrationInProgress = true;

                if(m_trackerLines.Count != 0)
                {
                    foreach(TrackerBoneLine l_trackerLine in m_trackerLines)
                        l_trackerLine.gameObject.active = true;
                }
            }
        }

        static public void VRCTrackingManager_RestoreTrackingAfterCalibration() => ms_instance?.OnCalibrationEnd();
        void OnCalibrationEnd()
        {
            if(Settings.Enabled && m_calibrationInProgress)
            {
                m_calibrationInProgress = false;

                if(m_trackerLines.Count != 0)
                {
                    foreach(TrackerBoneLine l_trackerLine in m_trackerLines)
                        l_trackerLine.gameObject.active = false;
                }
            }
        }
    }
}
