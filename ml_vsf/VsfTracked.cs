using System;
using UnityEngine;

namespace ml_vsf
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    class VsfTracked : MonoBehaviour
    {
        VRCPlayer m_player = null;
        readonly VRCPlayer.OnAvatarIsReady m_readyAvatarEvent = null;
        RootMotion.FinalIK.IKSolverVR m_solver = null;

        Vector3 m_headToView;
        Quaternion m_headToViewRotation;

        public VsfTracked(IntPtr ptr) : base(ptr)
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

        // LateUpdate only
        public void UpdateHeadTransform(RootMotion.FinalIK.IKSolverVR p_solver, Transform p_transform)
        {
            if((m_solver != null) && (m_solver.Pointer == p_solver.Pointer))
            {
                if(m_solver.spine != null)
                {
                    m_solver.spine.headPosition = p_transform.position;
                    m_solver.spine.IKPositionHead = p_transform.position;
                    m_solver.spine.headRotation = p_transform.rotation;
                    m_solver.spine.IKRotationHead = p_transform.rotation;

                    if(m_solver.spine.headTarget != null)
                    {
                        m_solver.spine.headTarget.parent.rotation = p_transform.rotation;
                        m_solver.spine.headTarget.parent.position = p_transform.position;
                        m_solver.spine.headTarget.position = p_transform.position;
                        m_solver.spine.headTarget.rotation = p_transform.rotation;
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
