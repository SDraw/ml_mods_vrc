﻿using UnityEngine;

namespace ml_vsf
{
    public class VsfExtension : MelonLoader.MelonMod
    {
        static VsfExtension ms_instance = null;

        bool m_quit = false;

        MemoryMapReader m_mapReader = null;
        byte[] m_buffer = null;

        VsfTracked m_localTracked = null;
        GameObject m_headTracker = null;
        FaceData m_faceData;
        Vector3 m_headOffset;

        public override void OnApplicationStart()
        {
            if(ms_instance == null)
                ms_instance = this;

            Settings.Load();

            m_mapReader = new MemoryMapReader();
            m_mapReader.Open("ml_vsf/data");

            m_buffer = new byte[1024];

            VRChatUtilityKit.Utilities.VRCUtils.OnUiManagerInit += this.OnUiManagerInit;
            VRChatUtilityKit.Utilities.NetworkEvents.OnPlayerJoined += this.OnPlayerJoined;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomLeft += this.OnRoomLeft;

            // Patches
            HarmonyInstance.Patch(
                typeof(RootMotion.FinalIK.IKSolverVR).GetMethod(nameof(RootMotion.FinalIK.IKSolverVR.VrcLateSolve)),
                new HarmonyLib.HarmonyMethod(typeof(VsfExtension), nameof(IKSolverVR_VrcLateSolve_Prefix))
            );
        }

        public override void OnApplicationQuit()
        {
            if(!m_quit)
            {
                m_quit = true;

                m_buffer = null;

                m_mapReader?.Close();
                m_mapReader = null;
            }
        }

        public override void OnPreferencesSaved()
        {
            if(!m_quit)
            {
                Settings.Reload();
                if(Settings.IsEnableEntryUpdated() && !Settings.Enabled && (m_localTracked != null))
                    m_localTracked.ResetViewPoint();
            }
        }

        public override void OnUpdate()
        {
            if(Settings.Enabled && (m_localTracked != null) && m_mapReader.Read(ref m_buffer))
            {
                m_faceData = FaceData.ToObject(m_buffer);

                m_headTracker.transform.localPosition = Vector3.Lerp(
                    new Vector3(Settings.Mirroring ? -m_faceData.m_headPositionX : m_faceData.m_headPositionX, m_faceData.m_headPositionY, m_faceData.m_headPositionZ) + m_headOffset,
                    m_headTracker.transform.localPosition,
                    Settings.Blending
                );
                m_headTracker.transform.localRotation = Quaternion.Slerp(
                    new Quaternion(m_faceData.m_headRotationX, Settings.Mirroring ? -m_faceData.m_headRotationY : m_faceData.m_headRotationY, Settings.Mirroring ? -m_faceData.m_headRotationZ : m_faceData.m_headRotationZ, m_faceData.m_headRotationW),
                    m_headTracker.transform.localRotation,
                    Settings.Blending
                );
            }
        }

        void OnUiManagerInit()
        {
            UIExpansionKit.API.ExpansionKitApi.GetExpandedMenu(UIExpansionKit.API.ExpandedMenu.QuickMenu).AddSimpleButton("VSF height reset", this.OnHeightReset);

            MelonLoader.MelonCoroutines.Start(CreateHeadTracker());
        }
        System.Collections.IEnumerator CreateHeadTracker()
        {
            while(Utils.GetVRCTrackingManager() == null)
                yield return null;
            while(Utils.GetVRCTrackingSteam() == null)
                yield return null;
            while(Utils.GetSteamVRControllerManager() == null)
                yield return null;

            m_headTracker = new GameObject("VSF_HeadTracker");
            m_headTracker.transform.parent = Utils.GetSteamVRControllerManager().transform;
            m_headTracker.transform.localPosition = Vector3.zero;
            m_headTracker.transform.localRotation = Quaternion.identity;
        }

        void OnPlayerJoined(VRC.Player p_player)
        {
            if(p_player == Utils.GetLocalPlayer()._player)
                OnRoomJoined();
        }

        void OnRoomJoined()
        {
            m_localTracked = Utils.GetLocalPlayer().gameObject.AddComponent<VsfTracked>();
        }

        void OnRoomLeft()
        {
            m_localTracked = null;
        }

        void OnHeightReset()
        {
            if(Settings.Enabled && (m_localTracked != null) && (m_headTracker != null))
            {
                m_headTracker.transform.parent = Utils.GetVRCTrackingManager().transform;
                m_headTracker.transform.position = m_localTracked.transform.position + m_localTracked.transform.rotation * new Vector3(0f, Utils.GetAvatarViewPoint().y - Utils.GetAvatarHeadToViewPoint().y * 1.22f, 0f);
                m_headTracker.transform.parent = Utils.GetSteamVRControllerManager().transform;
                m_headOffset = m_headTracker.transform.localPosition;
                m_headOffset.y -= m_faceData.m_headPositionY;
            }
        }

        static void IKSolverVR_VrcLateSolve_Prefix(ref RootMotion.FinalIK.IKSolverVR __instance) => ms_instance?.OnVrcLateIKSolve(__instance);
        void OnVrcLateIKSolve(RootMotion.FinalIK.IKSolverVR p_solver)
        {
            if(Settings.Enabled && (m_localTracked != null))
                m_localTracked.UpdateHeadTransform(p_solver, m_headTracker.transform);
        }
    }
}
