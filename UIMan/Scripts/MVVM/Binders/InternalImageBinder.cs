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
        public bool zeroAlphaOnImageNull;

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
        }

        public void OnUpdateImage(object newImage)
        {
            var key = newImage == null ? string.Empty : newImage.ToString();

            if (string.IsNullOrEmpty(key))
            {
                this.image.sprite = null;
            }
            else
            {
                UIManAssetLoader.Load<Sprite>(key, OnLoadedImage);
            }
        }

        private void OnLoadedImage(string key, Object asset)
        {
            if (!(asset is Sprite sprite))
            {
                Debug.LogError($"Asset of key={key} is not a Sprite.");
                this.image.sprite = null;
                return;
            }

            this.image.sprite = sprite;

            if (this.autoCorrectSize)
                this.image.SetNativeSize();
        }
    }
}