using UnityEngine;

namespace ml_alg
{
    public class Main : MelonLoader.MelonMod
    {
        bool m_quit = false;

        LiftedPlayer m_localLiftedPlayer = null;
        UnityEngine.UI.Text m_textComponent = null;
        UIExpansionKit.API.ICustomShowableLayoutedMenu m_menuSettings = null;

        bool m_update = false;
        bool m_buttonVisibility = false;
        VRC.Player m_currentSelectedPlayer = null;

        public override void OnApplicationStart()
        {
            IKTweaksHelper.ResolveTypes();
            Settings.LoadSettings();

            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomJoined += this.OnJoinedRoom;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomLeft += this.OnLeftRoom;
            VRChatUtilityKit.Utilities.NetworkEvents.OnPlayerJoined += this.OnPlayerJoined;
            VRChatUtilityKit.Utilities.NetworkEvents.OnPlayerLeft += this.OnPlayerLeft;
            VRChatUtilityKit.Utilities.NetworkEvents.OnFriended += this.OnFriended;
            VRChatUtilityKit.Utilities.NetworkEvents.OnUnfriended += this.OnUnfriended;
            VRChatUtilityKit.Utilities.NetworkEvents.OnAvatarInstantiated += this.OnAvatarInstantiated;

            m_menuSettings = UIExpansionKit.API.ExpansionKitApi.CreateCustomQuickMenuPage(UIExpansionKit.API.LayoutDescription.WideSlimList);
            m_menuSettings.AddLabel("World pull permission:",
                delegate (GameObject f_obj)
                {
                    var l_worldText = f_obj.GetComponentInChildren<UnityEngine.UI.Text>();
                    if(l_worldText != null)
                        l_worldText.text = "World pull permission: <color=#" + (VRChatUtilityKit.Utilities.VRCUtils.AreRiskyFunctionsAllowed ? "00FF00>Allowed" : "FF0000>Disallowed") + "</color>";
                }
            );
            m_menuSettings.AddSimpleButton("Reset manipulated pose", this.OnPoseReset);
            m_menuSettings.AddSimpleButton("Disallow manipulation for everyone in room", this.OnDisallowAll);
            m_menuSettings.AddSimpleButton("Close", this.OnMenuClose);
            UIExpansionKit.API.ExpansionKitApi.GetExpandedMenu(UIExpansionKit.API.ExpandedMenu.QuickMenu).AddSimpleButton("Avatar limbs grabber", this.OnMenuShow);

            UIExpansionKit.API.ExpansionKitApi.GetExpandedMenu(UIExpansionKit.API.ExpandedMenu.UserQuickMenu).AddSimpleButton("Allow limbs manipulation", this.OnManipulationAllow,
                delegate (GameObject f_obj)
                {
                    m_textComponent = f_obj.GetComponentInChildren<UnityEngine.UI.Text>();

                    var l_listener = f_obj.AddComponent<UIExpansionKit.Components.EnableDisableListener>();
                    l_listener.OnEnabled += this.OnAllowButtonShown;
                    l_listener.OnDisabled += this.OnAllowButtonHidden;
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

                if(m_update && (m_localLiftedPlayer != null))
                {
                    // Remove or add component on friends 
                    foreach(var l_remotePlayer in Utils.GetFriendsInInstance())
                    {
                        var l_component = l_remotePlayer.GetComponent<LifterPlayer>();
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

                    m_localLiftedPlayer.AllowPull = (Settings.AllowPull && VRChatUtilityKit.Utilities.VRCUtils.AreRiskyFunctionsAllowed);
                    m_localLiftedPlayer.AllowHandsPull = Settings.AllowHandsPull;
                    m_localLiftedPlayer.AllowHipsPull = Settings.AllowHipsPull;
                    m_localLiftedPlayer.AllowLegsPull = Settings.AllowLegsPull;
                    m_localLiftedPlayer.GrabDistance = Settings.GrabDistance;
                    m_localLiftedPlayer.RotateHips = Settings.UseHipsRotation;
                    m_localLiftedPlayer.RotateLegs = Settings.UseLegsRotation;
                    m_localLiftedPlayer.RotateHands = Settings.UseHandsRotation;
                    m_localLiftedPlayer.SavePose = Settings.SavePose;
                    m_localLiftedPlayer.UseVelocity = Settings.UseVelocity;
                    m_localLiftedPlayer.VelocityMultiplier = Settings.VelocityMultiplier;
                    m_localLiftedPlayer.AverageVelocity = Settings.UseAverageVelocity;
                    m_localLiftedPlayer.UseIKTweaks = Settings.UseIKTweaks;
                    m_localLiftedPlayer.ReapplyPermissions();
                }
            }
        }

        public override void OnUpdate()
        {
            if(m_update && m_buttonVisibility)
            {
                var l_selectedPlayer = Utils.GetQuickMenuSelectedPlayer();
                if((l_selectedPlayer != null) && (m_currentSelectedPlayer != l_selectedPlayer))
                {
                    m_currentSelectedPlayer = l_selectedPlayer;
                    var l_component = m_currentSelectedPlayer.GetComponent<LifterPlayer>();
                    m_textComponent.color = (l_component != null) ? Color.green : Color.white;
                }
            }
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
            m_localLiftedPlayer.RotateHips = Settings.UseHipsRotation;
            m_localLiftedPlayer.RotateLegs = Settings.UseLegsRotation;
            m_localLiftedPlayer.RotateHands = Settings.UseHandsRotation;
            m_localLiftedPlayer.SavePose = Settings.SavePose;
            m_localLiftedPlayer.UseVelocity = Settings.UseVelocity;
            m_localLiftedPlayer.VelocityMultiplier = Settings.VelocityMultiplier;
            m_localLiftedPlayer.AverageVelocity = Settings.UseAverageVelocity;
            m_localLiftedPlayer.UseIKTweaks = Settings.UseIKTweaks;
        }

        void OnLeftRoom()
        {
            m_update = false;
            m_localLiftedPlayer = null;
        }

        void OnPlayerJoined(VRC.Player f_player)
        {
            if(Settings.AllowFriends && Utils.IsFriend(f_player))
                MelonLoader.MelonCoroutines.Start(CreateLifterOnJoin(f_player));
        }
        System.Collections.IEnumerator CreateLifterOnJoin(VRC.Player f_player)
        {
            while(m_localLiftedPlayer == null)
                yield return null;
            var l_component = f_player.gameObject.AddComponent<LifterPlayer>();
            l_component.AddLifted(m_localLiftedPlayer);
        }

        void OnPlayerLeft(VRC.Player f_player)
        {
            var l_component = f_player.GetComponent<LifterPlayer>();
            if((l_component != null) && (m_localLiftedPlayer != null))
                m_localLiftedPlayer.UnassignRemoteLifter(l_component);
        }

        void OnFriended(VRC.Core.APIUser f_apiPlayer)
        {
            if(m_update && (m_localLiftedPlayer != null) && Settings.AllowFriends)
            {
                var l_remotePlayer = Utils.GetPlayerWithId(f_apiPlayer.id);
                if(l_remotePlayer != null)
                {
                    var l_component = l_remotePlayer.GetComponent<LifterPlayer>();
                    if(l_component == null)
                    {
                        l_component = l_remotePlayer.gameObject.AddComponent<LifterPlayer>();
                        l_component.AddLifted(m_localLiftedPlayer);
                    }
                }
            }
        }

        void OnUnfriended(string f_id)
        {
            if(m_update && (m_localLiftedPlayer != null) && Settings.AllowFriends)
            {
                var l_player = Utils.GetPlayerWithId(f_id);
                if(l_player != null)
                {
                    var l_component = l_player.GetComponent<LifterPlayer>();
                    if(l_component != null)
                    {
                        m_localLiftedPlayer.UnassignRemoteLifter(l_component);
                        Object.Destroy(l_component);
                    }
                }
            }
        }

        void OnAvatarInstantiated(VRCAvatarManager f_avatarManager, VRC.Core.ApiAvatar f_apiAvatar, GameObject f_avatarObject)
        {
            var l_playerObject = f_avatarObject.transform.root;
            if(l_playerObject != null)
            {
                var l_lifted = l_playerObject.GetComponent<LiftedPlayer>();
                if(l_lifted != null)
                    l_lifted.RecacheComponents();

                var l_lifter = l_playerObject.GetComponent<LifterPlayer>();
                if(l_lifter != null)
                    l_lifter.RecacheComponents();
            }
        }

        void OnManipulationAllow()
        {
            if(m_update && (m_localLiftedPlayer != null))
            {
                var l_remotePlayer = Utils.GetQuickMenuSelectedPlayer();
                if(l_remotePlayer != null)
                {
                    var l_component = l_remotePlayer.GetComponent<LifterPlayer>();
                    if(l_component == null)
                    {
                        l_component = l_remotePlayer.gameObject.AddComponent<LifterPlayer>();
                        l_component.AddLifted(m_localLiftedPlayer);

                        m_textComponent.color = Color.green;
                    }
                    else
                    {
                        l_component.RemoveLifted(m_localLiftedPlayer);
                        m_localLiftedPlayer.UnassignRemoteLifter(l_component);
                        Object.Destroy(l_component);
                        m_textComponent.color = Color.white;
                    }
                }
            }
        }

        void OnMenuShow()
        {
            if(m_update && (m_menuSettings != null))
                m_menuSettings.Show();
        }

        void OnPoseReset()
        {
            if(m_update && (m_localLiftedPlayer != null) && Settings.SavePose)
                m_localLiftedPlayer.ClearSavedLiftedBones();
        }

        void OnDisallowAll()
        {
            if(m_update && (m_localLiftedPlayer != null))
            {
                var l_remotePlayers = Utils.GetPlayers();
                if(l_remotePlayers != null)
                {
                    foreach(var l_remotePlayer in l_remotePlayers)
                    {
                        if(l_remotePlayer != null)
                        {
                            var l_component = l_remotePlayer.GetComponent<LifterPlayer>();
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
                m_menuSettings.Hide();
        }

        void OnAllowButtonShown()
        {
            m_buttonVisibility = true;

            var l_remotePlayer = Utils.GetQuickMenuSelectedPlayer();
            if(l_remotePlayer != null)
            {
                m_currentSelectedPlayer = l_remotePlayer;

                var l_component = l_remotePlayer.GetComponent<LifterPlayer>();
                m_textComponent.color = (l_component != null) ? Color.green : Color.white;
            }
        }

        void OnAllowButtonHidden()
        {
            m_buttonVisibility = false;
            m_currentSelectedPlayer = null;
        }
    }
}
