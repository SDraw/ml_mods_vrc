namespace ml_lat
{
    static class Logger
    {
        static MelonLoader.MelonLogger.Instance ms_melonLogger = new MelonLoader.MelonLogger.Instance("LegsAnimationTweaker");

        public static void Message(string p_message) => ms_melonLogger?.Msg(p_message);
        public static void Warning(string p_warning) => ms_melonLogger?.Warning(p_warning);
        public static void Error(string p_error) => ms_melonLogger?.Error(p_error);
        public static void DebugMessage(string p_message) => MelonLoader.MelonDebug.Msg("[LegsAnimationTweaker] " + p_message);
    }
}
