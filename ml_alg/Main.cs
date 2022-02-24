using UnityEngine;

namespace ml_alg
{
    public class AvatarLimbsGrabber : MelonLoader.MelonMod
    {
        bool m_quit = false;

        object m_menuSettings = null;
        object m_menuLabelWorld = null;
        object m_buttonPlayerAllow = null;

        bool m_update = false;
        LiftedPlayer m_localLiftedPlayer = null;
        VRC.Player m_selectedPlayer = null;

        public override void OnApplicationStart()
        {
            MethodsResolver.ResolveMethods();
            Settings.LoadSettings();

            VRChatUtilityKit.Utilities.VRCUtils.OnUiManagerInit += this.OnUiManagerInit;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomJoined += this.OnJoinedRoom;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomLeft += this.OnLeftRoom;
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

                if(m_update && (m_localLiftedPlayer != null) && Settings.IsAnyEntryUpdated())
                {
                    if(Settings.IsFriendsEntryUpdated())
                    {
                        // Remove or add component on friends 
                        foreach(VRC.Player l_remotePlayer in Utils.GetFriendsInInstance())
                        {
                            LifterPlayer l_component = l_remotePlayer.GetComponent<LifterPlayer>();
                            if(l_component != null)
                            {
                                if(!Settings.AllowFriends)
                                {
                                    m_localLiftedPlayer.UnassignRemoteLifter(l_component);
                                    Object.Destroy(l_component);
                                }
                            }
                            else
                            {
                                if(Settings.AllowFriends)
                                {
                                    l_component = l_remotePlayer.gameObject.AddComponent<LifterPlayer>();
                                    l_component.AddLifted(m_localLiftedPlayer);
                                }
                            }
                        }
                    }

                    m_localLiftedPlayer.AllowPull = (Settings.AllowPull && VRChatUtilityKit.Utilities.VRCUtils.AreRiskyFunctionsAllowed);
                    m_localLiftedPlayer.AllowHandsPull = Settings.AllowHandsPull;
                    m_localLiftedPlayer.AllowHipsPull = Settings.AllowHipsPull;
                    m_localLiftedPlayer.AllowLegsPull = Settings.AllowLegsPull;
                    m_localLiftedPlayer.GrabDistance = Settings.GrabDistance;
                    m_localLiftedPlayer.SavePose = Settings.SavePose;
                    m_localLiftedPlayer.UseVelocity = Settings.UseVelocity;
                    m_localLiftedPlayer.VelocityMultiplier = Settings.VelocityMultiplier;
                    m_localLiftedPlayer.AverageVelocity = Settings.UseAverageVelocity;
                    m_localLiftedPlayer.ReapplyPermissions();
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
                    ((UIExpansionKit.API.Controls.IMenuToggle)m_buttonPlayerAllow).Selected = (m_selectedPlayer.GetComponent<LifterPlayer>() != null);
                }
            }
        }

