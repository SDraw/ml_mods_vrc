namespace ml_vsf
{
    static class Settings
    {
        static bool ms_enableUpdated = false;

        static bool ms_enabled = false;
        static float ms_blending = 0.5f;
        static bool ms_mirroring = false;

        public static void Load()
        {
            MelonLoader.MelonPreferences.CreateCategory("VSF", "VSeeFace Extension");
            MelonLoader.MelonPreferences.CreateEntry("VSF", "Enabled", ms_enabled, "Enable VSF tracking").OnValueChanged += OnEnableEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("VSF", "Blend", ms_blending, "Linear smoothing");
            MelonLoader.MelonPreferences.CreateEntry("VSF", "Mirror", ms_mirroring, "Tracking mirroring");

            Reload();
        }

        public static void Reload()
        {
            ms_enabled = MelonLoader.MelonPreferences.GetEntryValue<bool>("VSF", "Enabled");
            ms_blending = MelonLoader.MelonPreferences.GetEntryValue<float>("VSF", "Blend");
            ms_mirroring = MelonLoader.MelonPreferences.GetEntryValue<bool>("VSF", "Mirror");
        }

        static void OnEnableEntryUpdate(bool p_oldValue, bool p_newValue) => ms_enableUpdated = true;
        public static bool IsEnableEntryUpdated()
        {
            bool l_result = ms_enableUpdated;
            ms_enableUpdated = false;
            return l_result;
        }

        public static bool Enabled
        {
            get => ms_enabled;
        }
        public static float Blending
        {
            get => ms_blending;
        }
        public static bool Mirroring
        {
            get => ms_mirroring;
        }
    }
}
