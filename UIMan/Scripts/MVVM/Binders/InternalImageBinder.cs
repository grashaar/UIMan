using UnityEngine;
using UnityEngine.UI;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

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

        [HideInInspector]
        public StringConverter imageConverter = new StringConverter("Image");

        [HideInInspector]
        public ColorConverter colorConverter = new ColorConverter("Color");

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

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.image = GetComponent<Image>();

            SubscribeOnChangedEvent(this.imageField, OnUpdateImage);
            SubscribeOnChangedEvent(this.colorField, OnUpdateColor);
        }

        private void OnUpdateColor(object val)
        {
            this.image.color = this.colorConverter.Convert(val, this);
            SetColor();
        }

        private void OnUpdateImage(object val)
        {
            var key = this.imageConverter.Convert(val, this);

            if (string.IsNullOrEmpty(key))
            {
                this.image.sprite = null;
                SetColor();
            }
            else
            {
                UIManLoader.Load<Sprite>(key, OnLoadedImage);
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
                color.a = 1f;
            }

            this.image.color = color;
        }
    }
}