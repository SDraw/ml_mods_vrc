using System;
using UnityEngine;

namespace ml_abp
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    class InteracterPlayer : MonoBehaviour
    {
        VRC.Player m_player = null;

        public VRC.Player Player
        {
            get => m_player;
        }

        public InteracterPlayer(IntPtr ptr) : base(ptr) { }

        void Awake()
        {
            m_player = this.GetComponent<VRC.Player>();
        }
    }
}
