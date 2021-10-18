namespace ml_kte
{
    static class Settings
    {
        public enum KinectVersion
        {
            V1 = 1,
            V2
        }

        static bool ms_enabled = false;
        static int ms_kinectVersion = (int)KinectVersion.V1;
        static float ms_rootPositionX = 0f;
        static float ms_rootPositionY = 0f;
        static float ms_rootPositionZ = 0f;
        static float ms_rootRotationX = 0f;
        static float ms_rootRotationY = 0f;
        static float ms_rootRotationZ = 0f;

        public static void Load()
        {
            MelonLoader.MelonPreferences.CreateCategory("KTE", "Kinect Tracking Extensions");
            MelonLoader.MelonPreferences.CreateEntry("KTE", "Enabled", ms_enabled, "Enable Kinect tracking");
            MelonLoader.MelonPreferences.CreateEntry("KTE", "Version", ms_kinectVersion, "Kinect version (1 or 2)");
            MelonLoader.MelonPreferences.CreateEntry("KTE", "OffsetX", ms_rootPositionX, "Root position X");
            MelonLoader.MelonPreferences.CreateEntry("KTE", "OffsetY", ms_rootPositionY, "Root position Y");
            MelonLoader.MelonPreferences.CreateEntry("KTE", "OffsetZ", ms_rootPositionZ, "Root position Z");
            MelonLoader.MelonPreferences.CreateEntry("KTE", "OffsetRX", ms_rootRotationX, "Root rotation X");
            MelonLoader.MelonPreferences.CreateEntry("KTE", "OffsetRY", ms_rootRotationY, "Root rotation Y");
            MelonLoader.MelonPreferences.CreateEntry("KTE", "OffsetRZ", ms_rootRotationZ, "Root rotation Z");

            Reload();
        }

        public static void Reload()
        {
            ms_enabled = MelonLoader.MelonPreferences.GetEntryValue<bool>("KTE", "Enabled");

            ms_kinectVersion = UnityEngine.Mathf.Clamp(MelonLoader.MelonPreferences.GetEntryValue<int>("KTE", "Version"), (int)KinectVersion.V1, (int)KinectVersion.V2);
            MelonLoader.MelonPreferences.SetEntryValue("KTE", "Version", ms_kinectVersion);

            ms_rootPositionX = MelonLoader.MelonPreferences.GetEntryValue<float>("KTE", "OffsetX");
            ms_rootPositionY = MelonLoader.MelonPreferences.GetEntryValue<float>("KTE", "OffsetY");
            ms_rootPositionZ = MelonLoader.MelonPreferences.GetEntryValue<float>("KTE", "OffsetZ");
            ms_rootRotationX = MelonLoader.MelonPreferences.GetEntryValue<float>("KTE", "OffsetRX");
            ms_rootRotationY = MelonLoader.MelonPreferences.GetEntryValue<float>("KTE", "OffsetRY");
            ms_rootRotationZ = MelonLoader.MelonPreferences.GetEntryValue<float>("KTE", "OffsetRZ");
        }

        public static bool Enabled
        {
            get => ms_enabled;
        }

        public static KinectVersion DeviceVersion
        {
            get => (KinectVersion)ms_kinectVersion;
        }

        public static float OffsetX
        {
            get => ms_rootPositionX;
        }
        public static float OffsetY
        {
            get => ms_rootPositionY;
        }
        public static float OffsetZ
        {
            get => ms_rootPositionZ;
        }

        public static float OffsetRX
        {
            get => ms_rootRotationX;
        }
        public static float OffsetRY
        {
            get => ms_rootRotationY;
        }
        public static float OffsetRZ
        {
            get => ms_rootRotationX;
        }
    }
}
