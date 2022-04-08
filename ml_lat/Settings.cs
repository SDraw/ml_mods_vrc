namespace ml_lat
{
    static class Settings
    {

        static bool ms_settingsUpdated = false;

        static bool ms_legsAnimation = false;
        static bool ms_legsAutostep = false;

        public static void Load()
        {
            MelonLoader.MelonPreferences.CreateCategory("LAT", "Legs Animation Tweaker");
            MelonLoader.MelonPreferences.CreateEntry("LAT", "LegsAnimation", ms_legsAnimation, "Override legs animation").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("LAT", "LegsAutostep", ms_legsAutostep, "Override legs autostep").OnValueChanged += OnAnyEntryUpdate;
            Reload();
        }

        public static void Reload()
        {
            ms_legsAnimation = MelonLoader.MelonPreferences.GetEntryValue<bool>("LAT", "LegsAnimation");
            ms_legsAutostep = MelonLoader.MelonPreferences.GetEntryValue<bool>("LAT", "LegsAutostep");
        }

        static void OnAnyEntryUpdate<T>(T p_oldValue, T p_newValue) => ms_settingsUpdated = true;
        public static bool IsAnyEntryUpdated()
        {
            bool l_result = ms_settingsUpdated;
            ms_settingsUpdated = false;
            return l_result;
        }

        public static bool LegsAnimation
        {
            get => ms_legsAnimation;
        }

        public static bool LegsAutostep
        {
            get => ms_legsAutostep;
        }
    }
}
