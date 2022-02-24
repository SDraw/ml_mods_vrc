using UnityEngine;

namespace ml_abp
{
    public class AvatarBonesProximity : MelonLoader.MelonMod
    {
        bool m_quit = false;

        object m_menuSettings = null;
        object m_buttonPlayerAllow = null;

        bool m_update = false;
        InteractedPlayer m_localInteracted = null;
        VRC.Player m_selectedPlayer = null;

        public override void OnApplicationStart()
        {
            MethodsResolver.ResolveMethods();
            Settings.LoadSettings();

            VRChatUtilityKit.Utilities.VRCUtils.OnUiManagerInit += this.OnUiManagerInit;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomJoined += this.OnRoomJoined;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomLeft += this.OnRoomLeft;
            VRChatUtilityKit.Utilities.NetworkEvents.OnPlayerJoined += this.OnPlayerJoined;
            VRChatUtilityKit.Utilities.NetworkEvents.OnPlayerLeft += this.OnPlayerLeft;
            VRChatUtilityKit.Utilities.NetworkEvents.OnFriended += this.OnFriended;
            VRChatUtilityKit.Utilities.NetworkEvents.OnUnfriended += this.OnUnfriended;
        }

        public override void OnApplicationQuit()
        {
            m_quit = true;
        }

        public override void OnPreferencesSaved()
        {
            if(!m_quit) // This is not a joke
            {
                Settings.ReloadSettings();

                if(m_update && (m_localInteracted != null) && Settings.IsAnyEntryUpdated())
                {
                    if(Settings.IsFriendsEntryUpdated())
                    {
                        // Remove or add component on friends 
                        foreach(VRC.Player l_remotePlayer in Utils.GetFriendsInInstance())
                        {
                            InteracterPlayer l_component = l_remotePlayer.GetComponent<InteracterPlayer>();
                            if(l_component != null)
                            {
                                if(!Settings.AllowFriends)
                                {
                                    m_localInteracted.RemoveInteracter(l_component);
                                    Object.Destroy(l_component);
                                }
                            }
                            else
                            {
                                if(Settings.AllowFriends)
                                {
                                    l_component = l_remotePlayer.gameObject.AddComponent<InteracterPlayer>();
                                    m_localInteracted.AddInteracter(l_component);
                                }
                            }
                        }
                    }

                    m_localInteracted.BonesProximity = Settings.ProximityDistance;
                    m_localInteracted.PlayersProximity = Settings.PlayersDistance;
                    m_localInteracted.UseCustomTargets = Settings.CustomTargets;
                }
            }
        }

        public override void OnUpdate()
        {
            if(m_update && ((UIExpansionKit.API.Controls.IMenuToggle)m_buttonPlayerAllow).Visible)
            {
                VRC.Player l_selectedPlayer = Utils.GetPlayerQM();
                if((l_selectedPlayer != null) && (m_selectedPlayer != l_selectedPlayer))
                {
                    m_selectedPlayer = l_selectedPlayer;
                    ((UIExpansionKit.API.Controls.IMenuToggle)m_buttonPlayerAllow).Selected = (m_selectedPlayer.GetComponent<InteracterPlayer>() != null);
                }
            }
        }

        void OnUiManagerInit()
        {
            m_menuSettings = UIExpansionKit.API.ExpansionKitApi.CreateCustomQuickMenuPage(UIExpansionKit.API.LayoutDescription.WideSlimList);
            ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).AddSimpleButton("Disable bones proximity for everyone in room", this.OnDisableAll);
            ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).AddSimpleButton("Close", this.OnMenuClose);
            UIExpansionKit.API.ExpansionKitApi.GetExpandedMenu(UIExpansionKit.API.ExpandedMenu.QuickMenu).AddSimpleButton("Avatar bones proximity", this.OnMenuShow);

