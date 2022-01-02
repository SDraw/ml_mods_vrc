using UnityEngine;

namespace ml_alg
{
    public class AvatarLimbsGrabber : MelonLoader.MelonMod
    {
        static AvatarLimbsGrabber ms_instance = null;

        bool m_quit = false;

        LiftedPlayer m_localLiftedPlayer = null;
        UnityEngine.UI.Text m_textComponent = null;
        object m_menuSettings = null;

        bool m_update = false;
        bool m_buttonVisibility = false;
        VRC.Player m_currentSelectedPlayer = null;

        public override void OnApplicationStart()
        {
            ms_instance = this;

            MethodsResolver.ResolveMethods();
            IKTweaksHelper.Resolve();
            Settings.LoadSettings();

            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomJoined += this.OnJoinedRoom;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomLeft += this.OnLeftRoom;
            VRChatUtilityKit.Utilities.NetworkEvents.OnPlayerJoined += this.OnPlayerJoined;
            VRChatUtilityKit.Utilities.NetworkEvents.OnPlayerLeft += this.OnPlayerLeft;
            VRChatUtilityKit.Utilities.NetworkEvents.OnFriended += this.OnFriended;
            VRChatUtilityKit.Utilities.NetworkEvents.OnUnfriended += this.OnUnfriended;

            if(VRChatUtilityKit.Utilities.VRCUtils.IsUIXPresent)
            {
                m_menuSettings = UIExpansionKit.API.ExpansionKitApi.CreateCustomQuickMenuPage(UIExpansionKit.API.LayoutDescription.WideSlimList);
                ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).AddLabel("World pull permission:", (GameObject p_obj) =>
                {
                    UnityEngine.UI.Text l_worldText = p_obj.GetComponentInChildren<UnityEngine.UI.Text>();
                    if(l_worldText != null)
                        l_worldText.text = "World pull permission: <color=#" + (VRChatUtilityKit.Utilities.VRCUtils.AreRiskyFunctionsAllowed ? "00FF00>Allowed" : "FF0000>Disallowed") + "</color>";
                });
                ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).AddSimpleButton("Reset manipulated pose", this.OnPoseReset);
                ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).AddSimpleButton("Disallow manipulation for everyone in room", this.OnDisallowAll);
                ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).AddSimpleButton("Close", this.OnMenuClose);
                UIExpansionKit.API.ExpansionKitApi.GetExpandedMenu(UIExpansionKit.API.ExpandedMenu.QuickMenu).AddSimpleButton("Avatar limbs grabber", this.OnMenuShow);

                UIExpansionKit.API.ExpansionKitApi.GetExpandedMenu(UIExpansionKit.API.ExpandedMenu.UserQuickMenu).AddSimpleButton("Allow limbs manipulation", this.OnManipulationAllow,
                    delegate (GameObject p_obj)
                    {
                        m_textComponent = p_obj.GetComponentInChildren<UnityEngine.UI.Text>();

                        var l_listener = p_obj.AddComponent<UIExpansionKit.Components.EnableDisableListener>();
                        l_listener.OnEnabled += this.OnAllowButtonShown;
                        l_listener.OnDisabled += this.OnAllowButtonHidden;
                    }
                );
            }

            // Patches
            if(IKTweaksHelper.Present && (IKTweaksHelper.PreSetupVRIK != null))
                HarmonyInstance.Patch(IKTweaksHelper.PreSetupVRIK, null, new HarmonyLib.HarmonyMethod(typeof(AvatarLimbsGrabber).GetMethod(nameof(OnPreSetupVRIK_PostfixPatch), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)));
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
            if(m_update && m_buttonVisibility)
            {
                VRC.Player l_selectedPlayer = Utils.GetPlayerQM();
                if((l_selectedPlayer != null) && (m_currentSelectedPlayer != l_selectedPlayer))
                {
                    m_currentSelectedPlayer = l_selectedPlayer;
                    LifterPlayer l_component = m_currentSelectedPlayer.GetComponent<LifterPlayer>();
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
            m_localLiftedPlayer.SavePose = Settings.SavePose;
            m_localLiftedPlayer.UseVelocity = Settings.UseVelocity;
            m_localLiftedPlayer.VelocityMultiplier = Settings.VelocityMultiplier;
            m_localLiftedPlayer.AverageVelocity = Settings.UseAverageVelocity;
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

        void OnManipulationAllow()
        {
            if(m_update && (m_localLiftedPlayer != null))
            {
                VRC.Player l_remotePlayer = Utils.GetPlayerQM();
                if(l_remotePlayer != null)
                {
                    LifterPlayer l_component = l_remotePlayer.GetComponent<LifterPlayer>();
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
                ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).Show();
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

        void OnAllowButtonShown()
        {
            m_buttonVisibility = true;

            VRC.Player l_remotePlayer = Utils.GetPlayerQM();
            if(l_remotePlayer != null)
            {
                m_currentSelectedPlayer = l_remotePlayer;

                LifterPlayer l_component = l_remotePlayer.GetComponent<LifterPlayer>();
                m_textComponent.color = (l_component != null) ? Color.green : Color.white;
            }
        }

        void OnAllowButtonHidden()
        {
            m_buttonVisibility = false;
            m_currentSelectedPlayer = null;
        }

        static void OnPreSetupVRIK_PostfixPatch() => ms_instance?.OnPreSetupVRIK();
        void OnPreSetupVRIK()
        {
            if(m_localLiftedPlayer != null)
                m_localLiftedPlayer.DetectIKTweaks();
        }
    }
}
