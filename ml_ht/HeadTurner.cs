using System;
using UnityEngine;

namespace ml_ht
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    class HeadTurner : MonoBehaviour
    {
        float m_lockedBodyRotation = 0f;
        bool m_lockBodyRotation = false;

        VRCPlayer m_player = null;
        GamelikeInputController m_inputController = null;

        public HeadTurner(IntPtr ptr) : base(ptr) { }

        void Awake()
        {
            m_player = this.GetComponent<VRCPlayer>();
            MelonLoader.MelonCoroutines.Start(FindInputController());
        }

        void LateUpdate()
        {
            if((m_player != null) && (m_inputController != null))
            {
                bool l_ignoreLimit = Input.GetKey(KeyCode.LeftControl);

                if(Input.GetKey(KeyCode.LeftAlt))
                {
                    float l_angle = Input.mouseScrollDelta.y;
                    if(l_angle != 0f)
                    {
                        var l_neckRotator = m_inputController.field_Protected_MonoBehaviourPublicObSiBoSiVeBoQuVeBoSiUnique_0;
                        if(l_neckRotator != null)
                        {
                            var l_quat = l_neckRotator.field_Private_Quaternion_0;
                            var l_vec = l_quat.eulerAngles;
                            l_vec.z += l_angle * 5f;
                            if(!l_ignoreLimit)
                            {
                                float l_delta = Mathf.DeltaAngle(l_vec.z, 0f);
                                if(Mathf.Abs(l_delta) > 90f)
                                {
                                    l_vec.z += (Mathf.Abs(l_delta) - 90f) * Mathf.Sign(l_delta);
                                    l_vec.z = (l_vec.z + 360f) % 360f;
                                }
                            }
                            l_quat.eulerAngles = l_vec;
                            l_neckRotator.field_Private_Quaternion_0 = l_quat;
                        }
                    }
                }

                if(Input.GetKeyDown(KeyCode.LeftAlt) && !m_lockBodyRotation)
                {
                    var l_transform = m_player.prop_VRCAvatarManager_0.transform;
                    if(l_transform != null)
                    {
                        m_lockedBodyRotation = l_transform.rotation.eulerAngles.y;
                        m_lockBodyRotation = true;
                    }
                }
                if((Input.GetKeyUp(KeyCode.LeftAlt) || !Application.isFocused) && m_lockBodyRotation)
                {
                    var l_transformAvatar = m_player.prop_VRCAvatarManager_0.transform;
                    if(l_transformAvatar != null)
                    {
                        l_transformAvatar.rotation = m_player.transform.rotation;
                    }
                    m_lockBodyRotation = false;
                }
                if(m_lockBodyRotation)
                {
                    var l_animatorTransform = m_player.prop_VRCAvatarManager_0.transform;
                    if(l_animatorTransform != null)
                    {
                        if(!l_ignoreLimit)
                        {
                            float l_delta = Mathf.DeltaAngle(m_lockedBodyRotation, m_player.transform.eulerAngles.y);
                            if(Mathf.Abs(l_delta) > 90f)
                            {
                                m_lockedBodyRotation += (Mathf.Abs(l_delta) - 90f) * Mathf.Sign(l_delta);
                                m_lockedBodyRotation = (m_lockedBodyRotation + 360f) % 360f;
                            }
                        }

                        var l_quat = l_animatorTransform.rotation;
                        var l_vec = l_quat.eulerAngles;
                        l_vec.y = m_lockedBodyRotation;
                        l_quat.eulerAngles = l_vec;
                        l_animatorTransform.rotation = l_quat;
                    }
                }
            }
        }

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        System.Collections.IEnumerator FindInputController()
        {
            while(m_player.GetComponent<GamelikeInputController>() == null)
                yield return null;
            m_inputController = m_player.GetComponent<GamelikeInputController>();
        }
    }
}
