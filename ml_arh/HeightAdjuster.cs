using System;
using UnityEngine;

namespace ml_ahr
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

        CharacterController m_characterController = null;
        VRCVrIkController m_ikController = null;

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

        public HeightAdjuster(IntPtr ptr) : base(ptr) { }

        void Awake()
        {
            m_characterController = this.GetComponent<CharacterController>();

            this.GetComponent<VRCPlayer>().field_Private_OnAvatarIsReady_0 += new System.Action(this.RecacheComponents);
        }

        void Update()
        {
            if(m_poseHeight && (m_ikController != null))
            {
                PoseState l_currentState = (m_ikController.field_Private_Boolean_15 ? PoseState.Crouching : (m_ikController.field_Private_Boolean_14 ? PoseState.Crawling : PoseState.Standing));
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

            if(m_enabled)
                UpdateHeight(Utils.GetTrackingHeight());
        }

        public void UpdateHeight(float f_height)
        {
            m_height = f_height;
            m_center = m_height * 0.515151f;
            m_radius = m_height * 0.121212f;

            ChangeHeight(m_height, m_center);
        }

        void ChangeHeight(float f_height, float f_center, bool f_updateRadius = true)
        {
            if(m_characterController != null)
            {
                m_characterController.height = f_height;

                var l_center = m_characterController.center;
                l_center.y = f_center;
                m_characterController.center = l_center;

                if(f_updateRadius)
                    m_characterController.radius = m_radius;
            }
        }
    }
}
