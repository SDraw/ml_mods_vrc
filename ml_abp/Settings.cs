namespace ml_abp
{
    static class Settings
    {
        static MelonLoader.MelonPreferences_Entry<bool> ms_friendsEntry = null;

        static bool ms_settingsUpdated = false;
        static bool ms_setingsUpdatedFriends = false; // Separated because components aren't that cheap

        static bool ms_allowFriends = true;
        static float ms_proximityDistance = 0.25f;
        static float ms_playersDistance = 5f;
        static bool ms_customTargets = false;

        public static void LoadSettings()
        {
            MelonLoader.MelonPreferences.CreateCategory("ABP", "Avatar Bones Proximity");

            ms_friendsEntry = MelonLoader.MelonPreferences.CreateEntry("ABP", "AllowFriends", true, "Allow proximity check for friends");
            ms_friendsEntry.OnValueChanged += OnAnyEntryUpdate;
            ms_friendsEntry.OnValueChanged += OnFriendsEntryUpdate;

            MelonLoader.MelonPreferences.CreateEntry("ABP", "ProximityDistance", 0.25f, "Proximity distance to bones").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("ABP", "PlayersDistance", 5f, "Proximity distance to players").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("ABP", "CustomTargets", ms_customTargets, "Use custom proximity targets (avatar reload required)").OnValueChanged += OnAnyEntryUpdate;

            ReloadSettings();
        }

        public static void ReloadSettings()
        {
            ms_allowFriends = ms_friendsEntry.Value;

            ms_proximityDistance = UnityEngine.Mathf.Clamp(MelonLoader.MelonPreferences.GetEntryValue<float>("ABP", "ProximityDistance"), 0.000001f, float.MaxValue);
            MelonLoader.MelonPreferences.SetEntryValue("ABP", "ProximityDistance", ms_proximityDistance);

            ms_playersDistance = MelonLoader.MelonPreferences.GetEntryValue<float>("ABP", "PlayersDistance");
            ms_customTargets = MelonLoader.MelonPreferences.GetEntryValue<bool>("ABP", "CustomTargets");
        }

        static void OnAnyEntryUpdate<T>(T p_oldValue, T p_newValue) => ms_settingsUpdated = true;
        public static bool IsAnyEntryUpdated()
        {
            bool l_result = ms_settingsUpdated;
            ms_settingsUpdated = false;
            return l_result;
        }

        static void OnFriendsEntryUpdate(bool p_oldValue, bool p_newValue) => ms_setingsUpdatedFriends = true;
        public static bool IsFriendsEntryUpdated()
        {
            bool l_result = ms_setingsUpdatedFriends;
            ms_setingsUpdatedFriends = false;
            return l_result;
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
