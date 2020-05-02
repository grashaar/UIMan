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
        public StringConverter atlasConverter = new StringConverter("Atlas");

        [HideInInspector]
        public StringConverter imageConverter = new StringConverter("Image");

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
        private float alpha = 1f;

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

        private void OnUpdateColor(object val)
        {
            var color = this.colorConverter.Convert(val, this);
            this.image.color = color;
            this.alpha = color.a;

            SetColor();
        }

        private void OnUpdateAtlas(object val)
        {
            var key = this.atlasConverter.Convert(val, this);

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

        private void OnUpdateImage(object val)
        {
            this.imageKey = this.imageConverter.Convert(val, this);
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

            SetColor();
        }

        private void SetColor()
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
                color.a = this.alpha;
            }

            this.image.color = color;
        }
    }
}