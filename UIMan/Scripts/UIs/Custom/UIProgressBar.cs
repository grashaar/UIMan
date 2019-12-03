using UnityEngine;
using UnityEngine.UI;

namespace UnuGames
{
    public class UIProgressBar : MonoBehaviour, IProgressBar
    {
        [SerializeField]
        private RectTransform foreground = null;

        [SerializeField]
        private RectTransform thumb = null;

        [SerializeField]
        private bool horizontalThumb = true;

        [SerializeField]
        private Image.Type type = Image.Type.Simple;

        [SerializeField]
        private bool autoMaxWidth = true;

        [SerializeField]
        private float maxWidth = 0;

        [SerializeField]
        [Range(0, 1)]
        private float value = 0;

        public bool HorizontalThumb
        {
            get { return this.horizontalThumb; }
            set { this.horizontalThumb = value; }
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

        private Image forgroundImage;

        private void Awake()
        {
            if (this.foreground)
                this.forgroundImage = this.foreground.GetComponent<Image>();

            if (this.autoMaxWidth)
                this.maxWidth = GetComponent<RectTransform>().rect.width;
        }

        private void UpdateVisual()
        {
            UpdateForeground();
            UpdateThumb();
        }

        private void UpdateForeground()
        {
            if (!this.foreground)
                return;

            if (this.type == Image.Type.Filled)
            {
                if (this.forgroundImage)
                    this.forgroundImage.fillAmount = this.value;
            }
            else
            {
                var newWidth = this.value * this.maxWidth;
                var newRect = this.foreground.sizeDelta;
                newRect.x = newWidth;
                this.foreground.sizeDelta = newRect;
            }
        }

        private void UpdateThumb()
        {
            if (!this.thumb)
                return;

            var halfWidth = this.thumb.rect.width / 2f;
            var newValue = this.value * this.maxWidth - halfWidth;
            var newPos = this.thumb.anchoredPosition;

            if (this.horizontalThumb)
                newPos.x = newValue;
            else
                newPos.y = newValue;

            this.thumb.anchoredPosition = newPos;
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (this.autoMaxWidth)
                this.maxWidth = GetComponent<RectTransform>().rect.width;

            UpdateVisual();
        }

#endif
    }
}