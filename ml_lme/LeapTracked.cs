using System;
using System.Collections.Generic;
using UnityEngine;

namespace ml_lme
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    class LeapTracked : MonoBehaviour
    {
        static readonly string[] gs_parameterNames =
        {
            "_LeftHandPresent",
            "_RightHandPresent",
            "_LeftHandThumbBend",
            "_LeftHandIndexBend",
            "_LeftHandMiddleBend",
            "_LeftHandRingBend",
            "_LeftHandPinkyBend",
            "_LeftHandThumbSpread",
            "_LeftHandIndexSpread",
            "_LeftHandMiddleSpread",
            "_LeftHandRingSpread",
            "_LeftHandPinkySpread",
            "_RightHandThumbBend",
            "_RightHandIndexBend",
            "_RightHandMiddleBend",
            "_RightHandRingBend",
            "_RightHandPinkyBend",
            "_RightHandThumbSpread",
            "_RightHandIndexSpread",
            "_RightHandMiddleSpread",
            "_RightHandRingSpread",
            "_RightHandPinkySpread"
        };
        enum CustomParameterType
        {
            LeftHandPresent,
            RightHandPresent,
            LeftHandThumbBend,
            LeftHandIndexBend,
            LeftHandMiddleBend,
            LeftHandRingBend,
            LeftHandPinkyBend,
            LeftHandThumbSpread,
            LeftHandIndexSpread,
            LeftHandMiddleSpread,
            LeftHandRingSpread,
            LeftHandPinkySpread,
            RightHandThumbBend,
            RightHandIndexBend,
            RightHandMiddleBend,
            RightHandRingBend,
            RightHandPinkyBend,
            RightHandThumbSpread,
            RightHandIndexSpread,
            RightHandMiddleSpread,
            RightHandRingSpread,
            RightHandPinkySpread
        }

        enum TrackingMode
        {
            Generic = 0,
            FBT
        }

        // Struct would be better, but it's not C++
        class CustomParameter
        {
            public VRC.Playables.AvatarParameter.EnumNPublicSealedvaUnBoInFl5vUnique m_valueType;
            public bool m_boolValue;
            public float m_floatValue;
            public int m_intValue;
            public int m_paramHash;
            public CustomParameterType m_parameterType;
        }

        VRCPlayer m_player = null;
        AvatarPlayableController m_playableController = null;
        HandGestureController m_handGestureController = null;
        RootMotion.FinalIK.IKSolverVR m_solver = null;
        RootMotion.FinalIK.FullBodyBipedIK m_fbtIK = null;
        TrackingMode m_trackigMode = TrackingMode.Generic;

        List<CustomParameter> m_parameters = null;

        bool m_fingersOnly = false;
        bool m_updateParameters = false;

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public bool FingersOnly
        {
            set => m_fingersOnly = value;
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public bool Sdk3Parameters
        {
            set => m_updateParameters = value;
        }

        public LeapTracked(IntPtr ptr) : base(ptr) { }

        void Awake()
        {
            m_player = this.GetComponent<VRCPlayer>();
            m_parameters = new List<CustomParameter>();
        }

        void Update()
        {
            m_trackigMode = TrackingMode.Generic;
            if((m_fbtIK != null) && m_fbtIK.enabled)
                m_trackigMode = TrackingMode.FBT;

            if((m_playableController != null) && (m_parameters.Count != 0) && m_updateParameters)
            {
                foreach(var l_param in m_parameters)
                {
                    switch(l_param.m_valueType)
                    {
                        case VRC.Playables.AvatarParameter.EnumNPublicSealedvaUnBoInFl5vUnique.Bool:
                            m_playableController.SetAvatarBoolParamEx(l_param.m_paramHash, l_param.m_boolValue);
                            break;
                        case VRC.Playables.AvatarParameter.EnumNPublicSealedvaUnBoInFl5vUnique.Float:
                            m_playableController.SetAvatarFloatParamEx(l_param.m_paramHash, l_param.m_floatValue);
                            break;
                        case VRC.Playables.AvatarParameter.EnumNPublicSealedvaUnBoInFl5vUnique.Int:
                            m_playableController.SetAvatarIntParamEx(l_param.m_paramHash, l_param.m_intValue);
                            break;
                    }
                }
            }
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public void UpdateFromGestures(GestureMatcher.GesturesData f_gesturesData)
        {
            if((m_parameters.Count != 0) && m_updateParameters)
            {
                foreach(var l_param in m_parameters)
                {
                    switch(l_param.m_parameterType)
                    {
                        case CustomParameterType.LeftHandPresent:
                        case CustomParameterType.RightHandPresent:
                            l_param.m_boolValue = f_gesturesData.m_handsPresenses[(int)l_param.m_parameterType];
                            break;

                        case CustomParameterType.LeftHandThumbBend:
                        case CustomParameterType.LeftHandIndexBend:
                        case CustomParameterType.LeftHandMiddleBend:
                        case CustomParameterType.LeftHandRingBend:
                        case CustomParameterType.LeftHandPinkyBend:
                            l_param.m_floatValue = f_gesturesData.m_leftFingersBends[(int)l_param.m_parameterType - (int)CustomParameterType.LeftHandThumbBend];
                            break;

                        case CustomParameterType.RightHandThumbBend:
                        case CustomParameterType.RightHandIndexBend:
                        case CustomParameterType.RightHandMiddleBend:
                        case CustomParameterType.RightHandRingBend:
                        case CustomParameterType.RightHandPinkyBend:
                            l_param.m_floatValue = f_gesturesData.m_rightFingersBends[(int)l_param.m_parameterType - (int)CustomParameterType.RightHandThumbBend];
                            break;

                        case CustomParameterType.LeftHandThumbSpread:
                        case CustomParameterType.LeftHandIndexSpread:
                        case CustomParameterType.LeftHandMiddleSpread:
                        case CustomParameterType.LeftHandRingSpread:
                        case CustomParameterType.LeftHandPinkySpread:
                            l_param.m_floatValue = f_gesturesData.m_leftFingersSpreads[(int)l_param.m_parameterType - (int)CustomParameterType.LeftHandThumbSpread];
                            break;

                        case CustomParameterType.RightHandThumbSpread:
                        case CustomParameterType.RightHandIndexSpread:
                        case CustomParameterType.RightHandMiddleSpread:
                        case CustomParameterType.RightHandRingSpread:
                        case CustomParameterType.RightHandPinkySpread:
                            l_param.m_floatValue = f_gesturesData.m_rightFingersSpreads[(int)l_param.m_parameterType - (int)CustomParameterType.RightHandThumbSpread];
                            break;
                    }
                }
            }

            if(m_handGestureController != null)
            {
                m_handGestureController.field_Internal_Boolean_0 = true;
                m_handGestureController.field_Private_EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique_0 = VRCInputManager.EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique.Index;

                for(int i = 0; i < 2; i++)
                {
                    if(f_gesturesData.m_handsPresenses[i])
                    {
                        for(int j = 0; j < 5; j++)
                        {
                            int l_dataIndex = i * 5 + j;
                            m_handGestureController.field_Private_ArrayOf_VRCInput_0[l_dataIndex].field_Public_Single_0 = 1.0f - ((i == 0) ? f_gesturesData.m_leftFingersBends[j] : f_gesturesData.m_rightFingersBends[j]); // Bend
                            m_handGestureController.field_Private_ArrayOf_VRCInput_1[l_dataIndex].field_Public_Single_0 = ((i == 0) ? f_gesturesData.m_leftFingersSpreads[j] : f_gesturesData.m_rightFingersSpreads[j]); // Spread
                        }
                    }
                }
            }
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public void UpdateHandsPositions(GestureMatcher.GesturesData f_gesturesData, Transform f_left, Transform f_right)
        {
            if(!m_fingersOnly)
            {
                switch(m_trackigMode)
                {
                    case TrackingMode.Generic:
                    {
                        if(f_gesturesData.m_handsPresenses[0] && (m_solver?.leftArm?.target != null))
                        {
                            m_solver.leftArm.positionWeight = 1f;
                            m_solver.leftArm.rotationWeight = 1f;
                            m_solver.leftArm.target.position = f_left.position;
                            m_solver.leftArm.target.rotation = f_left.rotation;
                        }

                        if(f_gesturesData.m_handsPresenses[1] && (m_solver?.rightArm?.target != null))
                        {
                            m_solver.rightArm.positionWeight = 1f;
                            m_solver.rightArm.rotationWeight = 1f;
                            m_solver.rightArm.target.position = f_right.position;
                            m_solver.rightArm.target.rotation = f_right.rotation;
                        }
                    }
                    break;

                    case TrackingMode.FBT:
                    {
                        if(m_fbtIK != null)
                        {
                            if(f_gesturesData.m_handsPresenses[0] && (m_fbtIK.solver?.leftHandEffector != null))
                            {
                                m_fbtIK.solver.leftHandEffector.position = f_left.position;
                                m_fbtIK.solver.leftHandEffector.rotation = f_left.rotation;
                                m_fbtIK.solver.leftHandEffector.target.position = f_left.position;
                                m_fbtIK.solver.leftHandEffector.target.rotation = f_left.rotation;
                            }

                            if(f_gesturesData.m_handsPresenses[1] && (m_fbtIK.solver?.rightHandEffector != null))
                            {
                                m_fbtIK.solver.rightHandEffector.position = f_right.position;
                                m_fbtIK.solver.rightHandEffector.rotation = f_right.rotation;
                                m_fbtIK.solver.rightHandEffector.target.position = f_right.position;
                                m_fbtIK.solver.rightHandEffector.target.rotation = f_right.rotation;
                            }
                        }
                    }
                    break;
                }
            }

            if(m_handGestureController != null)
            {
                m_handGestureController.field_Internal_Boolean_0 = true;
                m_handGestureController.field_Private_EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique_0 = VRCInputManager.EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique.Index;
                for(int i = 0; i < 2; i++)
                {
                    if(f_gesturesData.m_handsPresenses[i])
                    {
                        for(int j = 0; j < 5; j++)
                        {
                            int l_dataIndex = i * 5 + j;
                            m_handGestureController.field_Private_ArrayOf_VRCInput_0[l_dataIndex].field_Public_Single_0 = 1.0f - ((i == 0) ? f_gesturesData.m_leftFingersBends[j] : f_gesturesData.m_rightFingersBends[j]); // Bend
                            m_handGestureController.field_Private_ArrayOf_VRCInput_1[l_dataIndex].field_Public_Single_0 = ((i == 0) ? f_gesturesData.m_leftFingersSpreads[j] : f_gesturesData.m_rightFingersSpreads[j]); // Spread
                        }
                    }
                }
            }

        }

        public void ResetParameters()
        {
            m_parameters.Clear();
            m_playableController = null;

            m_handGestureController = m_player.field_Private_VRC_AnimationController_0.field_Private_HandGestureController_0;
            if(m_player.field_Private_VRC_AnimationController_0.field_Private_VRIK_0 != null)
                m_solver = m_player.field_Private_VRC_AnimationController_0.field_Private_VRIK_0.solver;
            else
                m_solver = null; // Generic avatar
            m_fbtIK = m_player.field_Private_VRC_AnimationController_0.field_Private_FullBodyBipedIK_0;

            RebuildParameters();
        }

        public void ResetTracking()
        {
            if(m_handGestureController != null)
            {
                m_handGestureController.field_Internal_Boolean_0 = false;
                m_handGestureController.field_Private_EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique_0 = VRCInputManager.EnumNPublicSealedvaKeMoCoGaViOcViDaWaUnique.Mouse;
            }
        }

        void RebuildParameters()
        {
            m_playableController = m_player.field_Private_AnimatorControllerManager_0.field_Private_AvatarAnimParamController_0.field_Private_AvatarPlayableController_0;
            if(m_playableController != null)
            {
                foreach(var l_param in m_playableController.field_Private_ArrayOf_ObjectNPublicInObInPaInUnique_0)
                {
                    for(int i = 0; i < gs_parameterNames.Length; i++)
                    {
                        if(l_param.field_Public_AvatarParameter_0?.field_Private_String_0 == gs_parameterNames[i])
                        {
                            m_parameters.Add(new CustomParameter
                            {
                                m_boolValue = false,
                                m_intValue = 0,
                                m_floatValue = 0f,
                                m_parameterType = (CustomParameterType)i,
                                m_paramHash = l_param.field_Public_Int32_0,
                                m_valueType = l_param.field_Public_AvatarParameter_0.field_Private_EnumNPublicSealedvaUnBoInFl5vUnique_0
                            });
                            break;
                        }
                    }
                }
            }
        }
    }
}
