﻿using UIExpansionKit.API.Controls;
using UnityEngine;

namespace ml_alg
{
    public class AvatarLimbsGrabber : MelonLoader.MelonMod
    {
        static AvatarLimbsGrabber ms_instance = null;

        bool m_quit = false;

        object m_menuSettings = null;
        object m_menuLabelWorld = null;
        object m_buttonPlayerAllow = null;

        bool m_update = false;
        LiftedPlayer m_localLifted = null;
        VRC.Player m_selectedPlayer = null;

        public override void OnApplicationStart()
        {
            if(ms_instance == null)
                ms_instance = this;

            MethodsResolver.ResolveMethods();
            Settings.LoadSettings();

            VRChatUtilityKit.Utilities.VRCUtils.OnUiManagerInit += this.OnUiManagerInit;
            VRChatUtilityKit.Utilities.NetworkEvents.OnRoomLeft += this.OnLeftRoom;
            VRChatUtilityKit.Utilities.NetworkEvents.OnPlayerJoined += this.OnPlayerJoined;
            VRChatUtilityKit.Utilities.NetworkEvents.OnFriended += this.OnFriended;
            VRChatUtilityKit.Utilities.NetworkEvents.OnUnfriended += this.OnUnfriended;

            // Patches
            HarmonyInstance.Patch(
                typeof(RootMotion.FinalIK.IKSolverVR).GetMethod(nameof(RootMotion.FinalIK.IKSolverVR.VrcLateSolve)),
                new HarmonyLib.HarmonyMethod(typeof(AvatarLimbsGrabber), nameof(IKSolverVR_VrcLateSolve_Prefix))
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

                if(m_update && (m_localLifted != null) && Settings.IsAnyEntryUpdated())
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
                                    Object.Destroy(l_component);
                            }
                            else
                            {
                                if(Settings.AllowFriends)
                                {
                                    l_component = l_remotePlayer.gameObject.AddComponent<LifterPlayer>();
                                    l_component.AddLifted(m_localLifted);
                                }
                            }
                        }
                    }

