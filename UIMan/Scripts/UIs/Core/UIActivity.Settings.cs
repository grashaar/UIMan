namespace UnuGames
{
    public partial class UIActivity
    {
        public readonly struct Settings
        {
            public readonly bool showIcon;
            public readonly bool showCover;
            public readonly bool showBackground;
            public readonly bool showProgress;
            public readonly bool deactivateOnHide;

            public Settings(bool showIcon, bool showCover, bool showBackground, bool showProgress, bool deactivateOnHide)
            {
                this.showIcon = showIcon;
                this.showCover = showCover;
                this.showBackground = showBackground;
                this.showProgress = showProgress;
                this.deactivateOnHide = deactivateOnHide;
            }

            public void Deconstruct(out bool showIcon, out bool showCover, out bool showBackground,
                                    out bool showProgress, out bool deactivateOnHide)
            {
                showIcon = this.showIcon;
                showCover = this.showCover;
                showBackground = this.showBackground;
                showProgress = this.showProgress;
                deactivateOnHide = this.deactivateOnHide;
            }

            /// <summary>
            /// Shorthand for writting Settings(
            ///     showIcon = true,
            ///     showCover = true,
            ///     showBackground = false,
            ///     showProgress = false,
            ///     deactivateOnHide = true
            /// )
            /// </summary>
            public static Settings Default { get; } = new Settings(true, true, false, false, true);
        }
    }
}