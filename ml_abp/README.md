# Avatar Bones Proximity
MelonLoader mod for VRChat that brings additional avatar parameters for proximity to avatar bones

# Installation
* Install [latest MelonLoader](https://github.com/LavaGang/MelonLoader)
* Install [latest UIExpansionKit](https://github.com/knah/VRCMods)
* Install [latest VRChatUtilityKit](https://github.com/SleepyVRC/Mods)
* Get [latest release DLL](../../../releases/latest)
* Put `ml_abp.dll` in `Mods` folder of game

# Usage
## Settings
Available mod's settings through UIExpansionKit:
* **Allow proximity check for friends:** enable proximity checks for friends in room (includes new joiners and option toggle), enabled by default.
* **Proximity distance to bones:** start distance to bones for avatar proximity targets, 0.25 by default. 
* **Proximity distance to players:** distance to remote player to be included in proximity checks, 5.0 by default.
* **Use custom proximity targets:** use custom proximity targets included in avatar, disabled by default.

## Avatar
Add additional float parameter(s) to your avatar(s) and animator(s) as pattern `_(BoneName)Proximity` (example: `_HeadProximity`). List of available bones is available on [this Unity documentation page](https://docs.unity3d.com/2019.4/Documentation/ScriptReference/HumanBodyBones.html).  
 Also, you can add your own custom proximity target(s). For that your avatar parameter(s) and game object(s) in avatar(s) hierarchy should have same name and start with `_ProximityTarget` (example: `_ProximityTargetNose`).

Added parameters change value according to proximity of another player's hands to bone(s) (or custom target(s)) of your avatar(s). Each added parameter has range from 0.0 to 1.0.

## Menu
* Select player from quick menu.
* Press `Toggle bones proximity` in UIExpansionKit quick menu.

# Notes
* This mod is made specificaly for SDK3 avatars creators, but will be obsolete after physics bones update release.
* Eyes and jaw bones are merged with head due to zero scaling of local player's head bone. Those bones will have same proximity values if camera is near of head bone.
