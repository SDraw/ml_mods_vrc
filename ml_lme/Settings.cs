namespace ml_lme
{
    static class Settings
    {
        static bool ms_settingsUpdated = false;

        static bool ms_enabled = false;
        static bool ms_leapHmdMode = false;

        public static void LoadSettings()
        {
            MelonLoader.MelonPreferences.CreateCategory("LME", "Leap Motion extension");
            MelonLoader.MelonPreferences.CreateEntry("LME", "Enabled", ms_enabled, "Enable fingers tracking").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("LME", "LeapHmdMode", ms_leapHmdMode, "Leap HMD mode").OnValueChanged += OnAnyEntryUpdate;

            ReloadSettings();
        }

        public static void ReloadSettings()
        {
            ms_enabled = MelonLoader.MelonPreferences.GetEntryValue<bool>("LME", "Enabled");
            ms_leapHmdMode = MelonLoader.MelonPreferences.GetEntryValue<bool>("LME", "LeapHmdMode");
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

        public static bool LeapHmdMode
        {
            get => ms_leapHmdMode;
        }
    }
}
