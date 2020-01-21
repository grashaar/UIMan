using UnityEngine;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
    [RequireComponent(typeof(Image))]
    [DisallowMultipleComponent]
    public class ExternalImageBinder : BinderBase
    {
        protected Image image;

        [HideInInspector]
        public BindingField imageField = new BindingField("Image");

        [HideInInspector]
        public BindingField colorField = new BindingField("Color");

        public string resourcePath = "/Images/";
        public bool autoCorrectSize;
        public float noSpriteAlpha = 1f;

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.image = GetComponent<Image>();

            SubscribeOnChangedEvent(this.imageField, OnUpdateImage);
            SubscribeOnChangedEvent(this.colorField, OnUpdateColor);
        }

        public void OnUpdateImage(object newImage)
        {
            var key = newImage == null ? string.Empty : newImage.ToString();

            if (string.IsNullOrEmpty(key))
            {
                this.image.sprite = null;
                SetAlpha();
            }
            else
            {
                ExternalImageLoader.Instance.Load("file:///" + Application.persistentDataPath + this.resourcePath + newImage.ToString(), OnLoadComplete);
            }
        }

        private void OnLoadComplete(Sprite sprite)
        {
            this.image.sprite = sprite;

            if (sprite && this.autoCorrectSize)
                this.image.SetNativeSize();

            SetAlpha();
        }

        public void OnUpdateColor(object val)
        {
            if (val == null)
                return;

            if (!(val is Color valChange))
                return;

            this.image.color = valChange;
            SetAlpha();
        }

        private void SetAlpha()
        {
            var color = this.image.color;

            if (!this.image.sprite)
            {
                color.a = this.noSpriteAlpha;
            }
            else
            {
                color.a = 1f;
            }

            this.image.color = color;
        }
    }
}