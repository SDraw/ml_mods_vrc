namespace ml_abp
{
    static class Settings
    {
        static bool ms_allowFriends = true;
        static float ms_proximityDistance = 0.25f;
        static float ms_playersDistance = 5f;
        static bool ms_customTargets = false;

        public static void LoadSettings()
        {
            MelonLoader.MelonPreferences.CreateCategory("ABP", "Avatar Bones Proximity");
            MelonLoader.MelonPreferences.CreateEntry("ABP", "AllowFriends", true, "Allow proximity check for friends");
            MelonLoader.MelonPreferences.CreateEntry("ABP", "ProximityDistance", 0.25f, "Proximity distance to bones");
            MelonLoader.MelonPreferences.CreateEntry("ABP", "PlayersDistance", 5f, "Proximity distance to players");
            MelonLoader.MelonPreferences.CreateEntry("ABP", "CustomTargets", ms_customTargets, "Use custom proximity targets (avatar reload required)");

            ReloadSettings();
        }

        public static void ReloadSettings()
        {
            ms_allowFriends = MelonLoader.MelonPreferences.GetEntryValue<bool>("ABP", "AllowFriends");

            ms_proximityDistance = UnityEngine.Mathf.Clamp(MelonLoader.MelonPreferences.GetEntryValue<float>("ABP", "ProximityDistance"), 0.000001f, float.MaxValue);
            MelonLoader.MelonPreferences.SetEntryValue("ABP", "ProximityDistance", ms_proximityDistance);

            ms_playersDistance = MelonLoader.MelonPreferences.GetEntryValue<float>("ABP", "PlayersDistance");
            ms_customTargets = MelonLoader.MelonPreferences.GetEntryValue<bool>("ABP", "CustomTargets");
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
        public static bool CustomTargets
        {
            get => ms_customTargets;
        }
    }
}
