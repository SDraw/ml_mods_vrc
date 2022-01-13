using System;
using System.Collections.Generic;
using UnityEngine;

namespace ml_abp
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    class InteracterPlayer : MonoBehaviour
    {
        static readonly HumanBodyBones[] ms_handBones =
        {
            HumanBodyBones.LeftHand, HumanBodyBones.RightHand
        };

        VRCPlayer m_player = null;

        readonly List<Transform> m_hands = null;
        Animator m_animator = null;

        public VRCPlayer Player
        {
            get => m_player;
        }

        public InteracterPlayer(IntPtr ptr) : base(ptr)
        {
            m_hands = new List<Transform>();
        }

        void Awake()
        {
            m_player = this.GetComponent<VRCPlayer>();
            m_player.field_Private_OnAvatarIsReady_0 += new Action(this.RecacheComponents);

            RecacheComponents();
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public List<Transform> GetProximityTargets() => m_hands;

        void RecacheComponents()
        {
            m_hands.Clear();

            m_animator = m_player.field_Internal_Animator_0;

            if((m_animator != null) && m_animator.isHuman)
            {
                foreach(HumanBodyBones l_bone in ms_handBones)
                {
                    Transform l_boneTransform = m_animator.GetBoneTransform(l_bone);
                    if(l_boneTransform != null)
                        m_hands.Add(l_boneTransform);
                }
            }
        }
    }
}
