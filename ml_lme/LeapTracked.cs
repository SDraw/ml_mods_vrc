using System;
using UnityEngine;

namespace ml_lme
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    class LeapTracked : MonoBehaviour
    {
        VRCPlayer m_player = null;
        readonly VRCPlayer.OnAvatarIsReady m_readyAvatarEvent = null;
        HandGestureController m_handGestureController = null;

        bool m_enabled = false;

        public LeapTracked(IntPtr ptr) : base(ptr)
        {
            m_readyAvatarEvent = new Action(this.RecacheComponents);
        }

        void Awake()
        {
            m_player = this.GetComponent<VRCPlayer>();

            m_player.field_Private_OnAvatarIsReady_0 += m_readyAvatarEvent;
        }

        void OnDestroy()
        {
            if(m_player != null)
                m_player.field_Private_OnAvatarIsReady_0 -= m_readyAvatarEvent;
        }

        public void SetEnabled(bool p_state) => m_enabled = p_state;

        [UnhollowerBaseLib.Attributes.HideFromIl2Cpp]
        public void UpdateFromGestures(GestureMatcher.GesturesData p_gesturesData)
        {
            if(m_enabled && (m_handGestureController != null))
            {
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
            m_handGestureController = null;

            m_handGestureController = m_player.field_Private_VRC_AnimationController_0.field_Private_HandGestureController_0;

            ReapplyTracking();
        }

        public void ReapplyTracking()
        {
            if(m_handGestureController != null)
            {
                m_handGestureController.field_Private_InputMethod_0 = ((m_enabled && !Utils.IsInVRMode()) ? VRCInputManager.InputMethod.Index : VRCInputManager.InputMethod.Count);
                HandGestureController.field_Private_Static_Boolean_1 = ((m_enabled && !Utils.IsInVRMode()) ? false : Utils.GetGesturesToggle());
            }
        }

        public void ForceIndexTracking(HandGestureController p_controller)
        {
            if(m_handGestureController == p_controller)
            {
                // Spaceballs!
                m_handGestureController.field_Private_InputMethod_0 = VRCInputManager.InputMethod.Index;
                VRC.PoseRecorder.field_Internal_Static_Int32_0 |= (int)0x200u;
            }
        }

        public void ForceDesktopTracking(HandGestureController p_controller)
        {
            if(m_handGestureController == p_controller)
            {
                // Spaceballs!
                m_handGestureController.field_Private_InputMethod_0 = VRCInputManager.InputMethod.Count;
            }
        }
    }
}
