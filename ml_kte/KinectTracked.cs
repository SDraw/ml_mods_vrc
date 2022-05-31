using System;
using UnityEngine;

namespace ml_kte
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    class KinectTracked : MonoBehaviour
    {
        VRCPlayer m_player = null;
        readonly VRCPlayer.OnAvatarIsReady m_readyAvatarEvent = null;
        RootMotion.FinalIK.IKSolverVR m_solver = null;

        bool m_trackHead = true;
        bool m_trackHips = true;
        bool m_trackLegs = true;
        bool m_trackHands = true;
        bool m_rotateHead = true;
        bool m_rotateHips = true;
        bool m_rotateLegs = true;
        bool m_rotateHands = true;

        Vector3 m_headToView;
        Quaternion m_headToViewRotation;

        public bool TrackHead
        {
            set => m_trackHead = value;
        }
        public bool TrackHips
        {
            set => m_trackHips = value;
        }
        public bool TrackLegs
        {
            set => m_trackLegs = value;
        }
        public bool TrackHands
        {
            set => m_trackHands = value;
        }

        public bool RotateHead
        {
            set => m_rotateHead = value;
        }
        public bool RotateHips
        {
            set => m_rotateHips = value;
        }
        public bool RotateLegs
        {
            set => m_rotateLegs = value;
        }
        public bool RotateHands
        {
            set => m_rotateHands = value;
        }

        public KinectTracked(IntPtr ptr) : base(ptr)
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

        public void LateUpdateTransforms(RootMotion.FinalIK.IKSolverVR p_solver, Transform p_head, Transform p_hips, Transform p_leftLeg, Transform p_rightLeg, Transform p_leftHand, Transform p_rightHand)
        {
            if((m_solver != null) && (m_solver.Pointer == p_solver.Pointer))
            {
                if(m_solver.spine != null)
                {
                    if(m_trackHead)
                    {
                        m_solver.spine.headPosition = p_head.position;
                        m_solver.spine.IKPositionHead = p_head.position;
                        if(m_rotateHead)
                        {
                            m_solver.spine.headRotation = p_head.rotation;
                            m_solver.spine.IKRotationHead = p_head.rotation;
                        }
                        if(m_solver.spine.headTarget != null)
                        {
                            m_solver.spine.headTarget.parent.position = p_head.position;
                            m_solver.spine.headTarget.position = p_head.position;
                            if(m_rotateHead)
                            {
                                m_solver.spine.headTarget.parent.rotation = p_head.rotation;
                                m_solver.spine.headTarget.rotation = p_head.rotation;
                            }
                        }
                    }

                    if(m_trackHips)
                    {
                        m_solver.spine.pelvisPositionWeight = 1f;
                        m_solver.spine.IKPositionPelvis = p_hips.position;
                        if(m_rotateHips)
                        {
                            m_solver.spine.IKRotationPelvis = p_hips.rotation;
                            m_solver.spine.pelvisRotationWeight = 1f;
                        }
                        if(m_solver.spine.pelvisTarget != null)
                        {
                            m_solver.spine.pelvisTarget.position = p_hips.position;
                            if(m_rotateHips)
                                m_solver.spine.pelvisTarget.rotation = p_hips.rotation;
                        }
                    }
                }

                if((m_solver.leftLeg != null) && m_trackLegs)
                {
                    m_solver.leftLeg.positionWeight = 1f;
                    m_solver.leftLeg.IKPosition = p_leftLeg.position;
                    if(m_rotateLegs)
                    {
                        m_solver.leftLeg.rotationWeight = 1f;
                        m_solver.leftLeg.IKRotation = p_leftLeg.rotation;
                    }
                }

                if((m_solver.rightLeg != null) && m_trackLegs)
                {
                    m_solver.rightLeg.positionWeight = 1f;
                    m_solver.rightLeg.IKPosition = p_rightLeg.position;
                    if(m_rotateLegs)
                    {
                        m_solver.rightLeg.rotationWeight = 1f;
                        m_solver.rightLeg.IKRotation = p_rightLeg.rotation;
                    }
                }

                if((m_solver.leftArm != null) && (m_solver.leftArm.target != null) && m_trackHands)
                {
                    m_solver.leftArm.positionWeight = 1f;
                    m_solver.leftArm.target.position = p_leftHand.position;
                    if(m_rotateHands)
                    {
                        m_solver.leftArm.target.rotation = p_leftHand.rotation;
                        m_solver.leftArm.rotationWeight = 1f;
                    }
                }

                if((m_solver.rightArm != null) && (m_solver.rightArm.target != null) && m_trackHands)
                {
                    m_solver.rightArm.positionWeight = 1f;
                    m_solver.rightArm.target.position = p_rightHand.position;
                    if(m_rotateHands)
                    {
                        m_solver.rightArm.rotationWeight = 1f;
                        m_solver.rightArm.target.rotation = p_rightHand.rotation;
                    }
                }
            }
        }

        void RecacheComponents()
        {
            m_solver = null;

            if(m_player.field_Private_VRC_AnimationController_0.field_Private_VRIK_0 != null)
                m_solver = m_player.field_Private_VRC_AnimationController_0.field_Private_VRIK_0.solver;

            if(m_solver?.spine?.headTarget != null)
            {
                m_headToView = m_solver.spine.headTarget.localPosition;
                m_headToViewRotation = m_solver.spine.headTarget.localRotation;
            }
        }

        public void ResetViewPoint()
        {
            if(m_solver?.spine?.headTarget != null)
            {
                m_solver.spine.headTarget.localPosition = m_headToView;
                m_solver.spine.headTarget.localRotation = m_headToViewRotation;
            }
        }
    }
}
