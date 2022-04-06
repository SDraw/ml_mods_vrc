using System;
using System.Collections.Generic;
using UnityEngine;

namespace ml_alg
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    class LifterPlayer : MonoBehaviour
    {
        VRCPlayer m_player = null;
        Animator m_animator = null;
        AnimatorCullingMode m_origCullingMode = AnimatorCullingMode.CullUpdateTransforms;
        HandGestureController m_gestureController = null;

        bool m_leftHandGrab = false;
        bool m_rightHandGrab = false;
        Transform m_leftHand = null;
        Transform m_rightHand = null;

        readonly List<LiftedPlayer> m_liftedPlayers = null;

        public LifterPlayer(IntPtr ptr) : base(ptr)
        {
            m_liftedPlayers = new List<LiftedPlayer>();
        }

        void Awake()
        {
            m_player = this.GetComponent<VRCPlayer>();
            m_animator = m_player.field_Internal_Animator_0;
            m_gestureController = m_player.field_Private_VRC_AnimationController_0.field_Private_HandGestureController_0;

            m_player.field_Private_OnAvatarIsReady_0 += new Action(this.RecacheComponents);
            RecacheComponents();
        }

        void OnDestroy()
        {
            foreach(LiftedPlayer l_lifted in m_liftedPlayers)
                l_lifted.UnassignRemoteLifter(this);

            if(m_animator != null)
                m_animator.cullingMode = m_origCullingMode;
        }

        void Update()
        {
            if(m_gestureController != null)
            {
                bool l_grabState = (m_gestureController.field_Private_Gesture_0 == HandGestureController.Gesture.Fist);
                if(m_leftHandGrab != l_grabState)
                {
                    m_leftHandGrab = l_grabState;
                    if(m_leftHand != null)
                    {
                        foreach(LiftedPlayer l_lifted in m_liftedPlayers)
                            l_lifted.OnLifterGesture(this, m_leftHand, m_leftHandGrab);
                    }
                }

                l_grabState = (m_gestureController.field_Private_Gesture_2 == HandGestureController.Gesture.Fist);
                if(m_rightHandGrab != l_grabState)
                {
                    m_rightHandGrab = l_grabState;
                    if(m_rightHand != null)
                    {
                        foreach(LiftedPlayer l_lifted in m_liftedPlayers)
                            l_lifted.OnLifterGesture(this, m_rightHand, m_rightHandGrab);
                    }
                }
            }
        }

        public void AddLifted(LiftedPlayer p_lifted)
        {
            if((p_lifted != null) && !m_liftedPlayers.Contains(p_lifted))
                m_liftedPlayers.Add(p_lifted);
        }
        public void RemoveLifted(LiftedPlayer p_lifted)
        {
            if((p_lifted != null) && m_liftedPlayers.Contains(p_lifted))
            {
                p_lifted.UnassignRemoteLifter(this);
                m_liftedPlayers.Remove(p_lifted);
            }
        }

        void RecacheComponents()
        {
            foreach(LiftedPlayer l_lifted in m_liftedPlayers)
                l_lifted.UnassignRemoteLifter(this);

            m_origCullingMode = AnimatorCullingMode.CullUpdateTransforms;
            m_leftHand = null;
            m_rightHand = null;

            m_animator = m_player.field_Internal_Animator_0;
            if(m_animator != null)
            {
                m_leftHand = m_animator.GetBoneTransform(HumanBodyBones.LeftHand);
                m_rightHand = m_animator.GetBoneTransform(HumanBodyBones.RightHand);

                m_origCullingMode = m_animator.cullingMode;
                m_animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            }
        }
    }
}
