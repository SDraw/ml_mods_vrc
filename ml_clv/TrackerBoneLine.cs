using System;
using UnityEngine;

namespace ml_clv
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    class TrackerBoneLine : MonoBehaviour
    {
        public static readonly string[] ms_trackerTypes =
        {
            "waist", "left_foot", "right_foot",
            "left_elbow", "right_elbow",
            "left_knee", "right_knee",
            "chest"
        };
        public static readonly HumanBodyBones[] ms_linkedBones =
        {
            HumanBodyBones.Hips, HumanBodyBones.LeftFoot, HumanBodyBones.RightFoot,
            HumanBodyBones.LeftLowerArm, HumanBodyBones.RightLowerArm,
            HumanBodyBones.LeftLowerLeg, HumanBodyBones.RightLowerLeg,
            HumanBodyBones.Chest,
        };

        static Material ms_lineMaterial = null;
        static SteamVR_ControllerManager ms_controllerManager = null;

        int m_index = -1;
        VRCPlayer m_player = null;
        LineRenderer m_lineRenderer = null;

        public int Index
        {
            set => m_index = value;
        }
        public VRCPlayer Player
        {
            set => m_player = value;
        }

        public static SteamVR_ControllerManager ControllerManager
        {
            set => ms_controllerManager = value;
        }

        public TrackerBoneLine(IntPtr ptr) : base(ptr) { }

        void Awake()
        {
            m_lineRenderer = this.GetComponent<LineRenderer>();
            m_lineRenderer.material = GetLineMaterial();
            m_lineRenderer.alignment = LineAlignment.View;
            m_lineRenderer.allowOcclusionWhenDynamic = false;
            m_lineRenderer.castShadows = false;
            m_lineRenderer.widthMultiplier = 0.025f;
            m_lineRenderer.positionCount = 2;
            for(int j = 0; j < m_lineRenderer.positionCount; j++)
                m_lineRenderer.SetPosition(j, Vector3.zeroVector);
        }

        void Update()
        {
            if((ms_controllerManager != null) && (m_index != -1) && (m_player != null))
            {
                Vector3 l_start;
                Animator l_animator = m_player.field_Internal_Animator_0;
                if((l_animator != null) && l_animator.isHuman)
                {
                    Transform l_bone = l_animator.GetBoneTransform(FindAssignedBone(l_animator));
                    l_start = (l_bone != null) ? l_bone.position : this.transform.position;
                }
                else 
                    l_start = this.transform.position;

                if(m_lineRenderer != null)
                {
                    m_lineRenderer.SetPosition(0, l_start);
                    m_lineRenderer.SetPosition(1, this.transform.position);
                    m_lineRenderer.widthMultiplier = 0.025f * Utils.GetVRCTrackingManager().transform.localScale.x;
                }
            }
        }

        HumanBodyBones FindAssignedBone(Animator p_animator)
        {
            HumanBodyBones l_result = HumanBodyBones.LastBone;

            var l_stringBuilder = new Il2CppSystem.Text.StringBuilder(64);
            Valve.VR.ETrackedPropertyError l_error = Valve.VR.ETrackedPropertyError.TrackedProp_NotYetAvailable;
            Valve.VR.OpenVR.System?.GetStringTrackedDeviceProperty(ms_controllerManager.field_Private_ArrayOf_UInt32_0[m_index], Valve.VR.ETrackedDeviceProperty.Prop_ControllerType_String, l_stringBuilder, (uint)l_stringBuilder.Capacity, ref l_error);
            if(l_error == Valve.VR.ETrackedPropertyError.TrackedProp_Success)
            {
                string l_controllerType = l_stringBuilder.ToString();
                int l_controllerTypeId = -1;
                for(int i = 0; i < ms_trackerTypes.Length; i++)
                {
                    if(l_controllerType.Contains(ms_trackerTypes[i]))
                    {
                        l_controllerTypeId = i;
                        break;
                    }
                }

                if(l_controllerTypeId != -1)
                    l_result = ms_linkedBones[l_controllerTypeId];
            }

            // Tracker is unassigned in SteamVR or has no right property 
            if(l_result == HumanBodyBones.LastBone)
            {
                // Find nearest bone
                float l_distance = float.MaxValue;
                foreach(HumanBodyBones l_bone in ms_linkedBones)
                {
                    Transform l_boneTransform = p_animator.GetBoneTransform(l_bone);
                    if(l_boneTransform != null)
                    {
                        float l_distanceToPuck = Vector3.Distance(l_boneTransform.position, this.transform.position);
                        if(l_distanceToPuck < l_distance)
                        {
                            l_distance = l_distanceToPuck;
                            l_result = l_bone;
                        }
                    }
                }
            }

            // No bone, revert to hips
            if(l_result == HumanBodyBones.LastBone)
                l_result = HumanBodyBones.Hips;
            return l_result;
        }

        public static Material GetLineMaterial()
        {
            if(ms_lineMaterial == null)
            {
                // Thank Requi for this code
                ms_lineMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
                ms_lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                ms_lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                ms_lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                ms_lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                ms_lineMaterial.SetInt("_ZWrite", 0);
                ms_lineMaterial.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
            }
            return ms_lineMaterial;
        }
    }
}
