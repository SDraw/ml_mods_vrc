namespace ml_arh
{
    static class Settings
    {
        static bool ms_settingsUpdated = false;

        static bool ms_enabled = true;
        static bool ms_poseHeight = true;

        public static void Load()
        {
            MelonLoader.MelonPreferences.CreateCategory("ARH", "Avatar Real Height");
            MelonLoader.MelonPreferences.CreateEntry("ARH", "Enabled", ms_enabled, "Use avatar height collision").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("ARH", "PoseHeight", ms_poseHeight, "Pose height (standing, crouching, crawling)").OnValueChanged += OnAnyEntryUpdate;

            Reload();
        }

        public static void Reload()
        {
            ms_enabled = MelonLoader.MelonPreferences.GetEntryValue<bool>("ARH", "Enabled");
            ms_poseHeight = MelonLoader.MelonPreferences.GetEntryValue<bool>("ARH", "PoseHeight");
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

        public static bool PoseHeight
        {
            get => ms_poseHeight;
        }
    }
}
