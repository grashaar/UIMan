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

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.input = GetComponent<InputField>();

            SubscribeOnChangedEvent(this.valueField, OnUpdateText);
        }

        public void OnUpdateText(object newText)
        {
            //if(newText == null)
            //	return;
            //input.text = newText.ToString();
        }

        private string oldText = "";

        private void Update()
        {
            if (string.Equals(this.input.text, this.oldText))
            {
                this.oldText = this.input.text;
                this.oldText.Replace("\t", string.Empty);

                this.input.text = this.oldText;
                SetValue(this.valueField.member, this.oldText);
            }
        }

        public void EndEdit()
        {
            this.input.text = this.oldText;
        }
    }
}