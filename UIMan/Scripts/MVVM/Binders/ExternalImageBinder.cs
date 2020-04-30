using UnityEngine;
using UnityEngine.UI;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

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

        [HideInInspector]
        public ColorConverter colorConverter = new ColorConverter("Color");

        public string resourcePath = "/Images/";
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

        private void OnUpdateImage(object newImage)
        {
            var key = newImage == null ? string.Empty : newImage.ToString();

            if (string.IsNullOrEmpty(key))
            {
                this.image.sprite = null;
                SetAlpha();
            }
            else
            {
                ExternalImageLoader.Instance.Load("file:///" + Application.persistentDataPath +
                                                  this.resourcePath + newImage.ToString(),
                                                  OnLoadComplete);
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
            this.image.color = this.colorConverter.Convert(val, this);
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