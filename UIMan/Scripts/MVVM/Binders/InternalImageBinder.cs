using UnityEngine;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
    [RequireComponent(typeof(Image))]
    [DisallowMultipleComponent]
    public class InternalImageBinder : BinderBase
    {
        protected Image image;

        [HideInInspector]
        public BindingField imageField = new BindingField("Image");

        [HideInInspector]
        public BindingField colorField = new BindingField("Color");

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

        public void OnUpdateColor(object val)
        {
            if (val == null)
                return;

            if (!(val is Color valChange))
                return;

            this.image.color = valChange;
            SetAlpha();
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
                UIManAssetLoader.Load<Sprite>(key, OnLoadedImage);
            }
        }

        private void OnLoadedImage(string key, Object asset)
        {
            if (asset is Sprite sprite)
            {
                this.image.sprite = sprite;

                if (this.autoCorrectSize)
                    this.image.SetNativeSize();
            }
            else
            {
                Debug.LogError($"Asset of key={key} is not a Sprite.");
                this.image.sprite = null;
            }

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