using UnityEngine;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
    [RequireComponent(typeof(Text))]
    [DisallowMultipleComponent]
    public class TextBinder : BinderBase
    {
        protected Text text;

        [HideInInspector]
        public BindingField textValue = new BindingField("Text");

        [HideInInspector]
        public BindingField textColor = new BindingField("Color", true);

        public string format;

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.text = GetComponent<Text>();

            SubscribeOnChangedEvent(this.textValue, OnUpdateText);
            SubscribeOnChangedEvent(this.textColor, OnUpdateColor);
        }

        public void OnUpdateText(object newText)
        {
            if (newText == null)
                return;

            if (string.IsNullOrEmpty(this.format))
            {
                this.text.text = newText.ToString();
            }
            else
            {
                this.text.text = string.Format(this.format, newText.ToString());
            }
        }

        public void OnUpdateColor(object newColor)
        {
            if (newColor == null)
                return;
            try
            {
                this.text.color = (Color)newColor;
            }
            catch
            {
                UnuLogger.LogWarning("Binding field is not a color!");
            }
        }
    }
}