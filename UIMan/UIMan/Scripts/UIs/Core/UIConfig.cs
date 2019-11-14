namespace UnuGames
{
    public class UIConfig
    {
        public const string SFX_KEY = "sfx";
        public const string BGM_KEY = "bgm";

        private static bool _isBGMOn = true;

        static public bool IsBGMOn
        {
            get { return _isBGMOn; }
            set { _isBGMOn = value; }
        }

        private static bool _isSFXOn = true;

        static public bool IsSFXOn
        {
            get { return _isSFXOn; }
            set { _isSFXOn = value; }
        }

        static public void Save(string key, string val)
        {
        }
    }
}