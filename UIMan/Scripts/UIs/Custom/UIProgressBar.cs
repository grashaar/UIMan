using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace UnuGames
{
    public class UIProgressBar : MonoBehaviour, IProgressBar
    {
        public enum HorizontalOrigins
        {
            Left, Right
        }

        public enum VerticalOrigins
        {
            Top, Bottom
        }

        [SerializeField, HideInInspector]
        private RectTransform rectTransform = null;

        [SerializeField]
        private RectTransform foreground = null;

        [SerializeField]
        private Image.Type type = Image.Type.Simple;

        [SerializeField, HideInInspector]
        private Image foregroundImage = null;

        [Header("Progress")]
        [SerializeField]
        [Range(0, 1)]
        private float value = 0;

        [Space]
        [Header("Horizonal")]
#if ODIN_INSPECTOR
        [LabelText("Enable")]
#endif
        [SerializeField]
        private bool horizontal = true;

#if ODIN_INSPECTOR
        [LabelText("Thumb")]
#endif
        [SerializeField]
        private RectTransform horizontalThumb = null;

#if ODIN_INSPECTOR
        [LabelText("Origin")]
#endif
        [SerializeField]
        private HorizontalOrigins horizontalThumbOrigin = HorizontalOrigins.Left;

        [FormerlySerializedAs("autoMaxWidth")]
        [SerializeField]
        private bool autoWidth = true;

        [FormerlySerializedAs("maxWidth")]
        [SerializeField]
        private float width = 0;

        [Space]
        [Header("Vertical")]
#if ODIN_INSPECTOR
        [LabelText("Enable")]
#endif
        [SerializeField]
        private bool vertical = true;

#if ODIN_INSPECTOR
        [LabelText("Thumb")]
#endif
        [SerializeField]
        private RectTransform verticalThumb = null;

#if ODIN_INSPECTOR
        [LabelText("Origin")]
#endif
        [SerializeField]
        private VerticalOrigins verticalThumbOrigin = VerticalOrigins.Top;

        [SerializeField]
        private bool autoHeight = true;

        [SerializeField]
        private float height = 0;

        public float Value
        {
            get { return this.value; }

            set
            {
                this.value = value;
                UpdateVisual();
            }
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
            get { return this.autoWidth; }
            set { this.autoWidth = value; UpdateAutoWidth(); }
        }

        public float Width
        {
            get { return this.width; }
            set { this.width = value; UpdateAutoWidth(); }
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
            get { return this.autoHeight; }
            set { this.autoHeight = value; UpdateAutoHeight(); }
        }

        public float Height
        {
            get { return this.height; }
            set { this.height = value; UpdateAutoHeight(); }
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
            if (this.autoWidth)
                this.width = this.rectTransform.rect.width;
        }

        private void UpdateAutoHeight()
        {
            if (this.autoHeight)
                this.height = this.rectTransform.rect.height;
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

            var newRect = this.foreground.sizeDelta;

            if (this.horizontal)
            {
                var newWidth = this.value * this.width;
                newRect.x = newWidth;
            }

            if (this.vertical)
            {
                var newHeight = this.value * this.height;
                newRect.y = newHeight;
            }

            this.foreground.sizeDelta = newRect;
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
            var newValue = this.value * this.width - halfWidth;
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
            var newValue = this.value * this.height - halfHeight;
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
    }
}