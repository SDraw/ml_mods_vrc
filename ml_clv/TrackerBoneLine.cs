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
        static VRCPlayer ms_player = null;

        LineRenderer m_lineRenderer = null;
        int m_index = -1;

        public static SteamVR_ControllerManager ControllerManager
        {
            set => ms_controllerManager = value;
        }

        public static Material Material
        {
            get => ms_lineMaterial;
            set => ms_lineMaterial = value;
        }

        public static VRCPlayer Player
        {
            set => ms_player = value;
        }

        public int Index
        {
            set => m_index = value;
        }

        public TrackerBoneLine(IntPtr ptr) : base(ptr) { }

        void Awake()
        {
            if(m_lineRenderer == null)
            {
                m_lineRenderer = this.gameObject.AddComponent<LineRenderer>();
                m_lineRenderer.material = ms_lineMaterial;
                m_lineRenderer.alignment = LineAlignment.View;
                m_lineRenderer.allowOcclusionWhenDynamic = false;
                m_lineRenderer.castShadows = false;
                m_lineRenderer.widthMultiplier = 0.025f;
                m_lineRenderer.positionCount = 2;
                for(int j = 0; j < m_lineRenderer.positionCount; j++)
                    m_lineRenderer.SetPosition(j, Vector3.zero);
            }
        }

        void Update()
        {
            if((m_index != -1) && (ms_player != null))
            {
                Vector3 l_start;
                Animator l_animator = ms_player.field_Internal_Animator_0;
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
                    m_lineRenderer.widthMultiplier = 0.025f * Utils.GetAvatarScale(ms_player);
                }
            }
        }

        HumanBodyBones FindAssignedBone(Animator p_animator)
        {
            HumanBodyBones l_result = HumanBodyBones.LastBone;

            if(ms_controllerManager != null)
            {
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
            }

            // No bone, revert to hips
            if(l_result == HumanBodyBones.LastBone)
                l_result = HumanBodyBones.Hips;

            return l_result;
        }
    }
}
