using UnityEngine;

namespace ml_abp
{
    public class Main : MelonLoader.MelonMod
    {
        bool m_quit = false;

        InteractedPlayer m_localInteracted = null;

        UnityEngine.UI.Text m_textComponent = null;
        UIExpansionKit.API.ICustomShowableLayoutedMenu m_menuSettings = null;

        bool m_update = false;
        bool m_toggleVisibility = false;
        VRC.Player m_currentSelectedPlayer = null;

        public override void OnApplicationStart()
        {
            MethodsResolver.ResolveMethods();
            Settings.LoadSettings();

            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomJoined += this.OnRoomJoined;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomLeft += this.OnRoomLeft;
            VRChatUtilityKit.Utilities.NetworkEvents.OnPlayerJoined += this.OnPlayerJoined;
            VRChatUtilityKit.Utilities.NetworkEvents.OnPlayerLeft += this.OnPlayerLeft;
            VRChatUtilityKit.Utilities.NetworkEvents.OnFriended += this.OnFriended;
            VRChatUtilityKit.Utilities.NetworkEvents.OnUnfriended += this.OnUnfriended;
            VRChatUtilityKit.Utilities.NetworkEvents.OnAvatarInstantiated += this.OnAvatarInstantiated;

            m_menuSettings = UIExpansionKit.API.ExpansionKitApi.CreateCustomQuickMenuPage(UIExpansionKit.API.LayoutDescription.WideSlimList);
            m_menuSettings.AddSimpleButton("Disable bones proximity from everyone in room", this.OnDisableAll);
            m_menuSettings.AddSimpleButton("Close", this.OnMenuClose);
            UIExpansionKit.API.ExpansionKitApi.GetExpandedMenu(UIExpansionKit.API.ExpandedMenu.QuickMenu).AddSimpleButton("Avatar bones proximity", this.OnMenuShow);

            UIExpansionKit.API.ExpansionKitApi.GetExpandedMenu(UIExpansionKit.API.ExpandedMenu.UserQuickMenu).AddSimpleButton("Toggle bones proximity", this.OnProximityToggle,
                delegate (GameObject f_obj)
                {
                    m_textComponent = f_obj.GetComponentInChildren<UnityEngine.UI.Text>();

                    var l_listener = f_obj.AddComponent<UIExpansionKit.Components.EnableDisableListener>();
                    l_listener.OnEnabled += this.OnProximityToggleShown;
                    l_listener.OnDisabled += this.OnProximityToggleHidden;
                }
            );
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

                if(m_update && (m_localInteracted != null))
                {
                    // Remove or add component on friends 
                    foreach(var l_remotePlayer in Utils.GetFriendsInInstance())
                    {
                        var l_component = l_remotePlayer.GetComponent<InteracterPlayer>();
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

                    m_localInteracted.BonesProximity = Settings.ProximityDistance;
                    m_localInteracted.PlayersProximity = Settings.PlayersDistance;
                    m_localInteracted.UseCustomTargets = Settings.CustomTargets;
                }
            }
        }

        public override void OnUpdate()
        {
            if(m_update && m_toggleVisibility)
            {
                var l_selectedPlayer = Utils.GetPlayerQM();
                if((l_selectedPlayer != null) && (m_currentSelectedPlayer != l_selectedPlayer))
                {
                    m_currentSelectedPlayer = l_selectedPlayer;

                    var l_component = m_currentSelectedPlayer.GetComponent<InteracterPlayer>();
                    m_textComponent.color = (l_component != null) ? Color.green : Color.white;
                }
            }
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

        void OnPlayerJoined(VRC.Player f_player)
        {
            if(Utils.IsFriend(f_player) && Settings.AllowFriends)
            {
                MelonLoader.MelonCoroutines.Start(CreateInteracterOnJoin(f_player));
            }
        }
        System.Collections.IEnumerator CreateInteracterOnJoin(VRC.Player f_player)
        {
            while(m_localInteracted == null)
                yield return null;
            var l_component = f_player.gameObject.AddComponent<InteracterPlayer>();
            m_localInteracted.AddInteracter(l_component);
        }

        void OnPlayerLeft(VRC.Player f_player)
        {
            var l_component = f_player.GetComponent<InteracterPlayer>();
            if((l_component != null) && (m_localInteracted != null))
                m_localInteracted.RemoveInteracter(l_component);
        }

        void OnFriended(VRC.Core.APIUser f_apiPlayer)
        {
            if(m_update && (m_localInteracted != null) && Settings.AllowFriends)
            {
                var l_remotePlayer = Utils.GetPlayerWithId(f_apiPlayer.id);
                if(l_remotePlayer != null)
                {
                    var l_component = l_remotePlayer.GetComponent<InteracterPlayer>();
                    if(l_component == null)
                    {
                        l_component = l_remotePlayer.gameObject.AddComponent<InteracterPlayer>();
                        m_localInteracted.AddInteracter(l_component);
                    }
                }
            }
        }

        void OnUnfriended(string f_id)
        {
            if(m_update && (m_localInteracted != null) && Settings.AllowFriends)
            {
                var l_remotePlayer = Utils.GetPlayerWithId(f_id);
                if(l_remotePlayer != null)
                {
                    var l_component = l_remotePlayer.GetComponent<InteracterPlayer>();
                    if(l_component != null)
                    {
                        m_localInteracted.RemoveInteracter(l_component);
                        Object.Destroy(l_component);
                    }
                }
            }
        }

        void OnAvatarInstantiated(VRCAvatarManager f_manager, VRC.Core.ApiAvatar f_apiAvatar, GameObject f_avatarObject)
        {
            var l_player = f_avatarObject.transform.root.GetComponent<VRC.Player>();
            if((l_player != null) && (l_player == Utils.GetLocalPlayer()) && (m_localInteracted != null))
                m_localInteracted.ResetParameters();
        }

        void OnMenuShow()
        {
            if(m_update && (m_menuSettings != null))
                m_menuSettings.Show();
        }

        void OnDisableAll()
        {
            if(m_update && (m_localInteracted != null))
            {
                var l_remotePlayers = Utils.GetPlayers();
                if(l_remotePlayers != null)
                {
                    foreach(var l_remotePlayer in l_remotePlayers)
                    {
                        if(l_remotePlayer != null)
                        {
                            var l_component = l_remotePlayer.GetComponent<InteracterPlayer>();
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
                m_menuSettings.Hide();
        }

        void OnProximityToggle()
        {
            if(m_update && (m_localInteracted != null))
            {
                var l_remotePlayer = Utils.GetPlayerQM();
                if(l_remotePlayer != null)
                {
                    var l_component = l_remotePlayer.GetComponent<InteracterPlayer>();
                    if(l_component == null)
                    {
                        l_component = l_remotePlayer.gameObject.AddComponent<InteracterPlayer>();
                        m_localInteracted.AddInteracter(l_component);

                        m_textComponent.color = Color.green;
                    }
                    else
                    {
                        m_localInteracted.RemoveInteracter(l_component);
                        Object.Destroy(l_component);

                        m_textComponent.color = Color.white;
                    }
                }
            }
        }

        void OnProximityToggleShown()
        {
            m_toggleVisibility = true;

            var l_remotePlayer = Utils.GetPlayerQM();
            if(l_remotePlayer != null)
            {
                m_currentSelectedPlayer = l_remotePlayer;

                var l_component = l_remotePlayer.GetComponent<InteracterPlayer>();
                m_textComponent.color = (l_component != null) ? Color.green : Color.white;
            }
        }

        void OnProximityToggleHidden()
        {
            m_toggleVisibility = false;
            m_currentSelectedPlayer = null;
        }
    }
}
