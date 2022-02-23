namespace ml_ps
{
    static class Settings
    {
        static bool ms_ignorePlayer = true;
        static bool ms_ignorePlayers = true;
        static bool ms_ignoreUI = true;
        static int ms_cubemapSize = 1024;
        static int ms_panoramaWidth = 2048;
        static int ms_panoramaHeight = 1024;

        public static void Load()
        {
            MelonLoader.MelonPreferences.CreateCategory("PS", "PanoramaScreenshot");
            MelonLoader.MelonPreferences.CreateEntry("PS", "IgnorePlayer", ms_ignorePlayer, "Ignore local player");
            MelonLoader.MelonPreferences.CreateEntry("PS", "IgnorePlayers", ms_ignorePlayers, "Ignore players");
            MelonLoader.MelonPreferences.CreateEntry("PS", "IgnoreUI", ms_ignoreUI, "Ignore UI");
            MelonLoader.MelonPreferences.CreateEntry("PS", "CubemapSize", ms_cubemapSize.ToString(), "Cubemap size");
            MelonLoader.MelonPreferences.CreateEntry("PS", "PanoramaWidth", ms_panoramaWidth.ToString(), "Panorama width");
            MelonLoader.MelonPreferences.CreateEntry("PS", "PanoramaHeight", ms_panoramaHeight.ToString(), "Panorama height");

            UIExpansionKit.API.ExpansionKitApi.RegisterSettingAsStringEnum("PS", "CubemapSize", new[]
            {
                ("32","32"),
                ("64","64"),
                ("128","128"),
                ("256","256"),
                ("512","512"),
                ("1024","1024"),
                ("2048","2048"),
                ("4096","4096"),
                ("8192","8192"),
                ("16384","16384")
            });
            UIExpansionKit.API.ExpansionKitApi.RegisterSettingAsStringEnum("PS", "PanoramaWidth", new[]
            {
                ("32","32"),
                ("64","64"),
                ("128","128"),
                ("256","256"),
                ("512","512"),
                ("1024","1024"),
                ("2048","2048"),
                ("4096","4096"),
                ("8192","8192"),
                ("16384","16384")
            });
            UIExpansionKit.API.ExpansionKitApi.RegisterSettingAsStringEnum("PS", "PanoramaHeight", new[]
            {
                ("32","32"),
                ("64","64"),
                ("128","128"),
                ("256","256"),
                ("512","512"),
                ("1024","1024"),
                ("2048","2048"),
                ("4096","4096"),
                ("8192","8192"),
                ("16384","16384")
            });

            Reload();
        }

        public static void Reload()
        {
            ms_ignorePlayer = MelonLoader.MelonPreferences.GetEntryValue<bool>("PS", "IgnorePlayer");
            ms_ignorePlayers = MelonLoader.MelonPreferences.GetEntryValue<bool>("PS", "IgnorePlayers");
            ms_ignoreUI = MelonLoader.MelonPreferences.GetEntryValue<bool>("PS", "IgnoreUI");
            ms_cubemapSize = int.Parse(MelonLoader.MelonPreferences.GetEntryValue<string>("PS", "CubemapSize"));
            ms_panoramaWidth = int.Parse(MelonLoader.MelonPreferences.GetEntryValue<string>("PS", "PanoramaWidth"));
            ms_panoramaHeight = int.Parse(MelonLoader.MelonPreferences.GetEntryValue<string>("PS", "PanoramaHeight"));
        }

        public static bool IgnorePlayer
        {
            get => ms_ignorePlayer;
        }
        public static bool IgnorePlayers
        {
            get => ms_ignorePlayers;
        }
        public static bool IgnoreUI
        {
            get => ms_ignoreUI;
        }
        public static int CubemapSize
        {
            get => ms_cubemapSize;
        }
        public static int PanoramaWidth
        {
            get => ms_panoramaWidth;
        }
        public static int PanoramaHeight
        {
            get => ms_panoramaHeight;
        }
    }
}
