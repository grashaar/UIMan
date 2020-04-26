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
        public BindingField textField = new BindingField("Text");

        [HideInInspector]
        public BindingField colorField = new BindingField("Color", true);

        [HideInInspector]
        public StringConverter textConverter = new StringConverter("Text");

        [HideInInspector]
        public ColorConverter colorConverter = new ColorConverter("Color");

        public string format;

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.text = GetComponent<Text>();

            SubscribeOnChangedEvent(this.textField, OnUpdateText);
            SubscribeOnChangedEvent(this.colorField, OnUpdateColor);
        }

        public void OnUpdateText(object val)
        {
            var newText = this.textConverter.Convert(val, this);

            if (string.IsNullOrEmpty(this.format))
            {
                this.text.text = newText;
            }
            else
            {
                this.text.text = string.Format(this.format, newText);
            }
        }

        public void OnUpdateColor(object val)
        {
            this.text.color = this.colorConverter.Convert(val, this);
        }
    }
}