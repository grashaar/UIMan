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
        public BindingField imageValue = new BindingField("Image");

        [HideInInspector]
        public BindingField imageColor = new BindingField("Color");

        public string resourcePath = "Images/";

        public bool autoCorrectSize;
        public bool zeroAlphaOnImageNull;

        public override void Init(bool forceInit)
        {
            if (CheckInit(forceInit))
            {
                this.image = GetComponent<Image>();

                SubscribeOnChangedEvent(this.imageValue, OnUpdateImage);
                SubscribeOnChangedEvent(this.imageColor, OnUpdateColor);
            }
        }

        public void OnUpdateImage(object newImage)
        {
            if (string.IsNullOrEmpty(newImage.ToString()))
            {
                this.image.color = new Color(this.image.color.r, this.image.color.g, this.image.color.b, 0);
            }
            else
            {
                this.image.color = Color.white;
                this.image.sprite = ResourceFactory.Load<Sprite>(this.resourcePath + newImage.ToString());
                if (this.autoCorrectSize)
                    this.image.SetNativeSize();
            }
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
    }
}