using System;
using System.Collections.Generic;
using UnityEngine;

namespace ml_abp
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    class InteractedPlayer : MonoBehaviour
    {
        static readonly string[] ms_parameterNames =
        {
            "_HeadProximity",
            "_HipsProximity",
            "_LeftHandProximity",
            "_RightHandProximity",
            "_LeftFootProximity",
            "_RightFootProximity",
            "_NeckProximity",
            "_LeftEyeProximity",
            "_RightEyeProximity",
            "_SpineProximity",
            "_ChestProximity",
            "_LeftShoulderProximity",
            "_RightShoulderProximity",
            "_LeftUpperArmProximity",
            "_RightUpperArmProximity",
            "_LeftLowerArmProximity",
            "_RightLowerArmProximity",
            "_LeftUpperLegProximity",
            "_RightUpperLegProximity",
            "_LeftLowerLegProximity",
            "_RightLowerLegProximity",
            "_LeftToesProximity",
            "_RightToesProximity",
            "_JawProximity"
        };
        static readonly HumanBodyBones[] ms_parameterBones =
        {
            HumanBodyBones.Head,
            HumanBodyBones.Hips,
            HumanBodyBones.LeftHand,
            HumanBodyBones.RightHand,
            HumanBodyBones.LeftFoot,
            HumanBodyBones.RightFoot,
            HumanBodyBones.Neck,
            HumanBodyBones.LeftEye,
            HumanBodyBones.RightEye,
            HumanBodyBones.Spine,
            HumanBodyBones.Chest,
            HumanBodyBones.LeftShoulder,
            HumanBodyBones.RightShoulder,
            HumanBodyBones.LeftUpperArm,
            HumanBodyBones.RightUpperArm,
            HumanBodyBones.LeftLowerArm,
            HumanBodyBones.RightLowerArm,
            HumanBodyBones.LeftUpperLeg,
            HumanBodyBones.RightUpperLeg,
            HumanBodyBones.LeftLowerLeg,
            HumanBodyBones.RightLowerLeg,
            HumanBodyBones.LeftToes,
            HumanBodyBones.RightToes,
            HumanBodyBones.Jaw
        };

        class CustomParameter
        {
            public int m_paramHash;
            public float m_distance;
            public HumanBodyBones m_bone;
            public Transform m_customTarget;
        }

        VRC.Player m_player = null;
        float m_boneProximity = 0.25f;
        float m_playersProximity = 5f;
        bool m_useCustomTargets = false;

        List<InteracterPlayer> m_interacterPlayers = null;
        List<CustomParameter> m_parameters = null;

        AvatarPlayableController m_playableController = null;

        public VRC.Player Player
        {
            get => m_player;
        }

        public float BonesProximity
        {
            set => m_boneProximity = value;
        }

        public float PlayersProximity
        {
            set => m_playersProximity = value;
        }

        public bool UseCustomTargets
        {
            set => m_useCustomTargets = value;
        }

        public InteractedPlayer(IntPtr ptr) : base(ptr)
        {
            m_interacterPlayers = new List<InteracterPlayer>();
            m_parameters = new List<CustomParameter>();
        }

        void Awake()
        {
            m_player = this.GetComponent<VRC.Player>();

            m_player.prop_VRCPlayer_0.field_Private_OnAvatarIsReady_0 += new System.Action(this.RebuildParameters);
        }

        void Update()
        {
            if(m_parameters.Count != 0)
            {
                // Reset values
                foreach(var l_param in m_parameters)
                    l_param.m_distance = float.MaxValue;

                if(m_interacterPlayers.Count != 0)
                {
                    foreach(var l_interacter in m_interacterPlayers)
                    {
                        float l_playerDistance = Vector3.Distance(m_player.transform.position, l_interacter.Player.transform.position);
                        if(l_playerDistance <= m_playersProximity)
                        {
                            var l_localAnimator = m_player.prop_VRCPlayer_0.field_Internal_Animator_0;
                            var l_remoteAnimator = l_interacter.Player.prop_VRCPlayer_0.field_Internal_Animator_0;
                            if((l_localAnimator != null) && (l_remoteAnimator != null))
                            {
                                var l_remoteLeftHandBone = l_remoteAnimator.GetBoneTransform(HumanBodyBones.LeftHand);
                                var l_remoteRightHandBone = l_remoteAnimator.GetBoneTransform(HumanBodyBones.RightHand);

                                foreach(var l_pair in m_parameters)
                                {
                                    var l_localTarget = ((l_pair.m_bone != HumanBodyBones.LastBone) ? l_localAnimator.GetBoneTransform(l_pair.m_bone) : l_pair.m_customTarget);
                                    if(l_localTarget != null)
                                    {
                                        float l_distance = float.MaxValue;
                                        if(l_remoteLeftHandBone != null)
                                            l_distance = Math.Min(l_distance, Vector3.Distance(l_localTarget.position, l_remoteLeftHandBone.position));
                                        if(l_remoteRightHandBone != null)
                                            l_distance = Math.Min(l_distance, Vector3.Distance(l_localTarget.position, l_remoteRightHandBone.position));
                                        if(l_distance <= m_boneProximity)
                                        {
                                            l_pair.m_distance = Math.Min(l_pair.m_distance, l_distance);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if(m_playableController != null)
                {
                    foreach(var l_param in m_parameters)
                    {
                        m_playableController.SetAvatarFloatParamEx(l_param.m_paramHash, (l_param.m_distance > m_boneProximity) ? 0f : Utils.QuadraticEaseOut(1f - (l_param.m_distance / m_boneProximity)));
                    }
                }
            }
        }

        public void AddInteracter(InteracterPlayer f_player)
        {
            if((f_player != null) && !m_interacterPlayers.Contains(f_player))
                m_interacterPlayers.Add(f_player);
        }

        public void RemoveInteracter(InteracterPlayer f_player)
        {
            if(f_player != null)
                m_interacterPlayers.Remove(f_player);
        }

        void RebuildParameters()
        {
            m_parameters.Clear();
            m_playableController = null;

            UnhollowerBaseLib.Il2CppArrayBase<Transform> l_avatarTransforms = (m_useCustomTargets ? m_player.prop_VRCPlayer_0.field_Internal_Animator_0.transform.GetComponentsInChildren<Transform>(true) : null);

            m_playableController = m_player.prop_VRCPlayer_0.field_Private_AnimatorControllerManager_0.field_Private_AvatarAnimParamController_0.field_Private_AvatarPlayableController_0;
            if(m_playableController != null)
            {
                foreach(var l_param in m_playableController.field_Private_ArrayOf_ObjectNPublicInObInPaInUnique_0)
                {
                    for(int i = 0; i < ms_parameterNames.Length; i++)
                    {
                        if((l_param.field_Public_AvatarParameter_0?.field_Private_String_0 == ms_parameterNames[i]) && (l_param.field_Public_AvatarParameter_0.field_Private_ParameterType_0 == VRC.Playables.AvatarParameter.ParameterType.Float))
                        {
                            m_parameters.Add(new CustomParameter
                            {
                                m_paramHash = l_param.field_Public_Int32_0,
                                m_distance = float.MaxValue,
                                m_bone = ms_parameterBones[i],
                                m_customTarget = null
                            });
                            break;
                        }
                    }

                    if(m_useCustomTargets && (l_param.field_Public_AvatarParameter_0 != null) && l_param.field_Public_AvatarParameter_0.field_Private_String_0.StartsWith("_ProximityTarget"))
                    {
                        foreach(var l_child in l_avatarTransforms)
                        {
                            if(l_child.name == l_param.field_Public_AvatarParameter_0.field_Private_String_0)
                            {
                                m_parameters.Add(new CustomParameter
                                {
                                    m_paramHash = l_param.field_Public_Int32_0,
                                    m_distance = float.MaxValue,
                                    m_bone = HumanBodyBones.LastBone,
                                    m_customTarget = l_child
                                });
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
