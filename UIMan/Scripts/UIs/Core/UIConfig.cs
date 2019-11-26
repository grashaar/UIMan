namespace UnuGames
{
    public class UIConfig
    {
        public const string SFX_KEY = "sfx";
        public const string BGM_KEY = "bgm";

        public static bool IsBackgrounMusicOn { get; set; } = true;

        public static bool IsSoundEffectOn { get; set; } = true;

        public static void Save(string key, string val)
        {
        }
    }
}