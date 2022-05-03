# Leap Motion Extension
This mod allows you to use your Leap Motion controller for hands and fingers visual tracking.

[![](.github/img_01.png)](https://youtu.be/ALDBcI9yCyM)

# Installation
* Install [latest Ultraleap Gemini tracking software](https://developer.leapmotion.com/tracking-software-download)
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
* **Attach to head:** attach hands transformation to head instead of body, disabled by default.
* **Fingers tracking only:** apply only fingers tracking, disabled by default.
* **Desktop Y/Z axis offset:** up/forward offset position for body attachment (`Attach to head` is **`false`**), (-0.5, 0.4) by default.
* **Head Y/Z axis offset:** up/forward offset position for head attachment (`Attach to head` is **`true`**), (-0.15, 0.15) by default.
* **X axis rotation (for neck mounts):** root rotation around X axis, useful for neck mounts, 0 by default.
* **Gestures based on fingers tracking (VR):** avatar gestures based on fingers tracking, if disabled original gestures from controllers input will be used, VR only, disabled by default.
* **Show Leap Motion controller model:** show Leap Motion controller model, useful for tracking visualizing, disabled by default.

# Notes
* Due to tracking changes of VRChat it's recommended to enable `Attach to head` option in VR mode.
* Offset values correspond to SteamVR environment units.
* Tracked hands don't change transformation of picked up objects.
* If game is launched and used in VR mode without controllers:
  * Avatar gestures will not work and will reset to default gesture (internal game restriction, won't be fixed).
  * Enabled `Gestures based on fingers tracking (VR)` option will lead to disabled fingers tracking (restriction to not block mouse input).
* If your system can't run latest Ultraleap Gemini tracking software for [(un/)known reasons](https://support.leapmotion.com/hc/en-us/articles/4412486302353-Known-Issues-Gemini-), alternatively you can install [Ultraleap Gemini v5.0.0-preview](https://mega.nz/file/xMphmIBC#73iINYr6qwfE3GmDYBGNkbzwszRuaQkfrZP8QYw5dk0) and use `ml_lme_gp.dll` from latest release. This build version isn't added to VRCMG mods list.

# Credits
* Thanks to [Magic3000](https://github.com/Magic3000) for patch to enable remote finger tracking in VR mode.
