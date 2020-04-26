using UnityEngine;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
    [RequireComponent(typeof(InputField))]
    [DisallowMultipleComponent]
    public class InputFieldBinder : BinderBase
    {
        protected InputField input;

        [HideInInspector]
        public BindingField valueField = new BindingField("Text");

        [HideInInspector]
        public StringConverter valueConverter = new StringConverter("Text");

        private string oldText = "";

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.input = GetComponent<InputField>();

            SubscribeOnChangedEvent(this.valueField, OnUpdateText);
        }

        public void OnUpdateText(object newText)
        {
            var text = this.valueConverter.Convert(newText, this);
            this.input.text = text;
        }

        private void Update()
        {
            if (!string.Equals(this.input.text, this.oldText))
            {
                this.oldText = this.input.text;
                this.oldText.Replace("\t", string.Empty);
                this.input.text = this.oldText;

                SetValue(this.valueField.member, this.oldText);
            }
        }
    }
}