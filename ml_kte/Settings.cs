namespace ml_kte
{
    static class Settings
    {
        public enum KinectVersion
        {
            V1 = 1,
            V2
        }

        static bool ms_settingsUpdated = false;

        static bool ms_enabled = false;
        static int ms_kinectVersion = (int)KinectVersion.V1;
        static float ms_rootPositionX = 0f;
        static float ms_rootPositionY = 0f;
        static float ms_rootPositionZ = 0f;
        static float ms_rootRotationX = 0f;
        static float ms_rootRotationY = 0f;
        static float ms_rootRotationZ = 0f;
        static bool ms_showPoints = false;
        static bool ms_trackHead = true;
        static bool ms_trackHands = true;
        static bool ms_trackHips = true;
        static bool ms_trackLegs = true;
        static bool ms_rotateHead = true;
        static bool ms_rotateHips = true;
        static bool ms_rotateLegs = true;
        static bool ms_rotateHands = true;

        public static void Load()
        {
            MelonLoader.MelonPreferences.CreateCategory("KTE", "Kinect Tracking Extensions");
            MelonLoader.MelonPreferences.CreateEntry("KTE", "Enabled", ms_enabled, "Enable Kinect tracking").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("KTE", "Version", ms_kinectVersion, "Kinect version (1 or 2)").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("KTE", "OffsetX", ms_rootPositionX, "Root position X").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("KTE", "OffsetY", ms_rootPositionY, "Root position Y").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("KTE", "OffsetZ", ms_rootPositionZ, "Root position Z").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("KTE", "OffsetRX", ms_rootRotationX, "Root rotation X").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("KTE", "OffsetRY", ms_rootRotationY, "Root rotation Y").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("KTE", "OffsetRZ", ms_rootRotationZ, "Root rotation Z").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("KTE", "ShowPoints", ms_showPoints, "Show tracking points").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("KTE", "TrackHead", ms_trackHead, "Head tracking").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("KTE", "TrackHips", ms_trackHips, "Hips tracking").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("KTE", "TrackLegs", ms_trackLegs, "Legs tracking").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("KTE", "TrackHands", ms_trackHands, "Hands tracking").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("KTE", "RotateHead", ms_rotateHead, "Head rotation").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("KTE", "RotateHips", ms_rotateHips, "Hips rotation").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("KTE", "RotateLegs", ms_rotateLegs, "Legs rotation").OnValueChanged += OnAnyEntryUpdate;
            MelonLoader.MelonPreferences.CreateEntry("KTE", "RotateHands", ms_rotateHands, "Hands rotation").OnValueChanged += OnAnyEntryUpdate;

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
            ms_showPoints = MelonLoader.MelonPreferences.GetEntryValue<bool>("KTE", "ShowPoints");
            ms_trackHead = MelonLoader.MelonPreferences.GetEntryValue<bool>("KTE", "TrackHead");
            ms_trackHands = MelonLoader.MelonPreferences.GetEntryValue<bool>("KTE", "TrackHands");
            ms_trackHips = MelonLoader.MelonPreferences.GetEntryValue<bool>("KTE", "TrackHips");
            ms_trackLegs = MelonLoader.MelonPreferences.GetEntryValue<bool>("KTE", "TrackLegs");
            ms_rotateHead = MelonLoader.MelonPreferences.GetEntryValue<bool>("KTE", "RotateHead");
            ms_rotateHips = MelonLoader.MelonPreferences.GetEntryValue<bool>("KTE", "RotateHips");
            ms_rotateLegs = MelonLoader.MelonPreferences.GetEntryValue<bool>("KTE", "RotateLegs");
            ms_rotateHands = MelonLoader.MelonPreferences.GetEntryValue<bool>("KTE", "RotateHands");
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

        public static bool ShowPoints
        {
            get => ms_showPoints;
        }

        public static bool TrackHead
        {
            get => ms_trackHead;
        }
        public static bool TrackHands
        {
            get => ms_trackHands;
        }
        public static bool TrackHips
        {
            get => ms_trackHips;
        }
        public static bool TrackLegs
        {
            get => ms_trackLegs;
        }

        public static bool RotateHead
        {
            get => ms_rotateHead;
        }
        public static bool RotateHips
        {
            get => ms_rotateHips;
        }
        public static bool RotateLegs
        {
            get => ms_rotateLegs;
        }
        public static bool RotateHands
        {
            get => ms_rotateHands;
        }
    }
}
