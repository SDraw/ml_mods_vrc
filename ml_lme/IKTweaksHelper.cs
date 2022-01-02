using System.Reflection;

namespace ml_lme
{
    public static class IKTweaksHelper
    {
        static bool ms_present = false;
        static MethodInfo ms_preSetupVrIk = null;

        public static bool Present
        {
            get => ms_present;
        }

        public static MethodInfo PreSetupVRIK
        {
            get => ms_preSetupVrIk;
        }

        public static void Resolve()
        {
            foreach(MelonLoader.MelonMod l_mod in MelonLoader.MelonHandler.Mods)
            {
                if(l_mod.Info.Name == "IKTweaks")
                {
                    ResovleMethods();
                    ms_present = true;
                    break;
                }
            }
        }

        // Those CSharp references are weird
        static void ResovleMethods()
        {
            ms_preSetupVrIk = typeof(IKTweaks.FullBodyHandling).GetMethod("PreSetupVrIk", BindingFlags.Static | BindingFlags.NonPublic);
        }
    }
}
