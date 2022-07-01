using System.ComponentModel;

namespace ml_ps
{
    static class Settings
    {
        [System.Flags]
        public enum TextureSize
        {
            [Description("32px")]
            X32 = 32,

            [Description("64px")]
            X64 = 64,

            [Description("128px")]
            X128 = 128,

            [Description("256px")]
            X256 = 256,

            [Description("512px")]
            X512 = 512,

            [Description("1024px")]
            X1024 = 1024,

            [Description("2048px")]
            X2048 = 2048,

            [Description("4096px")]
            X4096 = 4096,

            [Description("8192px")]
            X8192 = 8192,

            [Description("16384px")]
            X16384 = 16384,
        }

        static bool ms_ignorePlayer = true;
        static bool ms_ignorePlayers = true;
        static bool ms_ignoreUI = true;
        static TextureSize ms_cubemapSize = TextureSize.X1024;
        static TextureSize ms_panoramaWidth = TextureSize.X2048;
        static TextureSize ms_panoramaHeight = TextureSize.X1024;

        public static void Load()
        {
            MelonLoader.MelonPreferences.CreateCategory("PS", "PanoramaScreenshot");
            MelonLoader.MelonPreferences.CreateEntry("PS", "IgnorePlayer", ms_ignorePlayer, "Ignore local player");
            MelonLoader.MelonPreferences.CreateEntry("PS", "IgnorePlayers", ms_ignorePlayers, "Ignore players");
            MelonLoader.MelonPreferences.CreateEntry("PS", "IgnoreUI", ms_ignoreUI, "Ignore UI");
            MelonLoader.MelonPreferences.CreateEntry("PS", "CubemapSize", ms_cubemapSize, "Cubemap size");
            MelonLoader.MelonPreferences.CreateEntry("PS", "PanoramaWidth", ms_panoramaWidth, "Panorama width");
            MelonLoader.MelonPreferences.CreateEntry("PS", "PanoramaHeight", ms_panoramaHeight, "Panorama height");

            Reload();
        }

        public static void Reload()
        {
            ms_ignorePlayer = MelonLoader.MelonPreferences.GetEntryValue<bool>("PS", "IgnorePlayer");
            ms_ignorePlayers = MelonLoader.MelonPreferences.GetEntryValue<bool>("PS", "IgnorePlayers");
            ms_ignoreUI = MelonLoader.MelonPreferences.GetEntryValue<bool>("PS", "IgnoreUI");
            ms_cubemapSize = MelonLoader.MelonPreferences.GetEntryValue<TextureSize>("PS", "CubemapSize");
            ms_panoramaWidth = MelonLoader.MelonPreferences.GetEntryValue<TextureSize>("PS", "PanoramaWidth");
            ms_panoramaHeight = MelonLoader.MelonPreferences.GetEntryValue<TextureSize>("PS", "PanoramaHeight");
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
        public static TextureSize CubemapSize
        {
            get => ms_cubemapSize;
        }
        public static TextureSize PanoramaWidth
        {
            get => ms_panoramaWidth;
        }
        public static TextureSize PanoramaHeight
        {
            get => ms_panoramaHeight;
        }
    }
}
