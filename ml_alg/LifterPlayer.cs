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
        HandGestureController m_gestureController = null;
        bool m_leftHandGrab = false;
        bool m_rightHandGrab = false;
        List<LiftedPlayer> m_liftedPlayers = null;

        public VRCPlayer Player
        {
            get => m_player;
        }

        public Animator Animator
        {
            get => m_animator;
        }

        public LifterPlayer(IntPtr ptr) : base(ptr)
        {
            m_liftedPlayers = new List<LiftedPlayer>();
        }

        void Awake()
        {
            m_player = this.GetComponent<VRCPlayer>();
            m_animator = m_player.field_Internal_Animator_0;
            m_gestureController = m_player.field_Private_VRC_AnimationController_0.field_Private_HandGestureController_0;

            m_player.field_Private_OnAvatarIsReady_0 += new System.Action(this.RecacheComponents);
        }

        void Update()
        {
            if(m_gestureController != null)
            {
                bool l_grabState = (m_gestureController.field_Private_Gesture_0 == HandGestureController.Gesture.Fist);
                if(m_leftHandGrab != l_grabState)
                {
                    m_leftHandGrab = l_grabState;
                    foreach(LiftedPlayer l_lifted in m_liftedPlayers)
                        l_lifted.OnLifterGesture(this, HumanBodyBones.LeftHand, m_leftHandGrab);
                }

                l_grabState = (m_gestureController.field_Private_Gesture_2 == HandGestureController.Gesture.Fist);
                if(m_rightHandGrab != l_grabState)
                {
                    m_rightHandGrab = l_grabState;
                    foreach(LiftedPlayer l_lifted in m_liftedPlayers)
                        l_lifted.OnLifterGesture(this, HumanBodyBones.RightHand, m_rightHandGrab);
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
            if(p_lifted != null)
                m_liftedPlayers.Remove(p_lifted);
        }

        void RecacheComponents()
        {
            m_animator = m_player.field_Internal_Animator_0;
            if((m_animator != null) && !m_animator.isHuman)
            {
                foreach(LiftedPlayer l_lifted in m_liftedPlayers)
                    l_lifted.UnassignRemoteLifter(this);
            }
        }
    }
}
