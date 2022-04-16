using System;
using System.Collections.Generic;
using UnityEngine;

namespace ml_abp
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    class InteractedPlayer : MonoBehaviour
    {
        class CustomParameter
        {
            public int m_paramHash;
            public float m_distance;
            public HumanBodyBones m_bone;
            public Transform m_target;
        }

        VRCPlayer m_player = null;
        readonly VRCPlayer.OnAvatarIsReady m_avatarReadyEvent = null;
        AvatarPlayableController m_playableController = null;
        Animator m_animator = null;

        readonly List<InteracterPlayer> m_interacterPlayers = null;
        readonly List<CustomParameter> m_parameters = null;
        float m_boneProximity = 0.25f;
        float m_playersProximity = 5f;
        bool m_useCustomTargets = false;

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
            m_avatarReadyEvent = new Action(this.RebuildParameters);
        }

        void Awake()
        {
            m_player = this.GetComponent<VRCPlayer>();
            m_animator = m_player.field_Internal_Animator_0;

            m_player.field_Private_OnAvatarIsReady_0 += m_avatarReadyEvent;
        }

        void OnDestroy()
        {
            if(m_player != null)
                m_player.field_Private_OnAvatarIsReady_0 -= m_avatarReadyEvent;
        }

        void Update()
        {
            if(m_parameters.Count != 0)
            {
                // Reset values
                foreach(CustomParameter l_param in m_parameters)
                    l_param.m_distance = float.MaxValue;

                if(m_interacterPlayers.Count != 0)
                {
                    foreach(InteracterPlayer l_interacter in m_interacterPlayers)
                    {
                        if(Vector3.Distance(this.transform.position, l_interacter.transform.position) <= m_playersProximity)
                        {
                            List<Transform> l_remoteTargets = l_interacter.GetProximityTargets();
                            if((l_remoteTargets != null) && (l_remoteTargets.Count > 0))
                            {
                                foreach(CustomParameter l_param in m_parameters)
                                {
                                    if(l_param.m_target != null)
                                    {
                                        float l_distance = float.MaxValue;
                                        foreach(Transform l_remoteTarget in l_remoteTargets)
                                        {
                                            if(l_remoteTarget != null)
                                                l_distance = Math.Min(l_distance, Vector3.Distance(l_param.m_target.position, l_remoteTarget.position));
                                        }
                                        if(l_distance <= m_boneProximity)
                                        {
                                            l_param.m_distance = Math.Min(l_param.m_distance, l_distance);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if(m_playableController != null)
                {
                    foreach(CustomParameter l_param in m_parameters)
                    {
                        m_playableController.SetAvatarFloatParamEx(l_param.m_paramHash, (l_param.m_distance > m_boneProximity) ? 0f : Utils.QuadraticEaseOut(1f - (l_param.m_distance / m_boneProximity)));
                    }
                }
            }
        }

        public void AddInteracter(InteracterPlayer p_player)
        {
            if((p_player != null) && !m_interacterPlayers.Contains(p_player))
                m_interacterPlayers.Add(p_player);
        }

        public void RemoveInteracter(InteracterPlayer p_player)
        {
            if(p_player != null)
                m_interacterPlayers.Remove(p_player);
        }

        void RebuildParameters()
        {
            m_parameters.Clear();
            m_playableController = m_player.field_Private_AnimatorControllerManager_0.field_Private_AvatarAnimParamController_0.field_Private_AvatarPlayableController_0;
            m_animator = m_player.field_Internal_Animator_0;

            if(m_playableController != null)
            {
                var l_transforms = ((m_useCustomTargets && (m_animator != null)) ? m_animator.transform.GetComponentsInChildren<Transform>(true) : null);

                foreach(var l_param in m_playableController.field_Private_ArrayOf_ObjectNPublicInObInPaInUnique_0)
                {
                    bool l_skip = false;
                    for(int i = 0; i < (int)HumanBodyBones.UpperChest; i++)
                    {
                        if((l_param.field_Public_AvatarParameter_0?.field_Private_String_0 == string.Format("_{0}Proximity", (HumanBodyBones)i)) && (l_param.field_Public_AvatarParameter_0.field_Private_ParameterType_0 == VRC.Playables.AvatarParameter.ParameterType.Float))
                        {
                            m_parameters.Add(new CustomParameter
                            {
                                m_paramHash = l_param.field_Public_Int32_0,
                                m_distance = float.MaxValue,
                                m_bone = (HumanBodyBones)i,
                                m_target = ((m_animator != null) ? m_animator.GetBoneTransform((HumanBodyBones)i) : null)
                            });
                            l_skip = true;
                            break;
                        }
                    }
                    if(l_skip)
                        continue;

                    if(m_useCustomTargets && (l_transforms != null) && (l_param.field_Public_AvatarParameter_0 != null) && l_param.field_Public_AvatarParameter_0.field_Private_String_0.StartsWith("_ProximityTarget"))
                    {
                        foreach(Transform l_child in l_transforms)
                        {
                            if(l_child.name == l_param.field_Public_AvatarParameter_0.field_Private_String_0)
                            {
                                m_parameters.Add(new CustomParameter
                                {
                                    m_paramHash = l_param.field_Public_Int32_0,
                                    m_distance = float.MaxValue,
                                    m_bone = HumanBodyBones.LastBone,
                                    m_target = l_child
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
