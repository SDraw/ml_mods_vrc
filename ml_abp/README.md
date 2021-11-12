# Avatar Bones Proximity
MelonLoader mod for VRChat that brings additional avatar parameters for proximity to avatar bones

# Installation
* Install [latest MelonLoader](https://github.com/LavaGang/MelonLoader)
* Install [latest UIExpansionKit](https://github.com/knah/VRCMods)
* Install [latest VRChatUtilityKit](https://github.com/loukylor/VRC-Mods)
* Get [latest release DLL](../../../releases/latest)
* Put `ml_abp.dll` in `Mods` folder of game

# Usage
* Add additional parameters to your avatar(s) and animator(s). List of available additional parameters:
  * _HeadProximity
  * _HipsProximity
  * _LeftHandProximity
  * _RightHandProximity
  * _LeftFootProximity
  * _RightFootProximity
  * _NeckProximity
  * _LeftEyeProximity
  * _RightEyeProximity
  * _SpineProximity
  * _ChestProximity
  * _LeftShoulderProximity
  * _RightShoulderProximity
  * _LeftUpperArmProximity
  * _RightUpperArmProximity
  * _LeftLowerArmProximity
  * _RightLowerArmProximity
  * _LeftUpperLegProximity
  * _RightUpperLegProximity
  * _LeftLowerLegProximity
  * _RightLowerLegProximity
  * _LeftToesProximity
  * _RightToesProximity
  * _JawProximity
  * Your own proximity target. For that your avatar parameter and game object in avatar hierarchy should have same name and start with `_ProximityTarget`.
* Select player in quick menu.
* Press `Toggle bones proximity` in UIExpansionKit quick menu.  

After that added parameters will change value according to proximity of another player's hands to bones of your avatar. Each parameter has range from 0.0 to 1.0.  
Also, check self-explanatory mod settings in UIExpansionKit's mods settings tab.

# Notes
* This mod is made specificaly for SDK3 avatars creators.
* Eyes and jaw bones are merged with head due to zero scaling of local player's head bone. Those bones will have same proximity values if camera is near of head bone.
* Usage of mods breaks ToS of VRChat and can lead to ban. Use at your own risk.
