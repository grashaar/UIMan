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
        public BindingField formatField = new BindingField("Format");

        [HideInInspector]
        public StringConverter textConverter = new StringConverter("Text");

        [HideInInspector]
        public ColorConverter colorConverter = new ColorConverter("Color");

        [HideInInspector]
        public StringConverter formatConverter = new StringConverter("Format");

        public string format;

        private string value = string.Empty;

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.text = GetComponent<Text>();

            SubscribeOnChangedEvent(this.textField, OnUpdateText);
            SubscribeOnChangedEvent(this.colorField, OnUpdateColor);
            SubscribeOnChangedEvent(this.formatField, OnUpdateFormat);
        }

        private void OnUpdateText(object val)
        {
            SetValue(this.textConverter.Convert(val, this));
        }

        private void OnUpdateColor(object val)
        {
            this.text.color = this.colorConverter.Convert(val, this);
        }

        private void OnUpdateFormat(object val)
        {
            this.format = this.formatConverter.Convert(val, this);
            SetValue(this.value);
        }

        private void SetValue(string value)
        {
            this.value = value;

            if (string.IsNullOrEmpty(this.format))
            {
                this.text.text = value;
            }
            else
            {
                this.text.text = string.Format(this.format, value);
            }
        }
    }
}