using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ml_ahr
{
    static class Settings
    {
        static bool ms_enabled = true;

        public static void Load()
        {
            MelonLoader.MelonPreferences.CreateCategory("ARH", "Avatar Real Height");
            MelonLoader.MelonPreferences.CreateEntry("ARH", "Enabled", ms_enabled, "Use avatar height collision");

            Reload();
        }

        public static void Reload()
        {
            ms_enabled = MelonLoader.MelonPreferences.GetEntryValue<bool>("ARH", "Enabled");
        }

        public static bool Enabled
        {
            get => ms_enabled;
        }
    }
}
