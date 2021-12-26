namespace ml_alg
{
    static class Settings
    {
        static float ms_grabDistance = 0.25f;
        static bool ms_allowFriends = true;
        static bool ms_savePose = false;
        static bool ms_useHipsRotation = true;
        static bool ms_useLegsRotation = true;
        static bool ms_useHandsRotation = true;
        static bool ms_useVelocity = false;
        static float ms_velocityMultiplier = 5f;
        static bool ms_useAverageVelocity = true;
        static bool ms_allowPull = true;
        static bool ms_allowHandsPull = true;
        static bool ms_allowHipsPull = true;
        static bool ms_allowLegsPull = true;

        public static void LoadSettings()
        {
            MelonLoader.MelonPreferences.CreateCategory("ALG", "Avatar Limbs Grabber");
            MelonLoader.MelonPreferences.CreateEntry("ALG", "GrabRadius", ms_grabDistance, "Maximal distance to limbs");
            MelonLoader.MelonPreferences.CreateEntry("ALG", "AllowFriends", ms_allowFriends, "Allow friends to manipulate you");
            MelonLoader.MelonPreferences.CreateEntry("ALG", "AllowPull", ms_allowPull, "Allow pull");
            MelonLoader.MelonPreferences.CreateEntry("ALG", "AllowHandsPull", ms_allowHandsPull, "Allow hands pull");
            MelonLoader.MelonPreferences.CreateEntry("ALG", "AllowHipsPull", ms_allowHipsPull, "Allow hips pull");
            MelonLoader.MelonPreferences.CreateEntry("ALG", "AllowLegsPull", ms_allowLegsPull, "Allow legs pull");
            MelonLoader.MelonPreferences.CreateEntry("ALG", "SavePose", ms_savePose, "Preserve manipulated pose");
            MelonLoader.MelonPreferences.CreateEntry("ALG", "HipsRotation", ms_useHipsRotation, "Apply rotation to hips");
            MelonLoader.MelonPreferences.CreateEntry("ALG", "LegsRotation", ms_useLegsRotation, "Apply rotation to legs");
            MelonLoader.MelonPreferences.CreateEntry("ALG", "HandsRotation", ms_useHandsRotation, "Apply rotation to hands");
            MelonLoader.MelonPreferences.CreateEntry("ALG", "Velocity", ms_useVelocity, "Apply velocity on pull");
            MelonLoader.MelonPreferences.CreateEntry("ALG", "VelocityMultiplier", ms_velocityMultiplier, "Velocity multiplier (0-100)");
            MelonLoader.MelonPreferences.CreateEntry("ALG", "AverageVelocity", ms_useAverageVelocity, "Use average velocity");

            ReloadSettings();
        }

        public static void ReloadSettings()
        {
            ms_grabDistance = MelonLoader.MelonPreferences.GetEntryValue<float>("ALG", "GrabRadius");
            ms_allowFriends = MelonLoader.MelonPreferences.GetEntryValue<bool>("ALG", "AllowFriends");
            ms_allowPull = MelonLoader.MelonPreferences.GetEntryValue<bool>("ALG", "AllowPull");
            ms_allowHandsPull = MelonLoader.MelonPreferences.GetEntryValue<bool>("ALG", "AllowHandsPull");
            ms_allowHipsPull = MelonLoader.MelonPreferences.GetEntryValue<bool>("ALG", "AllowHipsPull");
            ms_allowLegsPull = MelonLoader.MelonPreferences.GetEntryValue<bool>("ALG", "AllowLegsPull");
            ms_savePose = MelonLoader.MelonPreferences.GetEntryValue<bool>("ALG", "SavePose");
            ms_useHipsRotation = MelonLoader.MelonPreferences.GetEntryValue<bool>("ALG", "HipsRotation");
            ms_useLegsRotation = MelonLoader.MelonPreferences.GetEntryValue<bool>("ALG", "LegsRotation");
            ms_useHandsRotation = MelonLoader.MelonPreferences.GetEntryValue<bool>("ALG", "HandsRotation");
            ms_useVelocity = MelonLoader.MelonPreferences.GetEntryValue<bool>("ALG", "Velocity");
            ms_velocityMultiplier = UnityEngine.Mathf.Clamp(MelonLoader.MelonPreferences.GetEntryValue<float>("ALG", "VelocityMultiplier"), 0f, 100f);
            MelonLoader.MelonPreferences.SetEntryValue("ALG", "VelocityMultiplier", ms_velocityMultiplier);
            ms_useAverageVelocity = MelonLoader.MelonPreferences.GetEntryValue<bool>("ALG", "AverageVelocity");
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
        public static bool UseHipsRotation
        {
            get => ms_useHipsRotation;
        }
        public static bool UseLegsRotation
        {
            get => ms_useLegsRotation;
        }
        public static bool UseHandsRotation
        {
            get => ms_useHandsRotation;
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
    }
}
