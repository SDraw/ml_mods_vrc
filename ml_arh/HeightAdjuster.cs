using System;
using UnityEngine;

namespace ml_arh
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    class HeightAdjuster : MonoBehaviour
    {
        const float ms_defaultColliderHeight = 1.65f;
        const float ms_defaultColliderCenter = 0.85f;
        const float ms_defaultColliderRadius = 0.2f;

        enum PoseState
        {
            Standing = 0,
            Crouching,
            Crawling
        };

        static readonly float[] ms_heightMultipliers = { 1f, 0.6f, 0.35f };

        VRCPlayer m_player = null;
        readonly VRCPlayer.OnAvatarIsReady m_avatarReadyEvent = null;
        CharacterController m_characterController = null;
        VRCVrIkController m_ikController = null;

        bool m_enabled = true;
        bool m_poseHeight = true;

        float m_avatarEyeHeight = ms_defaultColliderHeight;
        float m_center = ms_defaultColliderCenter;
        float m_radius = ms_defaultColliderRadius;
        PoseState m_poseState = PoseState.Standing;

        public HeightAdjuster(IntPtr ptr) : base(ptr)
        {
            m_avatarReadyEvent = new Action(this.RecacheComponents);
        }

        void Awake()
        {
            m_player = this.GetComponent<VRCPlayer>();
            m_characterController = this.GetComponent<CharacterController>();

            m_player.field_Private_OnAvatarIsReady_0 += m_avatarReadyEvent;
        }

        void OnDestroy()
        {
            if(m_player != null)
                m_player.field_Private_OnAvatarIsReady_0 -= m_avatarReadyEvent;
        }

        void Update()
        {
            if(m_enabled && m_poseHeight)
            {
                PoseState l_newPoseState = PoseState.Standing;

                // Generic avatars are skipped
                if(m_ikController != null)
                    l_newPoseState = (m_ikController.field_Private_Boolean_31 ? PoseState.Crawling : (m_ikController.field_Private_Boolean_32 ? PoseState.Crouching : PoseState.Standing));

                if(m_poseState != l_newPoseState)
                {
                    m_poseState = l_newPoseState;
                    UpdateCollider(m_avatarEyeHeight * ms_heightMultipliers[(int)m_poseState], m_center * ms_heightMultipliers[(int)m_poseState], m_radius, false);
                }
            }
        }

        void RecacheComponents()
        {
            m_ikController = null;

            m_ikController = m_player.field_Private_VRC_AnimationController_0.field_Private_VRCVrIkController_0;

            m_avatarEyeHeight = m_player.field_Private_VRCAvatarManager_0.field_Private_Single_4;
            m_center = m_avatarEyeHeight * 0.515151f;
            m_radius = m_avatarEyeHeight * 0.121212f;
            m_poseState = PoseState.Standing;

            if(m_enabled)
                UpdateCollider(m_avatarEyeHeight, m_center, m_radius);
        }

        void UpdateCollider(float p_height, float p_center, float p_radius, bool p_radiusUpdate = true)
        {
            if(m_characterController != null)
            {
                m_characterController.height = p_height;

                Vector3 l_center = m_characterController.center;
                l_center.y = p_center;
                m_characterController.center = l_center;

                if(p_radiusUpdate)
                    m_characterController.radius = p_radius;
            }
        }

        public void SetEnabled(bool p_state)
        {
            if(m_enabled != p_state)
            {
                m_enabled = p_state;
                m_poseState = PoseState.Standing;

                UpdateCollider(
                    m_enabled ? m_avatarEyeHeight : ms_defaultColliderHeight,
                    m_enabled ? m_center : ms_defaultColliderCenter,
                    m_enabled ? m_radius : ms_defaultColliderRadius
                );
            }
        }

        public void SetPoseHeight(bool p_state)
        {
            if(m_poseHeight != p_state)
            {
                m_poseHeight = p_state;
                m_poseState = PoseState.Standing;

                if(m_enabled && !m_poseHeight)
                    UpdateCollider(m_avatarEyeHeight, m_center, m_radius);
            }
        }
    }
}
