namespace ml_abp
{
    static class Logger
    {
        static MelonLoader.MelonLogger.Instance ms_melonLogger = new MelonLoader.MelonLogger.Instance("AvatarBonesProximity");

        public static void Message(string f_message) => ms_melonLogger?.Msg(f_message);
        public static void Warning(string f_warning) => ms_melonLogger?.Warning(f_warning);
        public static void Error(string f_error) => ms_melonLogger?.Error(f_error);
        public static void DebugMessage(string f_message) => MelonLoader.MelonDebug.Msg(f_message);
    }
}
