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
        public BindingField textValue = new BindingField("Text");

        public override void Init(bool forceInit)
        {
            if (CheckInit(forceInit))
            {
                this.input = GetComponent<InputField>();

                SubscribeOnChangedEvent(this.textValue, OnUpdateText);
            }
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
            if (this.input.text != this.oldText)
            {
                this.oldText = this.input.text;
                if (this.oldText.Contains("\t"))
                {
                    this.oldText.Replace("\t", string.Empty);
                }
                this.input.text = this.oldText;
                SetValue(this.textValue.member, this.oldText);
            }
        }

        public void EndEdit()
        {
            this.input.text = this.oldText;
        }
    }
}