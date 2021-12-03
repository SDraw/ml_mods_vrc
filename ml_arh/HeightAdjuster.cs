using System;
using UnityEngine;

namespace ml_ahr
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    class HeightAdjuster : MonoBehaviour
    {
        CharacterController m_characterController = null;

        public HeightAdjuster(IntPtr ptr) : base(ptr) { }

        void Awake()
        {
            m_characterController = this.GetComponent<CharacterController>();
        }

        public void ChangeHeight(float f_height, float f_center)
        {
            if(m_characterController != null)
            {
                m_characterController.height = f_height;

                var l_center = m_characterController.center;
                l_center.y = f_center;
                m_characterController.center = l_center;
            }
        }
    }
}