        void OnUiManagerInit()
        {
            m_menuSettings = UIExpansionKit.API.ExpansionKitApi.CreateCustomQuickMenuPage(UIExpansionKit.API.LayoutDescription.WideSlimList);
            m_menuLabelWorld = ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).AddLabel("");
            ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).AddSimpleButton("Reset manipulated pose", this.OnPoseReset);
            ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).AddSimpleButton("Disallow manipulation for everyone in room", this.OnDisallowAll);
            ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).AddSimpleButton("Close", this.OnMenuClose);
            UIExpansionKit.API.ExpansionKitApi.GetExpandedMenu(UIExpansionKit.API.ExpandedMenu.QuickMenu).AddSimpleButton("Avatar limbs grabber", this.OnMenuShow);

            m_buttonPlayerAllow = UIExpansionKit.API.ExpansionKitApi.GetExpandedMenu(UIExpansionKit.API.ExpandedMenu.UserQuickMenu).AddToggleButton("Limbs manipulation", this.OnManipulationToggle, null);
        }

        void OnJoinedRoom()
        {
            m_update = true;
            MelonLoader.MelonCoroutines.Start(CreateLocalLifted());
        }
        System.Collections.IEnumerator CreateLocalLifted()
        {
            while(Utils.GetLocalPlayer() == null)
                yield return null;

            m_localLiftedPlayer = Utils.GetLocalPlayer().gameObject.AddComponent<LiftedPlayer>();
            m_localLiftedPlayer.AllowPull = (Settings.AllowPull && VRChatUtilityKit.Utilities.VRCUtils.AreRiskyFunctionsAllowed);
            m_localLiftedPlayer.AllowHandsPull = Settings.AllowHandsPull;
            m_localLiftedPlayer.AllowHipsPull = Settings.AllowHipsPull;
            m_localLiftedPlayer.AllowLegsPull = Settings.AllowLegsPull;
            m_localLiftedPlayer.GrabDistance = Settings.GrabDistance;
            m_localLiftedPlayer.SavePose = Settings.SavePose;
            m_localLiftedPlayer.UseVelocity = Settings.UseVelocity;
            m_localLiftedPlayer.VelocityMultiplier = Settings.VelocityMultiplier;
            m_localLiftedPlayer.AverageVelocity = Settings.UseAverageVelocity;

            ((UIExpansionKit.API.Controls.IMenuLabel)m_menuLabelWorld).Text = "World pull permission: <color=#" + (VRChatUtilityKit.Utilities.VRCUtils.AreRiskyFunctionsAllowed ? "00FF00>Allowed" : "FF0000>Disallowed") + "</color>";
        }

        void OnLeftRoom()
        {
            m_update = false;
            m_localLiftedPlayer = null;
        }

        void OnPlayerJoined(VRC.Player p_player)
        {
            if(Settings.AllowFriends && Utils.IsFriend(p_player))
                MelonLoader.MelonCoroutines.Start(CreateLifterOnJoin(p_player));
        }
        System.Collections.IEnumerator CreateLifterOnJoin(VRC.Player p_player)
        {
            while(m_localLiftedPlayer == null)
                yield return null;

            LifterPlayer l_component = p_player.gameObject.AddComponent<LifterPlayer>();
            l_component.AddLifted(m_localLiftedPlayer);
        }

        void OnPlayerLeft(VRC.Player p_player)
        {
            LifterPlayer l_component = p_player.GetComponent<LifterPlayer>();
            if((l_component != null) && (m_localLiftedPlayer != null))
                m_localLiftedPlayer.UnassignRemoteLifter(l_component);
        }

        void OnFriended(VRC.Core.APIUser p_apiPlayer)
        {
            if(m_update && (m_localLiftedPlayer != null) && Settings.AllowFriends)
            {
                VRC.Player l_remotePlayer = Utils.GetPlayerWithId(p_apiPlayer.id);
                if(l_remotePlayer != null)
                {
                    LifterPlayer l_component = l_remotePlayer.GetComponent<LifterPlayer>();
                    if(l_component == null)
                    {
                        l_component = l_remotePlayer.gameObject.AddComponent<LifterPlayer>();
                        l_component.AddLifted(m_localLiftedPlayer);
                    }
                }
            }
        }

        void OnUnfriended(string p_id)
        {
            if(m_update && (m_localLiftedPlayer != null) && Settings.AllowFriends)
            {
                VRC.Player l_player = Utils.GetPlayerWithId(p_id);
                if(l_player != null)
                {
                    LifterPlayer l_component = l_player.GetComponent<LifterPlayer>();
                    if(l_component != null)
                    {
                        m_localLiftedPlayer.UnassignRemoteLifter(l_component);
                        Object.Destroy(l_component);
                    }
                }
            }
        }

        void OnManipulationToggle(bool p_state)
        {
            if(m_update && (m_localLiftedPlayer != null))
            {
                VRC.Player l_remotePlayer = Utils.GetPlayerQM();
                if(l_remotePlayer != null)
                {
                    LifterPlayer l_component = l_remotePlayer.GetComponent<LifterPlayer>();
                    if((l_component == null) && p_state)
                    {
                        l_component = l_remotePlayer.gameObject.AddComponent<LifterPlayer>();
                        l_component.AddLifted(m_localLiftedPlayer);
                    }
                    else if((l_component != null) && !p_state)
                    {
                        l_component.RemoveLifted(m_localLiftedPlayer);
                        m_localLiftedPlayer.UnassignRemoteLifter(l_component);
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

        void OnPoseReset()
        {
            if(m_update && (m_localLiftedPlayer != null) && Settings.SavePose)
                m_localLiftedPlayer.ClearSavedPose();
        }

        void OnDisallowAll()
        {
            if(m_update && (m_localLiftedPlayer != null))
            {
                var l_remotePlayers = Utils.GetPlayers();
                if(l_remotePlayers != null)
                {
                    foreach(VRC.Player l_remotePlayer in l_remotePlayers)
                    {
                        if(l_remotePlayer != null)
                        {
                            LifterPlayer l_component = l_remotePlayer.GetComponent<LifterPlayer>();
                            if(l_component != null)
                            {
                                m_localLiftedPlayer.UnassignRemoteLifter(l_component);
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
    }
}
