using System;
using System.Collections.Generic;
using UnityEngine;

namespace ml_alg
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    class LifterPlayer : MonoBehaviour
    {
        VRC.Player m_player = null;
        Animator m_animator = null;
        HandGestureController m_gestureController = null;
        bool m_leftHandGrab = false;
        bool m_rightHandGrab = false;
        List<LiftedPlayer> m_liftedPlayers = null;

        public LifterPlayer(IntPtr ptr) : base(ptr) { }

        public VRC.Player Player
        {
            get => m_player;
        }

        public Animator Animator
        {
            get => m_animator;
        }

        void Awake()
        {
            m_player = this.GetComponent<VRC.Player>();
            m_animator = m_player.prop_VRCPlayer_0.field_Internal_Animator_0;
            m_gestureController = m_player.prop_VRCPlayer_0.field_Private_VRC_AnimationController_0.field_Private_HandGestureController_0;
            m_liftedPlayers = new List<LiftedPlayer>();
        }

        void Update()
        {
            if((m_player != null) && (m_gestureController != null))
            {
                bool l_grabState = (m_gestureController.field_Private_Gesture_0 == HandGestureController.Gesture.Fist);
                if(m_leftHandGrab != l_grabState)
                {
                    m_leftHandGrab = l_grabState;
                    foreach(var l_lifted in m_liftedPlayers)
                        l_lifted.OnLifterGesture(this, HumanBodyBones.LeftHand, m_leftHandGrab);
                }

                l_grabState = (m_gestureController.field_Private_Gesture_2 == HandGestureController.Gesture.Fist);
                if(m_rightHandGrab != l_grabState)
                {
                    m_rightHandGrab = l_grabState;
                    foreach(var l_lifted in m_liftedPlayers)
                        l_lifted.OnLifterGesture(this, HumanBodyBones.RightHand, m_rightHandGrab);
                }
            }
        }

        public void AddLifted(LiftedPlayer f_lifted)
        {
            if((f_lifted != null) && !m_liftedPlayers.Contains(f_lifted))
                m_liftedPlayers.Add(f_lifted);
        }
        public void RemoveLifted(LiftedPlayer f_lifted)
        {
            if(f_lifted != null)
                m_liftedPlayers.Remove(f_lifted);
        }

        public void RecacheComponents()
        {
            m_animator = m_player.prop_VRCPlayer_0.field_Internal_Animator_0;
            if((m_animator != null) && !m_animator.isHuman)
            {
                foreach(var l_lifted in m_liftedPlayers)
                    l_lifted.UnassignRemoteLifter(this);
            }
        }
    }
}
