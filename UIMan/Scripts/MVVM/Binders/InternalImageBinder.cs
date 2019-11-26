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
        public BindingField value = new BindingField("Image");

        [HideInInspector]
        public BindingField color = new BindingField("Color");

        public bool autoCorrectSize;
        public bool zeroAlphaOnImageNull;

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.image = GetComponent<Image>();

            SubscribeOnChangedEvent(this.value, OnUpdateImage);
            SubscribeOnChangedEvent(this.color, OnUpdateColor);
        }

        public void OnUpdateColor(object newColor)
        {
            if (newColor == null)
                return;
            try
            {
                this.image.color = (Color)newColor;
            }
            catch
            {
                UnuLogger.LogWarning("Binding field is not a color!");
            }
        }

        public void OnUpdateImage(object newImage)
        {
            var key = newImage.ToString();

            if (string.IsNullOrEmpty(key))
            {
                this.image.color = new Color(this.image.color.r, this.image.color.g, this.image.color.b, 0);
            }
            else
            {
                this.image.color = Color.white;
                StartCoroutine(UIManAssetLoader.Load<Sprite>(key, OnLoadedImage));
            }
        }

        private void OnLoadedImage(string key, Object asset)
        {
            if (!(asset is Sprite sprite))
            {
                Debug.LogError($"Asset of key={key} is not a Sprite.");
                return;
            }

            this.image.sprite = sprite;

            if (this.autoCorrectSize)
                this.image.SetNativeSize();
        }
    }
}