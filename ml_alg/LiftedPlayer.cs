using System;
using UnityEngine;

namespace ml_alg
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    class LiftedPlayer : MonoBehaviour
    {
        static readonly HumanBodyBones[] gs_updateBones = { HumanBodyBones.Head, HumanBodyBones.Hips, HumanBodyBones.LeftHand, HumanBodyBones.RightHand, HumanBodyBones.LeftFoot, HumanBodyBones.RightFoot };
        static readonly HumanBodyBones[] gs_lateUpdateBones = { HumanBodyBones.Hips, HumanBodyBones.LeftHand, HumanBodyBones.RightHand, HumanBodyBones.LeftFoot, HumanBodyBones.RightFoot };

        enum BodyTrackingMode
        {
            Generic = 0, // Desktop, 3 point, 4 point
            FBT, // VRChat's lanky FBT
            IKTweaks // Timber limbs after grab
        }

        enum OffsetReapplyState
        {
            None = 0,
            Await,
            Final
        }

        struct LiftBodyPart
        {
            public LifterPlayer m_lifter;
            public HumanBodyBones m_attachedBone;
            public Matrix4x4 m_offsetMatrix;
            public Vector3 m_targetPos;
            public Quaternion m_targetRot;
            public OffsetReapplyState m_reapplyOffset;
            public bool m_saved;
        };

        VRC.Player m_player;
        LiftBodyPart[] m_liftBodyParts = null;
        float m_grabDistance = 0.25f;
        Vector3 m_lastPoint;
        Vector3 m_frameVelocity;
        float m_velocityMultiplier = 5f;
        bool m_rotateHips = true;
        bool m_rotateLegs = true;
        bool m_rotateHands = true;
        bool m_allowPull = true;
        bool m_allowHandsPull = true;
        bool m_allowHipsPull = true;
        bool m_allowLegsPull = true;
        bool m_savePose = false;
        bool m_useVelocity = false;
        bool m_averageVelocity = true;
        bool m_useIKTweaks = false;

        Animator m_animator = null;
        RootMotion.FinalIK.IKSolverVR m_solver = null;
        RootMotion.FinalIK.FullBodyBipedIK m_fbtIK = null;
        MonoBehaviour m_iktFbtIK = null;
        BodyTrackingMode m_trackingMode = BodyTrackingMode.Generic;

        public LiftedPlayer(IntPtr ptr) : base(ptr) { }

        public VRC.Player Player
        {
            get => m_player;
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public float GrabDistance
        {
            set => m_grabDistance = value;
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public bool RotateHips
        {
            set => m_rotateHips = value;
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public bool RotateLegs
        {
            set => m_rotateLegs = value;
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public bool RotateHands
        {
            set => m_rotateHands = value;
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public bool AllowPull
        {
            set => m_allowPull = value;
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public bool AllowHandsPull
        {
            set => m_allowHandsPull = value;
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public bool AllowHipsPull
        {
            set => m_allowHipsPull = value;
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public bool AllowLegsPull
        {
            set => m_allowLegsPull = value;
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public bool SavePose
        {
            set
            {
                m_savePose = value;
                if(!m_savePose)
                    ClearSavedLiftedBones();
            }
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public bool UseVelocity
        {
            set => m_useVelocity = value;
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public float VelocityMultiplier
        {
            set => m_velocityMultiplier = value;
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public bool AverageVelocity
        {
            set => m_averageVelocity = value;
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public bool UseIKTweaks
        {
            set => m_useIKTweaks = value;
        }

        void Awake()
        {
            m_player = this.GetComponent<VRC.Player>();

            // Reduntant array: 6 used, 49 dummies
            m_liftBodyParts = new LiftBodyPart[(int)HumanBodyBones.LastBone];
            for(int i = 0; i < (int)HumanBodyBones.LastBone; i++)
            {
                m_liftBodyParts[i] = new LiftBodyPart
                {
                    m_lifter = null,
                    m_attachedBone = HumanBodyBones.LastBone,
                    m_offsetMatrix = Matrix4x4.identity,
                    m_targetPos = Vector3.zero,
                    m_targetRot = Quaternion.identity,
                    m_reapplyOffset = OffsetReapplyState.None,
                    m_saved = false
                };
            }
        }
        void Update()
        {
            // Recheck tracking mode
            m_trackingMode = BodyTrackingMode.Generic;
            if((m_fbtIK != null) && m_fbtIK.enabled)
            {
                m_trackingMode = BodyTrackingMode.FBT;
            }
            if(m_useIKTweaks && IKTweaksHelper.Present)
            {
                if(m_iktFbtIK == null)
                {
                    if(m_animator != null)
                    {
                        // If you have better way to detect new component presence, just tell
                        var l_components = m_animator.GetComponents<MonoBehaviour>();
                        foreach(var l_component in l_components)
                        {
                            if((l_component != null) && (l_component.GetScriptClassName() == "VRIK_New"))
                            {
                                m_iktFbtIK = l_component;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if(m_iktFbtIK.enabled)
                        m_trackingMode = BodyTrackingMode.IKTweaks;
                }
            }

            foreach(int i in gs_updateBones)
            {
                if((m_liftBodyParts[i].m_lifter != null) && (m_liftBodyParts[i].m_attachedBone != HumanBodyBones.LastBone))
                {
                    if(m_liftBodyParts[i].m_lifter.Animator != null)
                    {
                        var l_bonePoint = m_liftBodyParts[i].m_lifter.Animator.GetBoneTransform(m_liftBodyParts[i].m_attachedBone);
                        if(l_bonePoint != null)
                        {
                            switch(i)
                            {
                                case (int)HumanBodyBones.Head:
                                {
                                    var l_transform = m_player.transform;
                                    if(l_transform != null)
                                    {
                                        l_transform.position = (l_bonePoint.transform.GetMatrix(true, false) * m_liftBodyParts[i].m_offsetMatrix) * Utils.gs_pointVec4;
                                        m_player.field_Private_VRCPlayerApi_0.SetVelocity(Vector3.zero);
                                    }

                                    if(m_useVelocity)
                                    {
                                        var l_position = m_player.field_Private_VRCPlayerApi_0.GetPosition();
                                        var l_frameVelocity = ((l_position - m_lastPoint) / Time.deltaTime) * m_velocityMultiplier;
                                        m_frameVelocity = (m_averageVelocity ? ((m_frameVelocity + l_frameVelocity) * 0.5f) : l_frameVelocity);
                                        m_lastPoint = l_position;
                                    }
                                }
                                break;

                                case (int)HumanBodyBones.Hips:
                                case (int)HumanBodyBones.LeftFoot:
                                case (int)HumanBodyBones.RightFoot:
                                {
                                    var l_resultMatrix = l_bonePoint.transform.GetMatrix() * m_liftBodyParts[i].m_offsetMatrix;
                                    m_liftBodyParts[i].m_targetPos = l_resultMatrix * Utils.gs_pointVec4;
                                    m_liftBodyParts[i].m_targetRot = l_resultMatrix.rotation;
                                }
                                break;

                                case (int)HumanBodyBones.LeftHand:
                                case (int)HumanBodyBones.RightHand:
                                {
                                    // Ha-ha, funny hands, very cool
                                    ReapplyOffset(i);

                                    var l_resultMatrix = l_bonePoint.transform.GetMatrix() * m_liftBodyParts[i].m_offsetMatrix;
                                    m_liftBodyParts[i].m_targetPos = l_resultMatrix * Utils.gs_pointVec4;
                                    m_liftBodyParts[i].m_targetRot = l_resultMatrix.rotation;
                                }
                                break;
                            }
                        }
                    }
                    continue;
                }

                if(m_savePose && m_liftBodyParts[i].m_saved)
                {
                    switch(i)
                    {
                        case (int)HumanBodyBones.Hips:
                        case (int)HumanBodyBones.LeftHand:
                        case (int)HumanBodyBones.RightHand:
                        case (int)HumanBodyBones.LeftFoot:
                        case (int)HumanBodyBones.RightFoot:
                        {
                            var l_resultMatrix = m_player.transform.GetMatrix() * m_liftBodyParts[i].m_offsetMatrix;
                            m_liftBodyParts[i].m_targetPos = l_resultMatrix * Utils.gs_pointVec4;
                            m_liftBodyParts[i].m_targetRot = l_resultMatrix.rotation;
                        }
                        break;
                    }
                }
            }
        }

        void LateUpdate()
        {
            foreach(int i in gs_lateUpdateBones)
            {
                if((m_liftBodyParts[i].m_attachedBone != HumanBodyBones.LastBone) || m_liftBodyParts[i].m_saved)
                {
                    switch(m_trackingMode)
                    {
                        case BodyTrackingMode.Generic:
                        {
                            switch(i)
                            {
                                case (int)HumanBodyBones.Hips:
                                {
                                    var l_spine = m_solver?.spine;
                                    if(l_spine?.pelvisTarget != null)
                                    {
                                        l_spine.pelvisPositionWeight = 1f;
                                        l_spine.IKPositionPelvis = m_liftBodyParts[i].m_targetPos;
                                        l_spine.pelvisTarget.position = m_liftBodyParts[i].m_targetPos;
                                        if(m_rotateHips)
                                        {
                                            l_spine.pelvisRotationWeight = 1f;
                                            l_spine.IKRotationPelvis = m_liftBodyParts[i].m_targetRot;
                                            l_spine.pelvisTarget.rotation = m_liftBodyParts[i].m_targetRot;
                                        }
                                    }
                                }
                                break;

                                case (int)HumanBodyBones.LeftHand:
                                case (int)HumanBodyBones.RightHand:
                                {
                                    var l_ikArm = (i == (int)HumanBodyBones.LeftHand) ? m_solver?.leftArm : m_solver?.rightArm;
                                    if(l_ikArm?.target != null)
                                    {
                                        l_ikArm.positionWeight = 1f;
                                        l_ikArm.target.transform.position = m_liftBodyParts[i].m_targetPos;
                                        if(m_rotateHands)
                                        {
                                            l_ikArm.rotationWeight = 1f;
                                            l_ikArm.target.transform.rotation = m_liftBodyParts[i].m_targetRot;
                                        }
                                    }
                                }
                                break;

                                case (int)HumanBodyBones.LeftFoot:
                                case (int)HumanBodyBones.RightFoot:
                                {
                                    var l_ikLeg = (i == (int)HumanBodyBones.LeftFoot) ? m_solver?.leftLeg : m_solver?.rightLeg;
                                    if(l_ikLeg != null)
                                    {
                                        l_ikLeg.IKPosition = m_liftBodyParts[i].m_targetPos;
                                        if(m_rotateLegs)
                                            l_ikLeg.IKRotation = m_liftBodyParts[i].m_targetRot;
                                    }
                                }
                                break;
                            }
                        }
                        break;

                        case BodyTrackingMode.FBT:
                        {
                            if(m_fbtIK != null)
                            {
                                RootMotion.FinalIK.IKEffector l_effector = null;
                                bool l_rotate = true;
                                switch(i)
                                {
                                    case (int)HumanBodyBones.Hips:
                                    {
                                        l_effector = m_fbtIK.solver?.bodyEffector;
                                        l_rotate = m_rotateHips;
                                    }
                                    break;
                                    case (int)HumanBodyBones.LeftHand:
                                    {
                                        l_effector = m_fbtIK.solver?.leftHandEffector;
                                        l_rotate = m_rotateHands;
                                    }
                                    break;
                                    case (int)HumanBodyBones.RightHand:
                                    {
                                        l_effector = m_fbtIK.solver?.rightHandEffector;
                                        l_rotate = m_rotateHands;
                                    }
                                    break;
                                    case (int)HumanBodyBones.LeftFoot:
                                    {
                                        l_effector = m_fbtIK.solver?.leftFootEffector;
                                        l_rotate = m_rotateLegs;
                                    }
                                    break;
                                    case (int)HumanBodyBones.RightFoot:
                                    {
                                        l_effector = m_fbtIK.solver?.rightFootEffector;
                                        l_rotate = m_rotateLegs;
                                    }
                                    break;
                                }
                                if(l_effector?.target != null)
                                {
                                    l_effector.position = m_liftBodyParts[i].m_targetPos;
                                    l_effector.target.position = m_liftBodyParts[i].m_targetPos;
                                    if(l_rotate)
                                    {
                                        l_effector.rotation = m_liftBodyParts[i].m_targetRot;
                                        l_effector.target.rotation = m_liftBodyParts[i].m_targetRot;
                                    }
                                }
                            }
                        }
                        break;

                        case BodyTrackingMode.IKTweaks:
                        {
                            if(m_iktFbtIK != null)
                            {
                                switch(i)
                                {
                                    case (int)HumanBodyBones.Hips:
                                        IKTweaksHelper.SetHipsIK(m_iktFbtIK, m_liftBodyParts[i].m_targetPos, m_liftBodyParts[i].m_targetRot, m_rotateHips);
                                        break;
                                    case (int)HumanBodyBones.LeftHand:
                                        IKTweaksHelper.SetLeftHandIK(m_iktFbtIK, m_liftBodyParts[i].m_targetPos, m_liftBodyParts[i].m_targetRot, m_rotateHands);
                                        break;
                                    case (int)HumanBodyBones.RightHand:
                                        IKTweaksHelper.SetRightHandIK(m_iktFbtIK, m_liftBodyParts[i].m_targetPos, m_liftBodyParts[i].m_targetRot, m_rotateHands);
                                        break;
                                    case (int)HumanBodyBones.LeftFoot:
                                        IKTweaksHelper.SetLeftLegIK(m_iktFbtIK, m_liftBodyParts[i].m_targetPos, m_liftBodyParts[i].m_targetRot, m_rotateLegs);
                                        break;
                                    case (int)HumanBodyBones.RightFoot:
                                        IKTweaksHelper.SetRightLegIK(m_iktFbtIK, m_liftBodyParts[i].m_targetPos, m_liftBodyParts[i].m_targetRot, m_rotateLegs);
                                        break;
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }

        public void RecacheComponents()
        {
            m_animator = m_player.prop_VRCPlayer_0.field_Internal_Animator_0;
            if(m_player.prop_VRCPlayer_0.field_Private_VRC_AnimationController_0.field_Private_VRIK_0 != null)
                m_solver = m_player.prop_VRCPlayer_0.field_Private_VRC_AnimationController_0.field_Private_VRIK_0.solver;
            else
                m_solver = null; // Generic avatar
            m_fbtIK = m_player.prop_VRCPlayer_0.field_Private_VRC_AnimationController_0.field_Private_FullBodyBipedIK_0;
            m_iktFbtIK = null;
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public void OnLifterGesture(LifterPlayer f_lifter, HumanBodyBones f_hand, bool f_state)
        {
            if(f_state)
            {
                if((m_animator != null) && (f_lifter.Animator != null))
                {
                    var l_nearestBone = GetNearestBone(f_lifter.Animator, f_hand);
                    if(l_nearestBone != HumanBodyBones.LastBone)
                    {
                        switch(l_nearestBone)
                        {
                            case HumanBodyBones.Head:
                            {
                                if(m_allowPull)
                                    AssignLiftedBone(f_lifter, f_lifter.Animator, l_nearestBone, f_hand);
                            }
                            break;

                            case HumanBodyBones.LeftHand:
                            case HumanBodyBones.RightHand:
                            {
                                if(m_allowHandsPull)
                                    AssignLiftedBone(f_lifter, f_lifter.Animator, l_nearestBone, f_hand);
                            }
                            break;

                            case HumanBodyBones.Hips:
                            {
                                if(m_allowHipsPull)
                                    AssignLiftedBone(f_lifter, f_lifter.Animator, l_nearestBone, f_hand);
                            }
                            break;

                            case HumanBodyBones.LeftFoot:
                            case HumanBodyBones.RightFoot:
                            {
                                if(m_allowLegsPull)
                                    AssignLiftedBone(f_lifter, f_lifter.Animator, l_nearestBone, f_hand);
                            }
                            break;
                        }
                    }
                }
            }
            else
                UnassignRemoteLifter(f_lifter, f_hand);
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        void AssignLiftedBone(LifterPlayer f_lifter, Animator f_remoteAnimator, HumanBodyBones f_localBone, HumanBodyBones f_remoteBone)
        {
            m_liftBodyParts[(int)f_localBone].m_lifter = f_lifter;
            m_liftBodyParts[(int)f_localBone].m_attachedBone = f_remoteBone;
            switch(f_localBone)
            {
                case HumanBodyBones.Head:
                {
                    m_liftBodyParts[(int)f_localBone].m_offsetMatrix = f_remoteAnimator.GetBoneTransform(f_remoteBone).transform.GetMatrix(true, false).inverse * m_player.transform.GetMatrix(true, false);
                    m_player.prop_VRCPlayerApi_0.Immobilize(true);

                    if(m_useVelocity)
                    {
                        m_frameVelocity = Vector3.zero;
                        m_lastPoint = m_player.field_Private_VRCPlayerApi_0.GetPosition();
                    }
                }
                break;

                case HumanBodyBones.Hips:
                {
                    switch(m_trackingMode)
                    {
                        case BodyTrackingMode.IKTweaks:
                        {
                            var l_trackerPoint = IKTweaksHelper.GetHipsTarget(m_iktFbtIK);
                            if((l_trackerPoint != null) && (l_trackerPoint.parent != null))
                            {
                                m_liftBodyParts[(int)f_localBone].m_offsetMatrix = f_remoteAnimator.GetBoneTransform(f_remoteBone).GetMatrix().inverse * l_trackerPoint.parent.GetMatrix();
                            }
                        }
                        break;
                        default:
                            m_liftBodyParts[(int)f_localBone].m_offsetMatrix = f_remoteAnimator.GetBoneTransform(f_remoteBone).GetMatrix().inverse * m_animator.GetBoneTransform(f_localBone).GetMatrix();
                            break;
                    }
                }
                break;

                case HumanBodyBones.LeftHand:
                case HumanBodyBones.RightHand:
                {
                    switch(m_trackingMode)
                    {
                        case BodyTrackingMode.IKTweaks:
                        {
                            var l_trackerPoint = (f_localBone == HumanBodyBones.LeftHand) ? IKTweaksHelper.GetLeftHandTarget(m_iktFbtIK) : IKTweaksHelper.GetRightHandTarget(m_iktFbtIK);
                            if((l_trackerPoint != null) && (l_trackerPoint.parent != null))
                            {
                                m_liftBodyParts[(int)f_localBone].m_offsetMatrix = f_remoteAnimator.GetBoneTransform(f_remoteBone).GetMatrix().inverse * l_trackerPoint.parent.GetMatrix();
                            }
                        } break;

                        default:
                        {
                            m_liftBodyParts[(int)f_localBone].m_offsetMatrix = f_remoteAnimator.GetBoneTransform(f_remoteBone).GetMatrix().inverse * m_animator.GetBoneTransform(f_localBone).GetMatrix();
                            m_liftBodyParts[(int)f_localBone].m_reapplyOffset = OffsetReapplyState.Await;
                        }
                        break;
                    }
                }
                break;

                case HumanBodyBones.LeftFoot:
                case HumanBodyBones.RightFoot:
                {
                    switch(m_trackingMode)
                    {
                        case BodyTrackingMode.IKTweaks:
                        {
                            var l_trackerPoint = (f_localBone == HumanBodyBones.LeftFoot) ? IKTweaksHelper.GetLeftLegTarget(m_iktFbtIK) : IKTweaksHelper.GetRightLegTarget(m_iktFbtIK);
                            if((l_trackerPoint != null) && (l_trackerPoint.parent != null))
                            {
                                m_liftBodyParts[(int)f_localBone].m_offsetMatrix = f_remoteAnimator.GetBoneTransform(f_remoteBone).GetMatrix().inverse * l_trackerPoint.parent.GetMatrix();
                            }
                        } break;

                        default:
                        {
                            Transform l_footTransform = null;
                            if(m_trackingMode == BodyTrackingMode.Generic)
                            {
                                // Consider toes if avatar has them
                                l_footTransform = m_animator.GetBoneTransform((f_localBone == HumanBodyBones.LeftFoot) ? HumanBodyBones.LeftToes : HumanBodyBones.RightToes);
                                if(l_footTransform == null)
                                    l_footTransform = m_animator.GetBoneTransform(f_localBone);
                                m_solver?.SetLegIKWeight(f_localBone, 1f, m_rotateLegs);
                            }
                            else
                                l_footTransform = m_animator.GetBoneTransform(f_localBone);

                            m_liftBodyParts[(int)f_localBone].m_offsetMatrix = f_remoteAnimator.GetBoneTransform(f_remoteBone).GetMatrix().inverse * l_footTransform.GetMatrix();
                        } break;
                    }
                }
                break;
            }
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        void UnassignRemoteLifter(LifterPlayer f_player, HumanBodyBones f_remoteBone)
        {
            for(int i = 0; i < (int)HumanBodyBones.LastBone; i++)
            {
                if((m_liftBodyParts[i].m_lifter == f_player) && (m_liftBodyParts[i].m_attachedBone == f_remoteBone))
                {
                    ProcessBoneReset((HumanBodyBones)i);
                    break;
                }
            }
        }

        public void UnassignRemoteLifter(LifterPlayer f_player)
        {
            if(f_player != null)
            {
                for(int i = 0; i < (int)HumanBodyBones.LastBone; i++)
                {
                    if(m_liftBodyParts[i].m_lifter == f_player)
                    {
                        ProcessBoneReset((HumanBodyBones)i);
                    }
                }
            }
        }

        public void ReapplyPermissions()
        {
            if(m_liftBodyParts != null)
            {
                if(!m_allowPull && (m_liftBodyParts[(int)HumanBodyBones.Head].m_lifter != null))
                    ProcessBoneReset(HumanBodyBones.Head);

                if(!m_allowHipsPull && (m_liftBodyParts[(int)HumanBodyBones.Hips].m_lifter != null))
                    ProcessBoneReset(HumanBodyBones.Hips);

                if(!m_allowHandsPull)
                {
                    if(m_liftBodyParts[(int)HumanBodyBones.LeftHand].m_lifter != null)
                        ProcessBoneReset(HumanBodyBones.LeftHand);
                    if(m_liftBodyParts[(int)HumanBodyBones.RightHand].m_lifter != null)
                        ProcessBoneReset(HumanBodyBones.RightHand);
                }

                if(!m_allowLegsPull)
                {
                    if(m_liftBodyParts[(int)HumanBodyBones.LeftFoot].m_lifter != null)
                        ProcessBoneReset(HumanBodyBones.LeftFoot);
                    if(m_liftBodyParts[(int)HumanBodyBones.RightFoot].m_lifter != null)
                        ProcessBoneReset(HumanBodyBones.RightFoot);
                }
            }
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        void ProcessBoneReset(HumanBodyBones f_bone)
        {
            m_liftBodyParts[(int)f_bone].m_lifter = null;
            m_liftBodyParts[(int)f_bone].m_attachedBone = HumanBodyBones.LastBone;
            m_liftBodyParts[(int)f_bone].m_offsetMatrix = Matrix4x4.identity;

            if(f_bone == HumanBodyBones.Head)
            {
                m_player.prop_VRCPlayerApi_0.Immobilize(false);

                if(m_useVelocity)
                {
                    m_player.field_Private_VRCPlayerApi_0.SetVelocity(m_frameVelocity);
                    m_frameVelocity = Vector3.zero;
                }
            }

            if(m_savePose)
            {
                switch(f_bone)
                {
                    case HumanBodyBones.Hips:
                    case HumanBodyBones.LeftHand:
                    case HumanBodyBones.RightHand:
                    case HumanBodyBones.LeftFoot:
                    case HumanBodyBones.RightFoot:
                    {
                        m_liftBodyParts[(int)f_bone].m_offsetMatrix = m_player.transform.GetMatrix().inverse * Matrix4x4.TRS(m_liftBodyParts[(int)f_bone].m_targetPos, m_liftBodyParts[(int)f_bone].m_targetRot, Vector3.one);
                        m_liftBodyParts[(int)f_bone].m_saved = true;
                    }
                    break;
                }
            }
            else
            {
                switch(f_bone)
                {
                    case HumanBodyBones.LeftFoot:
                    case HumanBodyBones.RightFoot:
                    {
                        if(m_trackingMode == BodyTrackingMode.Generic)
                            m_solver?.SetLegIKWeight(f_bone, 0f, m_rotateLegs);
                    }
                    break;
                }
            }

            m_liftBodyParts[(int)f_bone].m_targetPos = Vector3.zero;
            m_liftBodyParts[(int)f_bone].m_targetRot = Quaternion.identity;
            m_liftBodyParts[(int)f_bone].m_reapplyOffset = OffsetReapplyState.None;
        }

        public void ClearSavedLiftedBones()
        {
            if(m_liftBodyParts != null)
            {
                for(int i = 0; i < (int)HumanBodyBones.LastBone; i++)
                {
                    m_liftBodyParts[i].m_saved = false;
                    if((m_liftBodyParts[i].m_attachedBone == HumanBodyBones.LastBone) && (m_trackingMode == BodyTrackingMode.Generic))
                    {
                        switch(i)
                        {
                            case (int)HumanBodyBones.LeftFoot:
                            case (int)HumanBodyBones.RightFoot:
                                m_solver?.SetLegIKWeight((HumanBodyBones)i, 0f, m_rotateLegs);
                                break;
                        }
                    }
                }
            }
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        HumanBodyBones GetNearestBone(Animator f_remoteAnimator, HumanBodyBones f_remoteBone)
        {
            var l_result = HumanBodyBones.LastBone;
            if(m_animator.isHuman && f_remoteAnimator.isHuman)
            {
                var l_remoteTransform = f_remoteAnimator.GetBoneTransform(f_remoteBone);
                if(l_remoteTransform != null)
                {
                    float l_nearestDistance = float.MaxValue;
                    foreach(var l_bone in gs_updateBones)
                    {
                        var l_localTransform = m_animator.GetBoneTransform(l_bone);
                        if(l_localTransform != null)
                        {
                            float l_distance = Vector3.Distance(l_remoteTransform.position, l_localTransform.position);
                            if(l_distance < l_nearestDistance)
                            {
                                l_nearestDistance = l_distance;
                                l_result = l_bone;
                            }
                        }
                    }
                    if(l_nearestDistance > m_grabDistance)
                        l_result = HumanBodyBones.LastBone;
                }
            }
            return l_result;
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        void ReapplyOffset(int f_index)
        {
            switch(m_liftBodyParts[f_index].m_reapplyOffset)
            {
                case OffsetReapplyState.Await:
                    m_liftBodyParts[f_index].m_reapplyOffset = OffsetReapplyState.Final;
                    break;
                case OffsetReapplyState.Final:
                {
                    m_liftBodyParts[f_index].m_reapplyOffset = OffsetReapplyState.None;

                    if(m_animator != null)
                    {
                        var l_boneTransform = m_animator.GetBoneTransform((HumanBodyBones)f_index);
                        if(l_boneTransform != null)
                        {
                            m_liftBodyParts[f_index].m_offsetMatrix = m_liftBodyParts[f_index].m_offsetMatrix * (Quaternion.Inverse(l_boneTransform.transform.rotation) * m_liftBodyParts[f_index].m_targetRot).AsMatrix();
                        }
                    }
                }
                break;
            }
        }
    }
}
