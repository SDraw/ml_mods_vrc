# Leap Motion Extension
This mod allows you to use your Leap Motion controller for hands and fingers visual tracking.

[![](.github/img_01.png)](https://youtu.be/ALDBcI9yCyM)

# Installation
* Install [Gemini v5.3.1](https://developer.leapmotion.com/tracking-software-download)
* Install [latest MelonLoader](https://github.com/LavaGang/MelonLoader)
* Install [latest VRChatUtilityKit](https://github.com/SleepyVRC/Mods)
* **Recommended:** Install [latest UIExpansionKit](https://github.com/knah/VRCMods)
* Get [latest release DLL](../../../releases/latest):
  * Put `ml_lme.dll` in `Mods` folder of game

# Usage
## Settings
Available mod's settings through UIExpansionKit:
* **Enable hands tracking:** enable hands tracking from Leap Motion data, disabled by default.
* **HMD mode:** force Leap Motion to use head-mounted orientation mode, disabled by default.
* **Head as root point:** attach hands transformation to head instead of body, disabled by default.
* **Fingers tracking only:** apply only fingers tracking, disabled by default.
* **Desktop Y/Z axis:** up/forward offset position in desktop orientation mode, (-0.5, 0.4) by default.
* **HMD Y/Z axis:** up/forward offset position in head-mounted orientation mode, (-0.15, 0.15) by default.
* **Root X axis rotation:** root rotation around X axis, useful for neck mounts, 0 by default.

# Notes
* In desktop mode be sure to disable gestures in "Action Menu (R) - Options - Gestures" to make avatar animation actions be in fixed state instead of on hold state.
* Due to tracking changes of VRChat it's recommended to toggle on option `Head as root point` in VR mode.
* Offset values correspond to SteamVR environment units.
* Tracked hands don't change transformation of picked up objects.

# Credits
* Thanks to [Magic3000](https://github.com/Magic3000) for patch to enable remote finger tracking in VR mode.
