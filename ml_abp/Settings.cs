namespace ml_abp
{
    static class Settings
    {
        static bool ms_allowFriends = true;
        static float ms_proximityDistance = 0.25f;
        static float ms_playersDistance = 5f;

        public static void LoadSettings()
        {
            MelonLoader.MelonPreferences.CreateCategory("ABP", "Avatar Bones Proximity");
            MelonLoader.MelonPreferences.CreateEntry("ABP", "AllowFriends", true, "Allow proximity check for friends");
            MelonLoader.MelonPreferences.CreateEntry("ABP", "ProximityDistance", 0.25f, "Proximity distance to bones");
            MelonLoader.MelonPreferences.CreateEntry("ABP", "PlayersDistance", 5f, "Proximity distance to players");

            ReloadSettings();
        }

        public static void ReloadSettings()
        {
            ms_allowFriends = MelonLoader.MelonPreferences.GetEntryValue<bool>("ABP", "AllowFriends");
            ms_proximityDistance = MelonLoader.MelonPreferences.GetEntryValue<float>("ABP", "ProximityDistance");
            ms_playersDistance = MelonLoader.MelonPreferences.GetEntryValue<float>("ABP", "PlayersDistance");
        }

        public static bool AllowFriends
        {
            get => ms_allowFriends;
        }
        public static float ProximityDistance
        {
            get => ms_proximityDistance;
        }
        public static float PlayersDistance
        {
            get => ms_playersDistance;
        }
    }
}
