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
            var newText = val == null ? string.Empty : val.ToString();

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
            if (val == null)
                return;

            if (!(val is Color valChange))
                return;

            this.text.color = valChange;
        }
    }
}