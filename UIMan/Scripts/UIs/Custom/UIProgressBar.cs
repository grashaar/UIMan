using UnityEngine;
using UnityEngine.UI;

namespace UnuGames
{
    [ExecuteInEditMode]
    public class UIProgressBar : MonoBehaviour, IProgressBar
    {
        [SerializeField]
        private RectTransform foreground;

        [SerializeField]
        private RectTransform thumb;

        [SerializeField]
        private Image.Type type;

        [SerializeField]
        private float maxWidth = 0;

        [SerializeField]
        [Range(0, 1)]
        private float value;

        private Image forgroundImage;

        public float Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        private void Awake()
        {
            if (this.foreground)
                this.forgroundImage = this.foreground.GetComponentInChildren<Image>();
        }

        public void UpdateValue(float value)
        {
            this.Value = value;
            UpdateForeground();
            UpdateThumb();
        }

        private void UpdateForeground()
        {
            if (!this.foreground)
                return;

            if (this.type == Image.Type.Filled)
            {
                this.forgroundImage.fillAmount = this.value;
            }
            else
            {
                var newWidth = this.value * this.maxWidth;
                Vector2 newRect = this.foreground.sizeDelta;
                newRect.x = newWidth;
                this.foreground.sizeDelta = newRect;
            }
        }

        private void UpdateThumb()
        {
            if (!this.thumb)
                return;

            var newWidth = this.value * this.maxWidth;
            Vector2 newRect = this.thumb.sizeDelta;
            newRect.x = newWidth;
            this.thumb.sizeDelta = newRect;
        }

#if UNITY_EDITOR

        private void Update()
        {
            if (!Application.isPlaying)
            {
                UpdateValue(this.value);
            }
        }

#endif
    }
}