            m_buttonPlayerAllow = UIExpansionKit.API.ExpansionKitApi.GetExpandedMenu(UIExpansionKit.API.ExpandedMenu.UserQuickMenu).AddToggleButton("Bones proximity", this.OnProximityToggle, null);
        }

        void OnRoomJoined()
        {
            m_update = true;
            MelonLoader.MelonCoroutines.Start(CreateLocalInteracted());
        }
        System.Collections.IEnumerator CreateLocalInteracted()
        {
            while(Utils.GetLocalPlayer() == null)
                yield return null;
            m_localInteracted = Utils.GetLocalPlayer().gameObject.AddComponent<InteractedPlayer>();
            m_localInteracted.BonesProximity = Settings.ProximityDistance;
            m_localInteracted.PlayersProximity = Settings.PlayersDistance;
            m_localInteracted.UseCustomTargets = Settings.CustomTargets;
        }

        void OnRoomLeft()
        {
            m_update = false;
            m_localInteracted = null;
        }

        void OnPlayerJoined(VRC.Player p_player)
        {
            if(Settings.AllowFriends && Utils.IsFriend(p_player))
            {
                MelonLoader.MelonCoroutines.Start(CreateInteracterOnJoin(p_player));
            }
        }
        System.Collections.IEnumerator CreateInteracterOnJoin(VRC.Player p_player)
        {
            while(m_localInteracted == null)
                yield return null;
            InteracterPlayer l_component = p_player.gameObject.AddComponent<InteracterPlayer>();
            m_localInteracted.AddInteracter(l_component);
        }

        void OnPlayerLeft(VRC.Player p_player)
        {
            InteracterPlayer l_component = p_player.GetComponent<InteracterPlayer>();
            if((l_component != null) && (m_localInteracted != null))
                m_localInteracted.RemoveInteracter(l_component);
        }

        void OnFriended(VRC.Core.APIUser p_apiPlayer)
        {
            if(m_update && (m_localInteracted != null) && Settings.AllowFriends)
            {
                VRC.Player l_remotePlayer = Utils.GetPlayerWithId(p_apiPlayer.id);
                if(l_remotePlayer != null)
                {
                    InteracterPlayer l_component = l_remotePlayer.GetComponent<InteracterPlayer>();
                    if(l_component == null)
                    {
                        l_component = l_remotePlayer.gameObject.AddComponent<InteracterPlayer>();
                        m_localInteracted.AddInteracter(l_component);
                    }
                }
            }
        }

        void OnUnfriended(string p_id)
        {
            if(m_update && (m_localInteracted != null) && Settings.AllowFriends)
            {
                VRC.Player l_remotePlayer = Utils.GetPlayerWithId(p_id);
                if(l_remotePlayer != null)
                {
                    InteracterPlayer l_component = l_remotePlayer.GetComponent<InteracterPlayer>();
                    if(l_component != null)
                    {
                        m_localInteracted.RemoveInteracter(l_component);
                        Object.Destroy(l_component);
                    }
                }
            }
        }

        void OnMenuShow()
        {
            if(m_update && (m_menuSettings != null))
                ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).Show();
        }

        void OnDisableAll()
        {
            if(m_update && (m_localInteracted != null))
            {
                var l_remotePlayers = Utils.GetPlayers();
                if(l_remotePlayers != null)
                {
                    foreach(VRC.Player l_remotePlayer in l_remotePlayers)
                    {
                        if(l_remotePlayer != null)
                        {
                            InteracterPlayer l_component = l_remotePlayer.GetComponent<InteracterPlayer>();
                            if(l_component != null)
                            {
                                m_localInteracted.RemoveInteracter(l_component);
                                Object.Destroy(l_component);
                            }
                        }
                    }
                }
            }
        }

        void OnMenuClose()
        {
            if(m_update && (m_menuSettings != null))
                ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).Hide();
        }

        void OnProximityToggle(bool p_state)
        {
            if(m_update && (m_localInteracted != null))
            {
                VRC.Player l_remotePlayer = Utils.GetPlayerQM();
                if(l_remotePlayer != null)
                {
                    InteracterPlayer l_component = l_remotePlayer.GetComponent<InteracterPlayer>();
                    if((l_component == null) && p_state)
                    {
                        l_component = l_remotePlayer.gameObject.AddComponent<InteracterPlayer>();
                        m_localInteracted.AddInteracter(l_component);
                    }
                    else if((l_component != null) && !p_state)
                    {
                        m_localInteracted.RemoveInteracter(l_component);
                        Object.Destroy(l_component);
                    }
                }
            }
        }
    }
}
