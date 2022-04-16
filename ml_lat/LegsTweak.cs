using System;
using UnityEngine;

namespace ml_lat
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    class LegsTweak : MonoBehaviour
    {
        static Vector4 ms_pointVector = new Vector4(0f, 0f, 0f, 1f);

        VRCPlayer m_player = null;
        readonly VRCPlayer.OnAvatarIsReady m_readyAvatarEvent = null;
        RootMotion.FinalIK.IKSolverVR m_solver = null;
        VRCVrIkController m_vrIkController = null;

        Vector2 m_kneesOffset = Vector2.zero;

        bool m_legsAnimation = false;
        bool m_legsAutostep = false;
        bool m_legsForwardKnees = false;

        public LegsTweak(IntPtr ptr) : base(ptr)
        {
            m_readyAvatarEvent = new Action(this.RecacheComponents);
        }

        void Awake()
        {
            m_player = this.GetComponent<VRCPlayer>();

            m_player.field_Private_OnAvatarIsReady_0 += m_readyAvatarEvent;
        }

        void OnDestroy()
        {
            if(m_player != null)
                m_player.field_Private_OnAvatarIsReady_0 -= m_readyAvatarEvent;
        }

        void Update()
        {
            if(m_legsAutostep && (m_vrIkController != null))
                m_vrIkController.field_Private_Single_7 = 0f; // _lowerBodyWeight ?

            if(m_legsForwardKnees && (m_vrIkController != null) && (m_solver != null))
            {
                if(m_solver.leftLeg != null)
                {
                    m_solver.leftLeg.bendGoalWeight = m_vrIkController.field_Private_Single_3; // _ikBlendGoal ?
                    m_solver.leftLeg.bendToTargetWeight = m_vrIkController.field_Private_Single_3;
                }

                if(m_solver.rightLeg != null)
                {
                    m_solver.rightLeg.bendGoalWeight = m_vrIkController.field_Private_Single_3;
                    m_solver.rightLeg.bendToTargetWeight = m_vrIkController.field_Private_Single_3;
                }
            }
        }

        void RecacheComponents()
        {
            m_solver = null;
            m_vrIkController = null;
            m_kneesOffset = Vector2.zero;

            if(m_player.field_Private_VRC_AnimationController_0.field_Private_VRIK_0 != null)
                m_solver = m_player.field_Private_VRC_AnimationController_0.field_Private_VRIK_0.solver;

            m_vrIkController = m_player.field_Private_VRC_AnimationController_0.field_Private_VRCVrIkController_0;
            if((m_vrIkController != null) && (m_player.field_Internal_Animator_0 != null) && (m_player.field_Internal_Animator_0.isHuman))
            {
                Transform l_leftLowerLeg = m_player.field_Internal_Animator_0.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
                if(l_leftLowerLeg != null)
                {
                    m_kneesOffset = (m_player.transform.GetMatrix().inverse * l_leftLowerLeg.GetMatrix()) * ms_pointVector;
                }
            }

            SetLegsAnimation(m_legsAnimation);
            SetLegsAutostep(m_legsAutostep);
            SetForwardKnees(m_legsForwardKnees);
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

        public void SetForwardKnees(bool p_state)
        {
            m_legsForwardKnees = p_state;

            if((m_solver != null) && (m_vrIkController != null))
            {
                if(m_legsForwardKnees)
                {
                    if(m_solver.leftLeg?.bendGoal == null)
                    {
                        m_vrIkController.field_Public_Transform_0.localPosition = new Vector3(m_kneesOffset.x, m_kneesOffset.y, 5f * m_vrIkController.field_Private_Single_0); // _avatarScale
                        m_solver.leftLeg.bendGoal = m_vrIkController.field_Public_Transform_0; // LeftKneeTarget
                        m_solver.leftLeg.bendGoalWeight = 1f;
                        m_solver.leftLeg.bendToTargetWeight = 1f;
                    }

                    if(m_solver.rightLeg?.bendGoal == null)
                    {
                        m_vrIkController.field_Public_Transform_1.localPosition = new Vector3(-m_kneesOffset.x, m_kneesOffset.y, 5f * m_vrIkController.field_Private_Single_0);
                        m_solver.rightLeg.bendGoal = m_vrIkController.field_Public_Transform_1; // RightKneeTarget
                        m_solver.rightLeg.bendGoalWeight = 1f;
                        m_solver.rightLeg.bendToTargetWeight = 1f;
                    }
                }
                else
                {
                    if(m_solver.leftLeg?.bendGoal == m_vrIkController.field_Public_Transform_0)
                    {
                        m_solver.leftLeg.bendGoal = null;
                        m_solver.leftLeg.bendGoalWeight = 0f;
                        m_solver.leftLeg.bendToTargetWeight = 0f;
                    }

                    if(m_solver.rightLeg?.bendGoal == m_vrIkController.field_Public_Transform_1)
                    {
                        m_solver.rightLeg.bendGoal = null;
                        m_solver.rightLeg.bendGoalWeight = 0f;
                        m_solver.rightLeg.bendToTargetWeight = 0f;
                    }
                }
            }
        }

        public void CheckPoseChange(VRCVrIkController p_vrIkController)
        {
            if((m_vrIkController != null) && (m_vrIkController == p_vrIkController) && m_legsForwardKnees)
                SetForwardKnees(m_legsForwardKnees);
        }
    }
}
