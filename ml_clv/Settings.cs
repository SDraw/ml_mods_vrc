namespace ml_clv
{
    static class Settings
    {
        static bool ms_settingsUpdated = false;

        static bool ms_enabled = true;
        static float ms_colorR = 0f;
        static float ms_colorG = 1f;
        static float ms_colorB = 0f;

        public static void LoadSettings()
        {
            MelonLoader.MelonPreferences.CreateCategory("CLV", "Calibration Lines Visualizer");
            MelonLoader.MelonPreferences.CreateEntry("CLV", "Enabled", ms_enabled, "Enable lines for trackers").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("CLV", "LineColorR", ms_colorR, "Red color component for lines").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("CLV", "LineColorG", ms_colorG, "Green color component for lines").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("CLV", "LineColorB", ms_colorB, "Blue color component for lines").OnValueChanged += OnAnyEntryUpdate;

            ReloadSettings();
        }

        public static void ReloadSettings()
        {
            ms_enabled = MelonLoader.MelonPreferences.GetEntryValue<bool>("CLV", "Enabled");
            ms_colorR = MelonLoader.MelonPreferences.GetEntryValue<float>("CLV", "LineColorR");
            ms_colorG = MelonLoader.MelonPreferences.GetEntryValue<float>("CLV", "LineColorG");
            ms_colorB = MelonLoader.MelonPreferences.GetEntryValue<float>("CLV", "LineColorB");
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
        public static float ColorR
        {
            get => ms_colorR;
        }
        public static float ColorG
        {
            get => ms_colorG;
        }
        public static float ColorB
        {
            get => ms_colorB;
        }
    }
}
