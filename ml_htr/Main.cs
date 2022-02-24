namespace ml_htr
{
    public class HipTrackerRotator : MelonLoader.MelonMod
    {
        object m_menuSettings = null;

        public override void OnApplicationStart()
        {
            VRChatUtilityKit.Utilities.VRCUtils.OnUiManagerInit += this.OnUiManagerInit;
        }

        void OnUiManagerInit()
        {
            m_menuSettings = UIExpansionKit.API.ExpansionKitApi.CreateCustomQuickMenuPage(UIExpansionKit.API.LayoutDescription.WideSlimList);
            ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).AddSimpleButton("Rotate hip offset +5 degrees around X axis ", () => this.OnRotate(5f, 0f, 0f));
            ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).AddSimpleButton("Rotate hip offset -5 degrees around X axis", () => this.OnRotate(-5f, 0f, 0f));
            ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).AddSimpleButton("Rotate hip offset +5 degrees around Y axis ", () => this.OnRotate(0f, 5f, 0f));
            ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).AddSimpleButton("Rotate hip offset -5 degrees around Y axis", () => this.OnRotate(0f, -5f, 0f));
            ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).AddSimpleButton("Rotate hip offset +5 degrees around Z axis ", () => this.OnRotate(0f, 0f, 5f));
            ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).AddSimpleButton("Rotate hip offset -5 degrees around Z axis", () => this.OnRotate(0f, 0f, -5f));
            ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).AddSimpleButton("Close", this.OnMenuClose);

            UIExpansionKit.API.ExpansionKitApi.GetExpandedMenu(UIExpansionKit.API.ExpandedMenu.QuickMenu).AddSimpleButton("Hips tracker rotator", this.OnMenuShow);
        }

        void OnMenuShow()
        {
            if(m_menuSettings != null)
                ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).Show();
        }

        void OnMenuClose()
        {
            if(m_menuSettings != null)
                ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).Hide();
        }

        void OnRotate(float p_axisX, float p_axisY, float p_axisZ)
        {
            UnityEngine.Transform l_puckOffset = Utils.GetVRCTrackingSteam().field_Public_Transform_10;
            if(l_puckOffset != null)
            {
                UnityEngine.Quaternion l_localRotation = l_puckOffset.localRotation;
                l_localRotation = l_localRotation * UnityEngine.Quaternion.Euler(p_axisX, p_axisY, p_axisZ);
                l_puckOffset.localRotation = l_localRotation;
            }
        }
    }
}
