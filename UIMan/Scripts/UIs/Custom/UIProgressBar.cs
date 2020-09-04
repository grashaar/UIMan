using System;
using UnityEngine;
using UnityEngine.UI;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace UnuGames
{
    public class UIProgressBar : MonoBehaviour, IProgressBar
    {
        [SerializeField, HideInInspector]
        private RectTransform rectTransform = null;

        [SerializeField]
        private RectTransform foreground = null;

        [SerializeField]
        private Image.Type type = Image.Type.Simple;

        [SerializeField, HideInInspector]
        private Image foregroundImage = null;

        [SerializeField, Range(0f, 1f)]
        private float value = 0;

#if ODIN_INSPECTOR
        [FoldoutGroup("Horizontal"), LabelText("Enable")]
#else
        [Space]
        [Header("Horizonal")]
#endif
        [SerializeField]
        private bool horizontal = true;

#if ODIN_INSPECTOR
        [FoldoutGroup("Horizontal"), LabelText("Thumb")]
#endif
        [SerializeField]
        private RectTransform horizontalThumb = null;

#if ODIN_INSPECTOR
        [FoldoutGroup("Horizontal"), LabelText("Thumb Origin")]
#endif
        [SerializeField]
        private HorizontalOrigins horizontalThumbOrigin = HorizontalOrigins.Left;

#if ODIN_INSPECTOR
        [FoldoutGroup("Horizontal")]
#endif
        [SerializeField]
        private BarSize width = BarSize.Default;

#if ODIN_INSPECTOR
        [FoldoutGroup("Vertical"), LabelText("Enable")]
#else
        [Space]
        [Header("Vertical")]
#endif
        [SerializeField]
        private bool vertical = true;

#if ODIN_INSPECTOR
        [FoldoutGroup("Vertical"), LabelText("Thumb")]
#endif
        [SerializeField]
        private RectTransform verticalThumb = null;

#if ODIN_INSPECTOR
        [FoldoutGroup("Vertical"), LabelText("Thumb Origin")]
#endif
        [SerializeField]
        private VerticalOrigins verticalThumbOrigin = VerticalOrigins.Top;

#if ODIN_INSPECTOR
        [FoldoutGroup("Vertical")]
#endif
        [SerializeField]
        private BarSize height = BarSize.Default;

        public float Value
        {
            get { return this.value; }
            set { this.value = value; UpdateVisual(); }
        }

        public bool Horizontal
        {
            get { return this.horizontal; }
            set { this.horizontal = value; }
        }

        public HorizontalOrigins HorizontalThumbOrigin
        {
            get { return this.horizontalThumbOrigin; }
            set { this.horizontalThumbOrigin = value; }
        }

        public bool AutoWidth
        {
            get { return this.width.Auto; }
            set { this.width.Auto = value; UpdateAutoWidth(); }
        }

        public float Width
        {
            get { return this.width.Value; }
            set { this.width.Value = value; UpdateAutoWidth(); }
        }

        public bool Vertical
        {
            get { return this.vertical; }
            set { this.vertical = value; }
        }

        public VerticalOrigins VerticalThumbOrigin
        {
            get { return this.verticalThumbOrigin; }
            set { this.verticalThumbOrigin = value; }
        }

        public bool AutoHeight
        {
            get { return this.height.Auto; }
            set { this.height.Auto = value; UpdateAutoHeight(); }
        }

        public float Height
        {
            get { return this.height.Value; }
            set { this.height.Value = value; UpdateAutoHeight(); }
        }

        private void Awake()
        {
            if (!this.rectTransform)
                this.rectTransform = GetComponent<RectTransform>();

            if (this.foreground)
                this.foregroundImage = this.foreground.GetComponent<Image>();

            UpdateAutoWidth();
            UpdateAutoHeight();
        }

        private void UpdateAutoWidth()
        {
            if (this.width.Auto)
                this.width.Value = this.rectTransform.rect.width;
        }

        private void UpdateAutoHeight()
        {
            if (this.height.Auto)
                this.height.Value = this.rectTransform.rect.height;
        }

        private void UpdateVisual()
        {
            UpdateForeground();
            UpdateHorizontalThumb();
            UpdateVerticalThumb();
        }

        private void UpdateForeground()
        {
            if (!this.foreground)
                return;

            if (this.type == Image.Type.Filled &&
                this.foregroundImage)
            {
                this.foregroundImage.fillAmount = this.value;
                return;
            }

            var size = this.foreground.sizeDelta;

            if (this.horizontal)
            {
                var newWidth = this.value * this.width.Value;
                size.x = newWidth;
            }

            if (this.vertical)
            {
                var newHeight = this.value * this.height.Value;
                size.y = newHeight;
            }

            this.foreground.sizeDelta = size;
        }

        private void UpdateHorizontalThumb()
        {
            if (!this.horizontal || !this.horizontalThumb)
                return;

            if (this.horizontalThumbOrigin == HorizontalOrigins.Left)
            {
                this.horizontalThumb.anchorMin = new Vector2(0, 0);
                this.horizontalThumb.anchorMax = new Vector2(0, 1);
                this.horizontalThumb.pivot = new Vector2(0, 0.5f);
            }
            else
            {
                this.horizontalThumb.anchorMin = new Vector2(1, 0);
                this.horizontalThumb.anchorMax = new Vector2(1, 1);
                this.horizontalThumb.pivot = new Vector2(1, 0.5f);
            }

            var newPos = this.horizontalThumb.anchoredPosition;
            var halfWidth = this.horizontalThumb.rect.width / 2f;
            var newValue = this.value * this.width.Value - halfWidth;
            newPos.x = this.horizontalThumbOrigin == HorizontalOrigins.Left ? newValue : -newValue;

            this.horizontalThumb.anchoredPosition = newPos;
        }

        private void UpdateVerticalThumb()
        {
            if (!this.vertical || !this.verticalThumb)
                return;

            if (this.verticalThumbOrigin == VerticalOrigins.Top)
            {
                this.verticalThumb.anchorMin = new Vector2(0, 1);
                this.verticalThumb.anchorMax = new Vector2(1, 1);
                this.verticalThumb.pivot = new Vector2(0.5f, 1);
            }
            else
            {
                this.verticalThumb.anchorMin = new Vector2(0, 0);
                this.verticalThumb.anchorMax = new Vector2(1, 0);
                this.verticalThumb.pivot = new Vector2(0.5f, 0);
            }

            var newPos = this.verticalThumb.anchoredPosition;
            var halfHeight = this.verticalThumb.rect.height / 2f;
            var newValue = this.value * this.height.Value - halfHeight;
            newPos.y = this.verticalThumbOrigin == VerticalOrigins.Bottom ? newValue : -newValue;

            this.verticalThumb.anchoredPosition = newPos;
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            this.rectTransform = GetComponent<RectTransform>();

            if (this.foreground)
                this.foregroundImage = this.foreground.GetComponent<Image>();

            UpdateAutoWidth();
            UpdateAutoHeight();

            UpdateVisual();
        }

#endif

        public enum HorizontalOrigins
        {
            Left, Right
        }

        public enum VerticalOrigins
        {
            Top, Bottom
        }

#if ODIN_INSPECTOR
        [InlineProperty]
#endif
        [Serializable]
        private struct BarSize
        {
#if ODIN_INSPECTOR
            [HorizontalGroup, LabelWidth(35)]
#endif
            public bool Auto;

#if ODIN_INSPECTOR
            [HorizontalGroup, HideLabel, HideIf(nameof(Auto))]
#endif
            public float Value;

            public BarSize(bool auto, float size)
            {
                this.Auto = auto;
                this.Value = size;
            }

            public static BarSize Default { get; } = new BarSize(true, 0f);
        }
    }
}