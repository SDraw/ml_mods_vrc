using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ml_arh
{
    static class Settings
    {
        static bool ms_enabled = true;
        static bool ms_poseHeight = true;

        public static void Load()
        {
            MelonLoader.MelonPreferences.CreateCategory("ARH", "Avatar Real Height");
            MelonLoader.MelonPreferences.CreateEntry("ARH", "Enabled", ms_enabled, "Use avatar height collision");
            MelonLoader.MelonPreferences.CreateEntry("ARH", "PoseHeight", ms_poseHeight, "Pose height (standing, crouching, crawling)");

            Reload();
        }

        public static void Reload()
        {
            ms_enabled = MelonLoader.MelonPreferences.GetEntryValue<bool>("ARH", "Enabled");
            ms_poseHeight = MelonLoader.MelonPreferences.GetEntryValue<bool>("ARH", "PoseHeight");
        }

        public static bool Enabled
        {
            get => ms_enabled;
        }

        public static bool PoseHeight
        {
            get => ms_poseHeight;
        }
    }
}
