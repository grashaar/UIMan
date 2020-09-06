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
            public readonly float? alphaOnShow;

            public Settings(bool showIcon, bool showCover, bool showBackground, bool showProgress, bool deactivateOnHide,
                            float? alphaOnShow = null)
            {
                this.showIcon = showIcon;
                this.showCover = showCover;
                this.showBackground = showBackground;
                this.showProgress = showProgress;
                this.deactivateOnHide = deactivateOnHide;
                this.alphaOnShow = alphaOnShow;
            }

            public Settings With(bool? showIcon = null, bool? showCover = null, bool? showBackground = null,
                                 bool? showProgress = null, bool? deactivateOnHide = null,
                                 float? alphaOnShow = null)
            {
                return new Settings(
                    showIcon ?? this.showIcon,
                    showCover ?? this.showCover,
                    showBackground ?? this.showBackground,
                    showProgress ?? this.showProgress,
                    deactivateOnHide ?? this.deactivateOnHide,
                    alphaOnShow ?? this.alphaOnShow
                );
            }

            public void Deconstruct(out bool showIcon, out bool showCover, out bool showBackground,
                                    out bool showProgress, out bool deactivateOnHide,
                                    out float? alphaOnShow)
            {
                showIcon = this.showIcon;
                showCover = this.showCover;
                showBackground = this.showBackground;
                showProgress = this.showProgress;
                deactivateOnHide = this.deactivateOnHide;
                alphaOnShow = this.alphaOnShow;
            }

            /// <summary>
            /// Shorthand for writting Settings(
            ///     showIcon = true,
            ///     showCover = true,
            ///     showBackground = false,
            ///     showProgress = false,
            ///     deactivateOnHide = true,
            ///     alphaOnShow = null
            /// )
            /// </summary>
            public static Settings Default { get; } = new Settings(true, true, false, false, true, null);
        }
    }
}