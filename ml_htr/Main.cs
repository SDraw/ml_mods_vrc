namespace ml_htr
{
    public class Main : MelonLoader.MelonMod
    {
        UIExpansionKit.API.ICustomShowableLayoutedMenu m_menuSettings = null;

        public override void OnApplicationStart()
        {
            m_menuSettings = UIExpansionKit.API.ExpansionKitApi.CreateCustomQuickMenuPage(UIExpansionKit.API.LayoutDescription.WideSlimList);
            m_menuSettings.AddSimpleButton("Rotate hip offset +5 degrees around X axis ", () => this.OnRotate(5f,0f,0f));
            m_menuSettings.AddSimpleButton("Rotate hip offset -5 degrees around X axis", () => this.OnRotate(-5f, 0f, 0f));
            m_menuSettings.AddSimpleButton("Rotate hip offset +5 degrees around Y axis ", () => this.OnRotate(0f, 5f, 0f));
            m_menuSettings.AddSimpleButton("Rotate hip offset -5 degrees around Y axis", () => this.OnRotate(0f, -5f, 0f));
            m_menuSettings.AddSimpleButton("Rotate hip offset +5 degrees around Z axis ", () => this.OnRotate(0f, 0f, 5f));
            m_menuSettings.AddSimpleButton("Rotate hip offset -5 degrees around Z axis", () => this.OnRotate(0f, 0f, -5f));
            m_menuSettings.AddSimpleButton("Close", this.OnMenuClose);

            UIExpansionKit.API.ExpansionKitApi.GetExpandedMenu(UIExpansionKit.API.ExpandedMenu.QuickMenu).AddSimpleButton("Hips tracker rotator", this.OnMenuShow);
        }

        void OnMenuShow()
        {
            if(m_menuSettings != null)
                m_menuSettings.Show();
        }

        void OnMenuClose()
        {
            if(m_menuSettings != null)
                m_menuSettings.Hide();
        }

        void OnRotate(float f_axisX, float f_axisY, float f_axisZ)
        {
            var l_puckOffset = Utils.GetVRCTrackingSteam().field_Public_Transform_10;
            if(l_puckOffset != null)
            {
                var l_localRotation = l_puckOffset.localRotation;
                l_localRotation = l_localRotation * UnityEngine.Quaternion.Euler(f_axisX,f_axisY,f_axisZ);
                l_puckOffset.localRotation = l_localRotation;
            }
        }
    }
}
