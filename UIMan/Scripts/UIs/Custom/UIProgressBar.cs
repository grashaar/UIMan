using UnityEngine;
using UnityEngine.UI;

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

        [SerializeField]
        private RectTransform foreground = null;

        [SerializeField]
        private Image.Type type = Image.Type.Simple;

        [SerializeField, HideInInspector]
        private Image foregroundImage = null;

        [Space]
        [SerializeField]
        private RectTransform horizontalThumb = null;

        [SerializeField]
        private HorizontalOrigins horizontalThumbOrigin = HorizontalOrigins.Left;

        [Space]
        [SerializeField]
        private RectTransform verticalThumb = null;

        [SerializeField]
        private VerticalOrigins verticalThumbOrigin = VerticalOrigins.Top;

        [Space]
        [SerializeField]
        private bool autoMaxWidth = true;

        [SerializeField]
        private float maxWidth = 0;

        [SerializeField]
        [Range(0, 1)]
        private float value = 0;

        public HorizontalOrigins HorizontalThumbOrigin
        {
            get { return this.horizontalThumbOrigin; }
            set { this.horizontalThumbOrigin = value; }
        }

        public VerticalOrigins VerticalThumbOrigin
        {
            get { return this.verticalThumbOrigin; }
            set { this.verticalThumbOrigin = value; }
        }

        public bool AutoMaxWidth
        {
            get { return this.autoMaxWidth; }
            set { this.autoMaxWidth = value; }
        }

        public float MaxWidth
        {
            get { return this.maxWidth; }
            set { this.maxWidth = value; }
        }

        public float Value
        {
            get { return this.value; }

            set
            {
                this.value = value;
                UpdateVisual();
            }
        }

        private void Awake()
        {
            if (this.foreground)
                this.foregroundImage = this.foreground.GetComponent<Image>();

            if (this.autoMaxWidth)
                this.maxWidth = GetComponent<RectTransform>().rect.width;
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
            }
            else
            {
                var newWidth = this.value * this.maxWidth;
                var newRect = this.foreground.sizeDelta;
                newRect.x = newWidth;
                this.foreground.sizeDelta = newRect;
            }
        }

        private void UpdateHorizontalThumb()
        {
            if (!this.horizontalThumb)
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
            var newValue = this.value * this.maxWidth - halfWidth;
            newPos.x = this.horizontalThumbOrigin == HorizontalOrigins.Left ? newValue : -newValue;

            this.horizontalThumb.anchoredPosition = newPos;
        }

        private void UpdateVerticalThumb()
        {
            if (!this.verticalThumb)
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
            var halfWidth = this.verticalThumb.rect.height / 2f;
            var newValue = this.value * this.maxWidth - halfWidth;
            newPos.y = this.verticalThumbOrigin == VerticalOrigins.Bottom ? newValue : -newValue;

            this.verticalThumb.anchoredPosition = newPos;
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (this.foreground)
                this.foregroundImage = this.foreground.GetComponent<Image>();

            if (this.autoMaxWidth)
                this.maxWidth = GetComponent<RectTransform>().rect.width;

            UpdateVisual();
        }

#endif
    }
}