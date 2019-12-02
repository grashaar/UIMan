using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

namespace UnuGames.MVVM
{
    [RequireComponent(typeof(Image))]
    [DisallowMultipleComponent]
    public class SpriteAtlasImageBinder : BinderBase
    {
        protected Image image;

        [HideInInspector]
        public BindingField atlasField = new BindingField("Atlas");

        [HideInInspector]
        public BindingField valueField = new BindingField("Image");

        [HideInInspector]
        public BindingField colorField = new BindingField("Color");

        public SpriteAtlas atlas;
        public bool autoCorrectSize;
        public bool zeroAlphaOnImageNull;

        private SpriteAtlas loadedAtlas;
        private string imageKey = string.Empty;

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.image = GetComponent<Image>();
            this.loadedAtlas = this.atlas;

            SubscribeOnChangedEvent(this.atlasField, OnUpdateAtlas);
            SubscribeOnChangedEvent(this.valueField, OnUpdateImage);
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

        public void OnUpdateAtlas(object val)
        {
            var key = val == null ? string.Empty : val.ToString();

            if (string.IsNullOrEmpty(key))
            {
                this.loadedAtlas = this.atlas;
                TryResolveImage();
            }
            else
            {
                UIManAssetLoader.Load<SpriteAtlas>(key, OnLoadedAtlas);
            }
        }

        public void OnUpdateImage(object val)
        {
            this.imageKey = val == null ? string.Empty : val.ToString();
            TryResolveImage();
        }

        private void OnLoadedAtlas(string key, Object asset)
        {
            if (asset is SpriteAtlas atlas)
            {
                this.loadedAtlas = atlas;
            }
            else
            {
                Debug.LogError($"Asset of key={key} is not a SpriteAtlas.");
                this.loadedAtlas = this.atlas;
            }

            TryResolveImage();
        }

        private void TryResolveImage()
        {
            if (string.IsNullOrEmpty(this.imageKey) || !this.loadedAtlas)
            {
                this.image.sprite = null;
            }
            else
            {
                this.image.sprite = this.loadedAtlas.GetSprite(this.imageKey);

                if (this.autoCorrectSize)
                    this.image.SetNativeSize();
            }

            SetAlpha();
        }

        private void SetAlpha()
        {
            var color = this.image.color;

            if (!this.image.sprite && this.zeroAlphaOnImageNull)
            {
                color.a = 0;
            }
            else
            {
                color.a = 1f;
            }

            this.image.color = color;
        }
    }
}