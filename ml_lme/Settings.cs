namespace ml_lme
{
    static class Settings
    {
        [System.Flags]
        public enum LeapTrackingMode
        {
            [System.ComponentModel.Description("Screentop")]
            Screentop = 0,

            [System.ComponentModel.Description("Desktop")]
            Desktop = 1,

            [System.ComponentModel.Description("HMD")]
            Hmd = 2
        }

        static bool ms_settingsUpdated = false;

        static bool ms_enabled = false;
        static LeapTrackingMode ms_trackingMode = LeapTrackingMode.Desktop;
        static bool ms_headRoot = false;
        static bool ms_fingersTracking = false;
        static float ms_desktopOffsetY = -0.5f;
        static float ms_desktopOffsetZ = 0.4f;
        static float ms_headOffsetY = -0.15f;
        static float ms_headOffsetZ = 0.15f;
        static float ms_rootRotation = 0f;
        static bool ms_showModel = false;

        public static void LoadSettings()
        {
            MelonLoader.MelonPreferences.CreateCategory("LME", "Leap Motion extension");
            MelonLoader.MelonPreferences.CreateEntry("LME", "Enabled", ms_enabled, "Enable Leap tracking").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("LME", "TrackingMode", ms_trackingMode, "Tracking mode").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("LME", "HeadRoot", ms_headRoot, "Attach to head").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("LME", "FingersTracking", ms_fingersTracking, "Fingers tracking only").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("LME", "DesktopOffsetY", ms_desktopOffsetY, "Desktop Y axis (up) offset").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("LME", "DesktopOffsetZ", ms_desktopOffsetZ, "Desktop Z axis (forward) offset").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("LME", "HeadOffsetY", ms_headOffsetY, "Head Y axis (up) offset").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("LME", "HeadffsetZ", ms_headOffsetZ, "Head Z axis (forward) offset").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("LME", "RootRotation", ms_rootRotation, "X axis rotation (for neck mounts)").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("LME", "ShowModel", ms_showModel, "Show Leap Motion controller model").OnValueChanged += OnAnyEntryUpdate;

            ReloadSettings();
        }

        public static void ReloadSettings()
        {
            ms_enabled = MelonLoader.MelonPreferences.GetEntryValue<bool>("LME", "Enabled");
            ms_trackingMode = MelonLoader.MelonPreferences.GetEntryValue<LeapTrackingMode>("LME", "TrackingMode");
            ms_headRoot = MelonLoader.MelonPreferences.GetEntryValue<bool>("LME", "HeadRoot");
            ms_fingersTracking = MelonLoader.MelonPreferences.GetEntryValue<bool>("LME", "FingersTracking");
            ms_desktopOffsetY = MelonLoader.MelonPreferences.GetEntryValue<float>("LME", "DesktopOffsetY");
            ms_desktopOffsetZ = MelonLoader.MelonPreferences.GetEntryValue<float>("LME", "DesktopOffsetZ");
            ms_headOffsetY = MelonLoader.MelonPreferences.GetEntryValue<float>("LME", "HeadOffsetY");
            ms_headOffsetZ = MelonLoader.MelonPreferences.GetEntryValue<float>("LME", "HeadffsetZ");
            ms_rootRotation = MelonLoader.MelonPreferences.GetEntryValue<float>("LME", "RootRotation");
            ms_showModel = MelonLoader.MelonPreferences.GetEntryValue<bool>("LME", "ShowModel");
        }

        static void OnAnyEntryUpdate<T>(T p_oldValue, T p_newValue) => ms_settingsUpdated = true;
        public static bool IsAnyEntryUpdated()
        {
            bool l_result = ms_settingsUpdated;
            ms_settingsUpdated = false;
            return l_result;
        }

        public static bool Enabled
        {
            get => ms_enabled;
        }

        public static LeapTrackingMode TrackingMode
        {
            get => ms_trackingMode;
        }

        public static bool HeadRoot
        {
            get => ms_headRoot;
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

        public static float HeadOffsetY
        {
            get => ms_headOffsetY;
        }

        public static float HeadOffsetZ
        {
            get => ms_headOffsetZ;
        }

        public static float RootRotation
        {
            get => ms_rootRotation;
        }

        public static bool ShowModel
        {
            get => ms_showModel;
        }
    }
}
