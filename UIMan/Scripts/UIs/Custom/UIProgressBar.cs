using UnityEngine;
using UnityEngine.UI;

namespace UnuGames
{
    [ExecuteInEditMode]
    public class UIProgressBar : MonoBehaviour
    {
        public RectTransform foreground;
        public RectTransform thumb;
        public Image.Type type;
        private Image foreGroundImg;

        [SerializeField]
        private float maxWidth = 0;

        [SerializeField]
        [Range(0, 1)]
        private float currentValue;

        public float CurrentValue
        {
            get
            {
                return this.currentValue;
            }
            set
            {
                this.currentValue = value;
            }
        }

        private void Awake()
        {
            Transform fg = this.transform.Find("FG");
            if (fg != null)
                this.foreground = fg.GetComponent<RectTransform>();
            this.foreGroundImg = this.foreground.GetComponent<Image>();
            Transform fgThumb = this.transform.Find("FGThumb");
            if (fgThumb != null)
                this.foreground = fg.GetComponent<RectTransform>();
            if (fgThumb != null)
                this.thumb = fgThumb.GetComponent<RectTransform>();
        }

        public void UpdateValue(float value)
        {
            this.CurrentValue = value;
            if (this.type == Image.Type.Filled)
            {
                this.foreGroundImg.fillAmount = value;
            }
            else
            {
                var newWidth = value * this.maxWidth;
                Vector2 newRect = this.foreground.sizeDelta;
                newRect.x = newWidth;
                this.foreground.sizeDelta = newRect;
            }

            if (this.thumb != null)
            {
                var newWidth = value * this.maxWidth;
                Vector2 newRect = this.thumb.sizeDelta;
                newRect.x = newWidth;
                this.thumb.sizeDelta = newRect;
            }
        }

#if UNITY_EDITOR

        private void Update()
        {
            if (!Application.isPlaying)
            {
                UpdateValue(this.currentValue);
            }
        }

#endif
    }
}