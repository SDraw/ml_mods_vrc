using System.Collections.Generic;
using UnityEngine;

namespace ml_clv
{
    public class CalibrationLinesVisualizer : MelonLoader.MelonMod
    {
        static CalibrationLinesVisualizer ms_instance = null;
        bool m_quit = false;

        bool m_activeCalibration = false;

        List<GameObject> m_trackerLines = null;

        public override void OnApplicationStart()
        {
            if(ms_instance == null)
                ms_instance = this;

            m_trackerLines = new List<GameObject>();

            MethodsResolver.ResolveMethods();
            Settings.LoadSettings();

            // Events
            VRChatUtilityKit.Utilities.VRCUtils.OnUiManagerInit += this.OnUiManagerInit;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomJoined += this.OnRoomJoined;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomLeft += this.OnRoomLeft;

            // Patches
            if(MethodsResolver.PrepareForCalibration != null)
                HarmonyInstance.Patch(MethodsResolver.PrepareForCalibration, null, new HarmonyLib.HarmonyMethod(typeof(CalibrationLinesVisualizer), nameof(OnPrepareForCalibration_Postfix)));

            if(MethodsResolver.RestoreTrackingAfterCalibration != null)
                HarmonyInstance.Patch(MethodsResolver.RestoreTrackingAfterCalibration, null, new HarmonyLib.HarmonyMethod(typeof(CalibrationLinesVisualizer), nameof(OnRestoreTrackingAfterCalibration_Postfix)));

            if(MethodsResolver.IKTweaks_Calibrate != null)
                HarmonyInstance.Patch(MethodsResolver.IKTweaks_Calibrate, new HarmonyLib.HarmonyMethod(typeof(CalibrationLinesVisualizer), nameof(OnPrepareForCalibration_Postfix)), null);

            if(MethodsResolver.IKTweaks_ApplyStoredCalibration != null)
                HarmonyInstance.Patch(MethodsResolver.IKTweaks_ApplyStoredCalibration, new HarmonyLib.HarmonyMethod(typeof(CalibrationLinesVisualizer), nameof(OnRestoreTrackingAfterCalibration_Postfix)), null);
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
                if(Settings.IsAnyEntryUpdated())
                {
                    if(TrackerBoneLine.Material != null)
                        TrackerBoneLine.Material.color = new Color(Settings.ColorR, Settings.ColorG, Settings.ColorB);

                    foreach(GameObject l_trackerLine in m_trackerLines)
                        l_trackerLine.active = (Settings.Enabled && m_activeCalibration);
                }
            }
        }

        void OnUiManagerInit()
        {
            MelonLoader.MelonCoroutines.Start(CreateTrackerLines());
        }
        System.Collections.IEnumerator CreateTrackerLines()
        {
            while(Utils.GetVRCTrackingManager() == null)
                yield return null;
            while(Utils.GetVRCTrackingSteam() == null)
                yield return null;
            while(Utils.GetSteamVRControllerManager() == null)
                yield return null;

            TrackerBoneLine.ControllerManager = Utils.GetSteamVRControllerManager();

            // Material, thank Requi for this code
            TrackerBoneLine.Material = new Material(Shader.Find("Hidden/Internal-Colored"));
            TrackerBoneLine.Material.hideFlags = HideFlags.HideAndDontSave;
            TrackerBoneLine.Material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            TrackerBoneLine.Material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            TrackerBoneLine.Material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            TrackerBoneLine.Material.SetInt("_ZWrite", 0);
            TrackerBoneLine.Material.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
            TrackerBoneLine.Material.color = new Color(Settings.ColorR, Settings.ColorG, Settings.ColorB);

            // Game objects
            var l_puckArray = Utils.GetSteamVRControllerManager().field_Public_ArrayOf_GameObject_0;
            for(int i = 0, j = l_puckArray.Length - 2; i < j; i++)
            {
                GameObject l_obj = new GameObject("Line");
                l_obj.active = false;
                l_obj.layer = LayerMask.NameToLayer("Player");
                l_obj.transform.parent = l_puckArray[i + 2].transform;
                l_obj.transform.localPosition = Vector3.zero;
                l_obj.transform.localRotation = Quaternion.identity;
                Object.DontDestroyOnLoad(l_obj);

                l_obj.AddComponent<TrackerBoneLine>().Index = i + 2;

                m_trackerLines.Add(l_obj);
            }
        }

        void OnRoomJoined()
        {
            MelonLoader.MelonCoroutines.Start(AssignLocalPlayer());
        }
        System.Collections.IEnumerator AssignLocalPlayer()
        {
            while(Utils.GetLocalPlayer() == null)
                yield return null;

            TrackerBoneLine.Player = Utils.GetLocalPlayer();
        }

        void OnRoomLeft()
        {
            OnRestoreTrackingAfterCalibration();

            TrackerBoneLine.Player = null;
        }

        static public void OnPrepareForCalibration_Postfix() => ms_instance?.OnPrepareForCalibration();
        void OnPrepareForCalibration()
        {
            if(Settings.Enabled && !m_activeCalibration)
            {
                m_activeCalibration = true;

                foreach(GameObject l_trackerLine in m_trackerLines)
                    l_trackerLine.active = true;
            }
        }

        static public void OnRestoreTrackingAfterCalibration_Postfix() => ms_instance?.OnRestoreTrackingAfterCalibration();
        void OnRestoreTrackingAfterCalibration()
        {
            if(m_activeCalibration)
            {
                m_activeCalibration = false;

                foreach(GameObject l_trackerLine in m_trackerLines)
                    l_trackerLine.active = false;
            }
        }
    }
}
