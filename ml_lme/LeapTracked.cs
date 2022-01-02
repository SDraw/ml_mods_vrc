using System;
using System.Collections.Generic;
using UnityEngine;

namespace ml_lme
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    class LeapTracked : MonoBehaviour
    {
        static readonly Quaternion[] ms_fixRotations =
        {
            Quaternion.Euler(0f,0f,-90f),
            Quaternion.Euler(0f,0f,90f),
        };

        enum TrackingMode
        {
            Generic = 0,
            FBT,
            IKTweaks
        }

        VRCPlayer m_player = null;
        HandGestureController m_handGestureController = null;
        RootMotion.FinalIK.IKSolverVR m_solver = null;
        RootMotion.FinalIK.FullBodyBipedIK m_fbtIK = null;
        MonoBehaviour m_iktFbtIK = null;
        TrackingMode m_trackigMode = TrackingMode.Generic;
        Animator m_animator = null;

        bool m_fingersOnly = false;

        public bool FingersOnly
        {
            set => m_fingersOnly = value;
        }

        public LeapTracked(IntPtr ptr) : base(ptr) { }

        void Awake()
        {
            m_player = this.GetComponent<VRCPlayer>();
            m_animator = m_player.field_Internal_Animator_0;

            m_player.field_Private_OnAvatarIsReady_0 += new System.Action(this.RecacheComponents);
        }

        void Update()
        {
            m_trackigMode = TrackingMode.Generic;
            if((m_fbtIK != null) && m_fbtIK.enabled)
            {
                if((m_iktFbtIK != null) && m_iktFbtIK.enabled)
                    m_trackigMode = TrackingMode.IKTweaks;
                else
                    m_trackigMode = TrackingMode.FBT;
            }
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public void UpdateFromGestures(GestureMatcher.GesturesData p_gesturesData)
        {
            if(m_handGestureController != null)
            {
                m_handGestureController.field_Internal_Boolean_0 = true;
                m_handGestureController.field_Private_InputMethod_0 = VRCInputManager.InputMethod.Index;

                for(int i = 0; i < 2; i++)
                {
                    if(p_gesturesData.m_handsPresenses[i])
                    {
                        for(int j = 0; j < 5; j++)
                        {
                            int l_dataIndex = i * 5 + j;
                            m_handGestureController.field_Private_ArrayOf_VRCInput_0[l_dataIndex].field_Public_Single_0 = 1.0f - ((i == 0) ? p_gesturesData.m_leftFingersBends[j] : p_gesturesData.m_rightFingersBends[j]); // Bend
                            m_handGestureController.field_Private_ArrayOf_VRCInput_1[l_dataIndex].field_Public_Single_0 = ((i == 0) ? p_gesturesData.m_leftFingersSpreads[j] : p_gesturesData.m_rightFingersSpreads[j]); // Spread
                        }
                    }
                }
            }
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public void UpdateHandsPositions(GestureMatcher.GesturesData p_gesturesData, Transform p_left, Transform p_right)
        {
            if(!m_fingersOnly)
            {
                switch(m_trackigMode)
                {
                    case TrackingMode.Generic:
                    {
                        if(p_gesturesData.m_handsPresenses[0] && (m_solver?.leftArm?.target != null))
                        {
                            m_solver.leftArm.positionWeight = 1f;
                            m_solver.leftArm.rotationWeight = 1f;
                            m_solver.leftArm.target.position = p_left.position;
                            m_solver.leftArm.target.rotation = p_left.rotation;
                        }

                        if(p_gesturesData.m_handsPresenses[1] && (m_solver?.rightArm?.target != null))
                        {
                            m_solver.rightArm.positionWeight = 1f;
                            m_solver.rightArm.rotationWeight = 1f;
                            m_solver.rightArm.target.position = p_right.position;
                            m_solver.rightArm.target.rotation = p_right.rotation;
                        }
                    }
                    break;

                    case TrackingMode.FBT:
                    {
                        if(m_fbtIK != null)
                        {
                            if(p_gesturesData.m_handsPresenses[0] && (m_fbtIK.solver?.leftHandEffector != null))
                            {
                                m_fbtIK.solver.leftHandEffector.position = p_left.position;
                                m_fbtIK.solver.leftHandEffector.rotation = p_left.rotation;
                                m_fbtIK.solver.leftHandEffector.target.position = p_left.position;
                                m_fbtIK.solver.leftHandEffector.target.rotation = p_left.rotation;
                            }

                            if(p_gesturesData.m_handsPresenses[1] && (m_fbtIK.solver?.rightHandEffector != null))
                            {
                                m_fbtIK.solver.rightHandEffector.position = p_right.position;
                                m_fbtIK.solver.rightHandEffector.rotation = p_right.rotation;
                                m_fbtIK.solver.rightHandEffector.target.position = p_right.position;
                                m_fbtIK.solver.rightHandEffector.target.rotation = p_right.rotation;
                            }
                        }
                    }
                    break;

                    case TrackingMode.IKTweaks:
                    {
                        if(p_gesturesData.m_handsPresenses[0])
                        {
                            Transform l_controller = Utils.GetTrackingLeftController();
                            if(l_controller != null)
                            {
                                l_controller.position = p_left.position;
                                l_controller.rotation = p_left.rotation * ms_fixRotations[0];
                            }
                        }

                        if(p_gesturesData.m_handsPresenses[1])
                        {
                            Transform l_controller = Utils.GetTrackingRightController();
                            if(l_controller != null)
                            {
                                l_controller.position = p_right.position;
                                l_controller.rotation = p_right.rotation * ms_fixRotations[1];
                            }
                        }
                    }
                    break;
                }
            }

            if(m_handGestureController != null)
            {
                m_handGestureController.field_Internal_Boolean_0 = true;
                m_handGestureController.field_Private_InputMethod_0 = VRCInputManager.InputMethod.Index;
                for(int i = 0; i < 2; i++)
                {
                    if(p_gesturesData.m_handsPresenses[i])
                    {
                        for(int j = 0; j < 5; j++)
                        {
                            int l_dataIndex = i * 5 + j;
                            m_handGestureController.field_Private_ArrayOf_VRCInput_0[l_dataIndex].field_Public_Single_0 = 1.0f - ((i == 0) ? p_gesturesData.m_leftFingersBends[j] : p_gesturesData.m_rightFingersBends[j]); // Bend
                            m_handGestureController.field_Private_ArrayOf_VRCInput_1[l_dataIndex].field_Public_Single_0 = ((i == 0) ? p_gesturesData.m_leftFingersSpreads[j] : p_gesturesData.m_rightFingersSpreads[j]); // Spread
                        }
                    }
                }
            }
        }

        void RecacheComponents()
        {
            m_animator = m_player.field_Internal_Animator_0;
            m_handGestureController = m_player.field_Private_VRC_AnimationController_0.field_Private_HandGestureController_0;
            if(m_player.field_Private_VRC_AnimationController_0.field_Private_VRIK_0 != null)
                m_solver = m_player.field_Private_VRC_AnimationController_0.field_Private_VRIK_0.solver;
            else
                m_solver = null; // Generic avatar
            m_fbtIK = m_player.field_Private_VRC_AnimationController_0.field_Private_FullBodyBipedIK_0;
            m_iktFbtIK = null;
        }

        public void ResetTracking()
        {
            if(m_handGestureController != null)
            {
                m_handGestureController.field_Internal_Boolean_0 = false;
                m_handGestureController.field_Private_InputMethod_0 = VRCInputManager.InputMethod.Mouse;
            }
        }

        public void DetectIKTweaks()
        {
            if((m_iktFbtIK == null) && (m_animator != null))
                m_iktFbtIK = m_animator.GetComponent<RootMotionNew.FinalIK.VRIK_New>();
        }
    }
}
