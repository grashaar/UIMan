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
        public BindingField imageValue = new BindingField("Image");

        [HideInInspector]
        public BindingField imageColor = new BindingField("Color");

        public string resourcePath = "/Images/";

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.image = GetComponent<Image>();

            SubscribeOnChangedEvent(this.imageValue, OnUpdateImage);
            SubscribeOnChangedEvent(this.imageColor, OnUpdateColor);
        }

        public void OnUpdateImage(object newImage)
        {
            if (newImage == null)
                return;

            ExternalImageLoader.Instance.Load("file:///" + Application.persistentDataPath + this.resourcePath + newImage.ToString(), OnLoadComplete);
        }

        private void OnLoadComplete(Sprite sprite)
        {
            this.image.sprite = sprite;
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