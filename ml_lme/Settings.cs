namespace ml_lme
{
    static class Settings
    {
        static bool ms_enabled = false;
        static bool ms_leapHmdMode = false;
        static bool ms_headRoot = false;
        static bool ms_sdk3Parameters = false;
        static bool ms_fingersTracking = false;
        static float ms_desktopOffsetY = -0.5f;
        static float ms_desktopOffsetZ = 0.4f;
        static float ms_hmdOffsetY = -0.15f;
        static float ms_hmdOffsetZ = 0.15f;
        static float ms_rootRotation = 0f;

        public static void LoadSettings()
        {
            MelonLoader.MelonPreferences.CreateCategory("LME", "Leap Motion extension");
            MelonLoader.MelonPreferences.CreateEntry("LME", "Enabled", ms_enabled, "Enable hands tracking");
            MelonLoader.MelonPreferences.CreateEntry("LME", "LeapHmdMode", ms_leapHmdMode, "HMD mode");
            MelonLoader.MelonPreferences.CreateEntry("LME", "HeadRoot", ms_headRoot, "Head as root point");
            MelonLoader.MelonPreferences.CreateEntry("LME", "Sdk3Parameters", ms_sdk3Parameters, "Set avatar SDK3 parameters");
            MelonLoader.MelonPreferences.CreateEntry("LME", "FingersTracking", ms_fingersTracking, "Fingers tracking only");
            MelonLoader.MelonPreferences.CreateEntry("LME", "DesktopOffsetY", ms_desktopOffsetY, "Desktop Y axis (up) offset");
            MelonLoader.MelonPreferences.CreateEntry("LME", "DesktopOffsetZ", ms_desktopOffsetZ, "Desktop Z axis (forward) offset");
            MelonLoader.MelonPreferences.CreateEntry("LME", "HmdOffsetY", ms_hmdOffsetY, "HMD Y axis (up) offset");
            MelonLoader.MelonPreferences.CreateEntry("LME", "HmdOffsetZ", ms_hmdOffsetZ, "HMD Z axis (forward) offset");
            MelonLoader.MelonPreferences.CreateEntry("LME", "RootRotation", ms_rootRotation, "Root X axis rotation (for neck mounts)");

            ReloadSettings();
        }

        public static void ReloadSettings()
        {
            ms_enabled = MelonLoader.MelonPreferences.GetEntryValue<bool>("LME", "Enabled");
            ms_leapHmdMode = MelonLoader.MelonPreferences.GetEntryValue<bool>("LME", "LeapHmdMode");
            ms_headRoot = MelonLoader.MelonPreferences.GetEntryValue<bool>("LME", "HeadRoot");
            ms_sdk3Parameters = MelonLoader.MelonPreferences.GetEntryValue<bool>("LME", "Sdk3Parameters");
            ms_fingersTracking = MelonLoader.MelonPreferences.GetEntryValue<bool>("LME", "FingersTracking");
            ms_desktopOffsetY = MelonLoader.MelonPreferences.GetEntryValue<float>("LME", "DesktopOffsetY");
            ms_desktopOffsetZ = MelonLoader.MelonPreferences.GetEntryValue<float>("LME", "DesktopOffsetZ");
            ms_hmdOffsetY = MelonLoader.MelonPreferences.GetEntryValue<float>("LME", "HmdOffsetY");
            ms_hmdOffsetZ = MelonLoader.MelonPreferences.GetEntryValue<float>("LME", "HmdOffsetZ");
            ms_rootRotation = MelonLoader.MelonPreferences.GetEntryValue<float>("LME", "RootRotation");
        }

        public static bool Enabled
        {
            get => ms_enabled;
        }

        public static bool LeapHmdMode
        {
            get => ms_leapHmdMode;
        }

        public static bool HeadRoot
        {
            get => ms_headRoot;
        }

        public static bool SDK3Parameters
        {
            get => ms_sdk3Parameters;
        }

        public static bool FingersTracking
        {
            get => ms_fingersTracking;
        }

        public static float DesktopOffsetY
        {
            get => ms_desktopOffsetY;
        }

        public static float DesktopOffsetZ
        {
            get => ms_desktopOffsetZ;
        }

        public static float HmdOffsetY
        {
            get => ms_hmdOffsetY;
        }

        public static float HmdOffsetZ
        {
            get => ms_hmdOffsetZ;
        }

        public static float RootRotation
        {
            get => ms_rootRotation;
        }
    }
}