                    m_localLifted.AllowPull = (Settings.AllowPull && VRChatUtilityKit.Utilities.VRCUtils.AreRiskyFunctionsAllowed);
                    m_localLifted.AllowHeadPull = Settings.AllowHeadPull;
                    m_localLifted.AllowHandsPull = Settings.AllowHandsPull;
                    m_localLifted.AllowHipsPull = Settings.AllowHipsPull;
                    m_localLifted.AllowLegsPull = Settings.AllowLegsPull;
                    m_localLifted.GrabDistance = Settings.GrabDistance;
                    m_localLifted.SavePose = Settings.SavePose;
                    m_localLifted.UseVelocity = Settings.UseVelocity;
                    m_localLifted.VelocityMultiplier = Settings.VelocityMultiplier;
                    m_localLifted.AverageVelocity = Settings.UseAverageVelocity;
                    m_localLifted.DistanceScale = Settings.DistanceScale;
                    m_localLifted.ReapplyPermissions();
                }
            }
        }

        public override void OnUpdate()
        {
            if(m_update && ((IMenuToggle)m_buttonPlayerAllow).CurrentInstance.activeInHierarchy)
            {
                VRC.Player l_selectedPlayer = Utils.GetPlayerQM();
                if((l_selectedPlayer != null) && (m_selectedPlayer != l_selectedPlayer))
                {
                    m_selectedPlayer = l_selectedPlayer;
                    ((IMenuToggle)m_buttonPlayerAllow).Selected = (m_selectedPlayer.GetComponent<LifterPlayer>() != null);
                }
            }
        }

        void OnUiManagerInit()
        {
            m_menuSettings = UIExpansionKit.API.ExpansionKitApi.CreateCustomQuickMenuPage(UIExpansionKit.API.LayoutDescription.WideSlimList);
            m_menuLabelWorld = ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).AddLabel("");
            ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).AddSimpleButton("Disallow manipulation for everyone in room", this.OnDisallowAll);
            ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).AddSimpleButton("Reset saved pose (All)", () => this.OnPoseReset(LiftedPlayer.BodyLimbs.All));
            ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).AddSimpleButton("Reset saved pose (Head)", () => this.OnPoseReset(LiftedPlayer.BodyLimbs.Head));
            ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).AddSimpleButton("Reset saved pose (Hands)", () => this.OnPoseReset(LiftedPlayer.BodyLimbs.Hands));
            ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).AddSimpleButton("Reset saved pose (Hips)", () => this.OnPoseReset(LiftedPlayer.BodyLimbs.Hips));
            ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).AddSimpleButton("Reset saved pose (Legs)", () => this.OnPoseReset(LiftedPlayer.BodyLimbs.Legs));
            ((UIExpansionKit.API.ICustomShowableLayoutedMenu)m_menuSettings).AddSimpleButton("Close", this.OnMenuClose);
            UIExpansionKit.API.ExpansionKitApi.GetExpandedMenu(UIExpansionKit.API.ExpandedMenu.QuickMenu).AddSimpleButton("[ALG]\nMenu", this.OnMenuShow);

            m_buttonPlayerAllow = UIExpansionKit.API.ExpansionKitApi.GetExpandedMenu(UIExpansionKit.API.ExpandedMenu.UserQuickMenu).AddToggleButton("[ALG]\nAllow manipulation", this.OnManipulationToggle, null);
        }

        void OnJoinedRoom()
        {
            m_update = true;

            m_localLifted = Utils.GetLocalPlayer().gameObject.AddComponent<LiftedPlayer>();
            m_localLifted.AllowPull = (Settings.AllowPull && VRChatUtilityKit.Utilities.VRCUtils.AreRiskyFunctionsAllowed);
            m_localLifted.AllowHeadPull = Settings.AllowHeadPull;
            m_localLifted.AllowHandsPull = Settings.AllowHandsPull;
            m_localLifted.AllowHipsPull = Settings.AllowHipsPull;
            m_localLifted.AllowLegsPull = Settings.AllowLegsPull;
            m_localLifted.GrabDistance = Settings.GrabDistance;
            m_localLifted.SavePose = Settings.SavePose;
            m_localLifted.UseVelocity = Settings.UseVelocity;
            m_localLifted.VelocityMultiplier = Settings.VelocityMultiplier;
            m_localLifted.AverageVelocity = Settings.UseAverageVelocity;
            m_localLifted.DistanceScale = Settings.DistanceScale;

            ((IMenuLabel)m_menuLabelWorld).SetText("World pull permission: <color=#" + (VRChatUtilityKit.Utilities.VRCUtils.AreRiskyFunctionsAllowed ? "00FF00>Allowed" : "FF0000>Disallowed") + "</color>");
        }

        void OnLeftRoom()
        {
            m_update = false;
            m_localLifted = null;
            m_selectedPlayer = null;
        }

        void OnPlayerJoined(VRC.Player p_player)
        {
            if(p_player == Utils.GetLocalPlayer())
                OnJoinedRoom();

            if(Settings.AllowFriends && Utils.IsFriend(p_player))
                MelonLoader.MelonCoroutines.Start(CreateLifterOnJoin(p_player));
        }
        System.Collections.IEnumerator CreateLifterOnJoin(VRC.Player p_player)
        {
            while(m_localLifted == null)
                yield return null;

            if(p_player != null)
            {
                LifterPlayer l_component = p_player.gameObject.AddComponent<LifterPlayer>();
                l_component.AddLifted(m_localLifted);
            }
        }

        void OnFriended(VRC.Core.APIUser p_apiPlayer)
        {
            if(m_update && (m_localLifted != null) && Settings.AllowFriends)
            {
                VRC.Player l_remotePlayer = Utils.GetPlayerWithId(p_apiPlayer.id);
                if(l_remotePlayer != null)
                {
                    LifterPlayer l_component = l_remotePlayer.GetComponent<LifterPlayer>();
                    if(l_component == null)
                    {
                        l_component = l_remotePlayer.gameObject.AddComponent<LifterPlayer>();
                        l_component.AddLifted(m_localLifted);
                    }
                }
            }
        }

        void OnUnfriended(string p_id)
        {
            if(m_update && (m_localLifted != null) && Settings.AllowFriends)
            {
                VRC.Player l_player = Utils.GetPlayerWithId(p_id);
                if(l_player != null)
                {
                    LifterPlayer l_component = l_player.GetComponent<LifterPlayer>();
                    if(l_component != null)
                        Object.Destroy(l_component);
                }
            }
        }

        void OnManipulationToggle(bool p_state)
        {
            if(m_update && (m_localLifted != null))
            {
                VRC.Player l_remotePlayer = Utils.GetPlayerQM();
                if(l_remotePlayer != null)
                {
                    LifterPlayer l_component = l_remotePlayer.GetComponent<LifterPlayer>();
                    if((l_component == null) && p_state)
                    {
                        l_component = l_remotePlayer.gameObject.AddComponent<LifterPlayer>();
                        l_component.AddLifted(m_localLifted);
                    }
                    else if((l_component != null) && !p_state)
                    {
                        l_component.RemoveLifted(m_localLifted);
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

        void OnPoseReset(LiftedPlayer.BodyLimbs p_bodyPart)
        {
            if(m_update && (m_localLifted != null) && Settings.SavePose)
                m_localLifted.ClearSavedPose(p_bodyPart);
        }

        void OnDisallowAll()
        {
            if(m_update && (m_localLifted != null))
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
                                Object.Destroy(l_component);
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

        static void IKSolverVR_VrcLateSolve_Prefix(ref RootMotion.FinalIK.IKSolverVR __instance) => ms_instance?.OnVrcLateIKSolve(__instance);
        void OnVrcLateIKSolve(RootMotion.FinalIK.IKSolverVR p_solver)
        {
            if(m_localLifted != null)
                m_localLifted.LateUpdateIK(p_solver);
        }
    }
}
