namespace ml_alg
{
    static class Settings
    {
        static MelonLoader.MelonPreferences_Entry<bool> ms_friendsEntry = null;

        static bool ms_settingsUpdated = false;
        static bool ms_setingsUpdatedFriends = false; // Separated because components aren't that cheap

        static float ms_grabDistance = 0.25f;
        static bool ms_allowFriends = true;
        static bool ms_savePose = false;
        static bool ms_useVelocity = false;
        static float ms_velocityMultiplier = 5f;
        static bool ms_useAverageVelocity = true;
        static bool ms_allowPull = true;
        static bool ms_allowHeadPull = false;
        static bool ms_allowHandsPull = true;
        static bool ms_allowHipsPull = true;
        static bool ms_allowLegsPull = true;
        static bool ms_distanceScale = true;

        public static void LoadSettings()
        {
            MelonLoader.MelonPreferences.CreateCategory("ALG", "Avatar Limbs Grabber");
            MelonLoader.MelonPreferences.CreateEntry("ALG", "GrabRadius", ms_grabDistance, "Maximal distance to limbs").OnValueChanged += OnAnyEntryUpdate;

            ms_friendsEntry = MelonLoader.MelonPreferences.CreateEntry("ALG", "AllowFriends", ms_allowFriends, "Allow friends to manipulate you");
            ms_friendsEntry.OnValueChanged += OnAnyEntryUpdate;
            ms_friendsEntry.OnValueChanged += OnFriendsEntryUpdate;

            MelonLoader.MelonPreferences.CreateEntry("ALG", "AllowPull", ms_allowPull, "Allow pull").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("ALG", "AllowHeadPull", ms_allowHeadPull, "Allow head pull").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("ALG", "AllowHandsPull", ms_allowHandsPull, "Allow hands pull").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("ALG", "AllowHipsPull", ms_allowHipsPull, "Allow hips pull").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("ALG", "AllowLegsPull", ms_allowLegsPull, "Allow legs pull").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("ALG", "SavePose", ms_savePose, "Preserve manipulated pose").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("ALG", "Velocity", ms_useVelocity, "Apply velocity on pull").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("ALG", "VelocityMultiplier", ms_velocityMultiplier, "Velocity multiplier (0-100)").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("ALG", "AverageVelocity", ms_useAverageVelocity, "Use average velocity").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("ALG", "DistanceScale", ms_distanceScale, "Use avatar scale for grabbing").OnValueChanged += OnAnyEntryUpdate;

            ReloadSettings();
        }

        public static void ReloadSettings()
        {
            ms_grabDistance = MelonLoader.MelonPreferences.GetEntryValue<float>("ALG", "GrabRadius");
            ms_allowFriends = ms_friendsEntry.Value;
            ms_allowPull = MelonLoader.MelonPreferences.GetEntryValue<bool>("ALG", "AllowPull");
            ms_allowHeadPull = MelonLoader.MelonPreferences.GetEntryValue<bool>("ALG", "AllowHeadPull");
            ms_allowHandsPull = MelonLoader.MelonPreferences.GetEntryValue<bool>("ALG", "AllowHandsPull");
            ms_allowHipsPull = MelonLoader.MelonPreferences.GetEntryValue<bool>("ALG", "AllowHipsPull");
            ms_allowLegsPull = MelonLoader.MelonPreferences.GetEntryValue<bool>("ALG", "AllowLegsPull");
            ms_savePose = MelonLoader.MelonPreferences.GetEntryValue<bool>("ALG", "SavePose");
            ms_useVelocity = MelonLoader.MelonPreferences.GetEntryValue<bool>("ALG", "Velocity");
            ms_velocityMultiplier = UnityEngine.Mathf.Clamp(MelonLoader.MelonPreferences.GetEntryValue<float>("ALG", "VelocityMultiplier"), 0f, 100f);
            MelonLoader.MelonPreferences.SetEntryValue("ALG", "VelocityMultiplier", ms_velocityMultiplier);
            ms_useAverageVelocity = MelonLoader.MelonPreferences.GetEntryValue<bool>("ALG", "AverageVelocity");
            ms_distanceScale = MelonLoader.MelonPreferences.GetEntryValue<bool>("ALG", "DistanceScale");
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

        public static float GrabDistance
        {
            get => ms_grabDistance;
        }
        public static bool AllowFriends
        {
            get => ms_allowFriends;
        }
        public static bool AllowPull
        {
            get => ms_allowPull;
        }
        public static bool AllowHeadPull
        {
            get => ms_allowHeadPull;
        }
        public static bool AllowHandsPull
        {
            get => ms_allowHandsPull;
        }
        public static bool AllowHipsPull
        {
            get => ms_allowHipsPull;
        }
        public static bool AllowLegsPull
        {
            get => ms_allowLegsPull;
        }
        public static bool SavePose
        {
            get => ms_savePose;
        }
        public static bool UseVelocity
        {
            get => ms_useVelocity;
        }
        public static float VelocityMultiplier
        {
            get => ms_velocityMultiplier;
        }
        public static bool UseAverageVelocity
        {
            get => ms_useAverageVelocity;
        }
        public static bool DistanceScale
        {
            get => ms_distanceScale;
        }
    }
}
