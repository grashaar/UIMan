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
            public readonly float? hideDelay;

            public Settings(bool showIcon, bool showCover, bool showBackground, bool showProgress, bool deactivateOnHide,
                            in float? alphaOnShow = null, in float? hideDelay = null)
            {
                this.showIcon = showIcon;
                this.showCover = showCover;
                this.showBackground = showBackground;
                this.showProgress = showProgress;
                this.deactivateOnHide = deactivateOnHide;
                this.alphaOnShow = alphaOnShow;
                this.hideDelay = hideDelay;
            }

            public Settings With(in bool? showIcon = null, in bool? showCover = null, in bool? showBackground = null,
                                 in bool? showProgress = null, in bool? deactivateOnHide = null,
                                 in float? alphaOnShow = null, in float? hideDelay = null)
            {
                return new Settings(
                    showIcon ?? this.showIcon,
                    showCover ?? this.showCover,
                    showBackground ?? this.showBackground,
                    showProgress ?? this.showProgress,
                    deactivateOnHide ?? this.deactivateOnHide,
                    alphaOnShow ?? this.alphaOnShow,
                    hideDelay ?? this.hideDelay
                );
            }

            public void Deconstruct(out bool showIcon, out bool showCover, out bool showBackground,
                                    out bool showProgress, out bool deactivateOnHide,
                                    out float? alphaOnShow, out float? hideDelay)
            {
                showIcon = this.showIcon;
                showCover = this.showCover;
                showBackground = this.showBackground;
                showProgress = this.showProgress;
                deactivateOnHide = this.deactivateOnHide;
                alphaOnShow = this.alphaOnShow;
                hideDelay = this.hideDelay;
            }

            /// <summary>
            /// Shorthand for writting Settings(
            ///     showIcon = true,
            ///     showCover = true,
            ///     showBackground = false,
            ///     showProgress = false,
            ///     deactivateOnHide = true,
            ///     alphaOnShow = 0f,
            ///     hideDelay = 0f
            /// )
            /// </summary>
            public static Settings Default { get; } = new Settings(true, true, false, false, true, 0f, 0f);
        }
    }
}