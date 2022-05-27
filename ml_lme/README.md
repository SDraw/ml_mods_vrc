# Leap Motion Extension
This mod allows you to use your Leap Motion controller for fingers visual tracking.

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
* **Enable fingers tracking:** enable fingers tracking from Leap Motion data, disabled by default.
* **Leap HMD mode:** force Leap Motion to use head-mounted orientation mode, disabled by default.

# Notes
* Due to IK 2.0 update hands tracking was removed.
* If your system can't run latest Ultraleap Gemini tracking software for [(un/)known reasons](https://support.leapmotion.com/hc/en-us/articles/4412486302353-Known-Issues-Gemini-), alternatively you can install [Ultraleap Gemini v5.0.0-preview](https://mega.nz/file/xMphmIBC#73iINYr6qwfE3GmDYBGNkbzwszRuaQkfrZP8QYw5dk0) and use `ml_lme_gp.dll` from latest release. This build version isn't added to VRCMG mods list.
