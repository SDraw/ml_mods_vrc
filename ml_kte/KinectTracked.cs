using System;
using UnityEngine;

namespace ml_kte
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    class KinectTracked : MonoBehaviour
    {
        VRCPlayer m_player = null;
        RootMotion.FinalIK.IKSolverVR m_solver = null;

        public KinectTracked(IntPtr ptr) : base(ptr) { }

        void Awake()
        {
            m_player = this.GetComponent<VRCPlayer>();
        }

        public void LateUpdateTransforms(Transform f_head, Transform f_hips, Transform f_leftLeg, Transform f_rightLeg, Transform f_leftHand, Transform f_rightHand)
        {
            if(m_solver != null)
            {
                if(m_solver.spine != null)
                {
                    m_solver.spine.headPosition = f_head.position;
                    m_solver.spine.headRotation = f_head.rotation; // Doesn't work?
                    m_solver.spine.IKPositionHead = f_head.position;
                    m_solver.spine.IKRotationHead = f_head.rotation; // Doesn't work?
                    if(m_solver.spine.headTarget != null)
                    {
                        m_solver.spine.headTarget.position = f_head.position;
                        m_solver.spine.headTarget.rotation = f_head.rotation; // Doesn't work?
                    }

                    m_solver.spine.pelvisPositionWeight = 1f;
                    m_solver.spine.pelvisRotationWeight = 1f;
                    m_solver.spine.IKPositionPelvis = f_hips.position;
                    m_solver.spine.IKRotationPelvis = f_hips.rotation;
                    if(m_solver.spine.pelvisTarget != null)
                    {
                        m_solver.spine.pelvisTarget.position = f_hips.position;
                        m_solver.spine.pelvisTarget.rotation = f_hips.rotation;
                    }
                }

                if(m_solver.leftLeg != null)
                {
                    m_solver.leftLeg.positionWeight = 1f;
                    //m_solver.leftLeg.rotationWeight = 1f;
                    m_solver.leftLeg.IKPosition = f_leftLeg.position;
                    //m_solver.leftLeg.IKRotation = f_leftLeg.rotation;
                }

                if(m_solver.rightLeg != null)
                {
                    m_solver.rightLeg.positionWeight = 1f;
                    //m_solver.rightLeg.rotationWeight = 1f;
                    m_solver.rightLeg.IKPosition = f_rightLeg.position;
                    //m_solver.rightLeg.IKRotation = f_rightLeg.rotation;
                }

                if((m_solver.leftArm != null) && (m_solver.leftArm.target != null))
                {
                    m_solver.leftArm.positionWeight = 1f;
                    //m_solver.leftArm.rotationWeight = 1f;
                    m_solver.leftArm.target.position = f_leftHand.position;
                    //m_solver.leftArm.target.rotation = f_leftHand.rotation;
                }

                if((m_solver.rightArm != null) && (m_solver.rightArm.target != null))
                {
                    m_solver.rightArm.positionWeight = 1f;
                    //m_solver.rightArm.rotationWeight = 1f;
                    m_solver.rightArm.target.position = f_rightHand.position;
                    //m_solver.rightArm.target.rotation = f_rightHand.rotation;
                }
            }
        }

        public void RecacheComponents()
        {
            if(m_player.field_Private_VRC_AnimationController_0.field_Private_VRIK_0 != null)
                m_solver = m_player.field_Private_VRC_AnimationController_0.field_Private_VRIK_0.solver;
            else
                m_solver = null;
        }
    }
}
