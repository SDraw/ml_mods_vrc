using System;
using UnityEngine;

namespace ml_arh
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    class HeightAdjuster : MonoBehaviour
    {
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
        RootMotion.FinalIK.FullBodyBipedIK m_fbtIK = null;

        bool m_enabled = true;
        bool m_poseHeight = true;
        float m_height = 1.65f;
        float m_center = 0.85f;
        float m_radius = 0.2f;
        PoseState m_poseState = PoseState.Standing;

        public bool Enabled
        {
            set => m_enabled = value;
        }
        public bool PoseHeight
        {
            set => m_poseHeight = value;
        }

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
            if(m_poseHeight && (m_ikController != null) && (m_fbtIK != null))
            {
                PoseState l_currentState = PoseState.Standing;
                if(m_fbtIK.enabled)
                {
                    float l_upright = Mathf.Clamp(Utils.GetUprightAmount(), 0f, 1f);
                    if(l_upright <= 0.6f)
                        l_currentState = ((l_upright <= 0.35f) ? PoseState.Crawling : PoseState.Crouching);
                }
                else
                    l_currentState = (m_ikController.field_Private_Boolean_15 ? PoseState.Crouching : (m_ikController.field_Private_Boolean_14 ? PoseState.Crawling : PoseState.Standing));

                if(m_poseState != l_currentState)
                {
                    m_poseState = l_currentState;
                    ChangeHeight(m_height * ms_heightMultipliers[(int)m_poseState], m_center * ms_heightMultipliers[(int)m_poseState], false);
                }
            }
        }

        void RecacheComponents()
        {
            m_poseState = PoseState.Standing;

            m_ikController = null;
            m_ikController = this.GetComponent<VRCPlayer>().field_Private_VRC_AnimationController_0.field_Private_VRCVrIkController_0;
            m_fbtIK = this.GetComponent<VRCPlayer>().field_Private_VRC_AnimationController_0.field_Private_FullBodyBipedIK_0;

            if(m_enabled)
                UpdateHeight(Utils.GetTrackingHeight());
        }

        public void UpdateHeight(float p_height)
        {
            m_height = p_height;
            m_center = m_height * 0.515151f;
            m_radius = m_height * 0.121212f;

            ChangeHeight(m_height, m_center);
        }

        void ChangeHeight(float p_height, float p_center, bool p_radiusUpdate = true)
        {
            if(m_characterController != null)
            {
                m_characterController.height = p_height;

                Vector3 l_center = m_characterController.center;
                l_center.y = p_center;
                m_characterController.center = l_center;

                if(p_radiusUpdate)
                    m_characterController.radius = m_radius;
            }
        }
    }
}
