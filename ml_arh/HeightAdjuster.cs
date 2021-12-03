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

        bool m_poseHeight = true;
        float m_height = 1.65f;
        float m_center = 0.85f;
        PoseState m_poseState = PoseState.Standing;

        public bool PoseHeight
        {
            set => m_poseHeight = value;
        }

        public HeightAdjuster(IntPtr ptr) : base(ptr) { }

        void Awake()
        {
            m_characterController = this.GetComponent<CharacterController>();
        }

        void Update()
        {
            if(m_poseHeight && (m_ikController != null))
            {
                PoseState l_currentState = (m_ikController.field_Private_Boolean_15 ? PoseState.Crouching : (m_ikController.field_Private_Boolean_14 ? PoseState.Crawling : PoseState.Standing));
                if(m_poseState != l_currentState)
                {
                    m_poseState = l_currentState;
                    ChangeHeight(m_height * ms_heightMultipliers[(int)m_poseState], m_center * ms_heightMultipliers[(int)m_poseState]);
                }
            }
        }

        public void RecacheComponents()
        {
            m_poseState = PoseState.Standing;

            m_ikController = null;
            m_ikController = this.GetComponent<VRCPlayer>().field_Private_VRC_AnimationController_0.field_Private_VRCVrIkController_0;
        }

        public void UpdateHeights(float f_height, float f_center)
        {
            m_height = f_height;
            m_center = f_center;

            ChangeHeight(m_height, m_center);
        }

        void ChangeHeight(float f_height, float f_center)
        {
            if(m_characterController != null)
            {
                m_characterController.height = f_height;

                var l_center = m_characterController.center;
                l_center.y = f_center;
                m_characterController.center = l_center;
            }
        }
    }
}
