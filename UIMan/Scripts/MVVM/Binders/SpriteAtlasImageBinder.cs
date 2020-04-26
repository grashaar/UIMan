using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

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

        [HideInInspector]
        public ColorConverter colorConverter = new ColorConverter("Color");

        public SpriteAtlas atlas;
        public bool autoCorrectSize;

        [Header("No Sprite")]
        [Range(0, 1f)]
#if ODIN_INSPECTOR
        [LabelText("Alpha")]
#endif
        public float noSpriteAlpha = 1f;

#if ODIN_INSPECTOR
        [HorizontalGroup("CustomColor")]
        [LabelText("Custom Color")]
#endif
        public bool useNoSpriteColor = false;

#if ODIN_INSPECTOR
        [HorizontalGroup("CustomColor")]
        [HideLabel]
        [ShowIf("useNoSpriteColor")]
#endif
        public Color noSpriteColor = Color.white;

        private SpriteAtlas loadedAtlas;
        private string imageKey = string.Empty;

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.image = GetComponent<Image>();
            SetLoadedAtlas(this.atlas);

            SubscribeOnChangedEvent(this.atlasField, OnUpdateAtlas);
            SubscribeOnChangedEvent(this.valueField, OnUpdateImage);
            SubscribeOnChangedEvent(this.colorField, OnUpdateColor);
        }

        public void OnUpdateColor(object val)
        {
            this.image.color = this.colorConverter.Convert(val, this);
            SetAlpha();
        }

        public void OnUpdateAtlas(object val)
        {
            var key = val == null ? string.Empty : val.ToString();

            if (string.IsNullOrEmpty(key))
            {
                SetLoadedAtlas(this.atlas);
                TryResolveImage();
            }
            else
            {
                UIManLoader.Load<SpriteAtlas>(key, OnLoadedAtlas);
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
                SetLoadedAtlas(atlas);
            }
            else
            {
                Debug.LogError($"Asset of key={key} is not a SpriteAtlas.");
                SetLoadedAtlas(this.atlas);
            }

            TryResolveImage();
        }

        private void SetLoadedAtlas(SpriteAtlas value)
        {
            this.loadedAtlas = value;
            SpriteAtlasManager.Register(value);
        }

        private void TryResolveImage()
        {
            var result = SpriteAtlasManager.TryGetSprite(this.loadedAtlas, this.imageKey, out var sprite);
            this.image.sprite = sprite;

            if (result && this.autoCorrectSize)
            {
                this.image.SetNativeSize();
            }

            SetAlpha();
        }

        private void SetAlpha()
        {
            var color = this.image.color;

            if (!this.image.sprite)
            {
                if (this.useNoSpriteColor)
                    color = this.noSpriteColor;

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