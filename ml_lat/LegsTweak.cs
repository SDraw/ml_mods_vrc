using System;
using UnityEngine;

namespace ml_lat
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    class LegsTweak : MonoBehaviour
    {
        VRCPlayer m_player = null;
        RootMotion.FinalIK.IKSolverVR m_solver = null;
        VRCVrIkController m_vrIkController = null;

        bool m_legsAnimation = false;
        bool m_legsAutostep = false;

        public LegsTweak(IntPtr ptr) : base(ptr) { }

        void Awake()
        {
            m_player = this.GetComponent<VRCPlayer>();

            m_player.field_Private_OnAvatarIsReady_0 += new Action(this.RecacheComponents);
        }

        void Update()
        {
            if(m_legsAutostep && (m_vrIkController != null))
                m_vrIkController.field_Private_Single_7 = 0f;
        }

        void RecacheComponents()
        {
            m_solver = null;
            m_vrIkController = null;

            if(m_player.field_Private_VRC_AnimationController_0.field_Private_VRIK_0 != null)
                m_solver = m_player.field_Private_VRC_AnimationController_0.field_Private_VRIK_0.solver;
            m_vrIkController = m_player.field_Private_VRC_AnimationController_0.field_Private_VRCVrIkController_0;

            SetLegsAnimation(m_legsAnimation);
            SetLegsAutostep(m_legsAutostep);
        }

        public void SetLegsAnimation(bool p_state)
        {
            m_legsAnimation = p_state;

            if(m_solver != null)
            {
                if(m_solver.leftLeg != null)
                    m_solver.leftLeg.useAnimatedBendNormal = !m_legsAnimation;

                if(m_solver.rightLeg != null)
                    m_solver.rightLeg.useAnimatedBendNormal = !m_legsAnimation;
            }
        }

        public void SetLegsAutostep(bool p_state) => m_legsAutostep = p_state;
    }
}
