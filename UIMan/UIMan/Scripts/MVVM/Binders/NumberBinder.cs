using UnityEngine;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
    [RequireComponent(typeof(Text))]
    [DisallowMultipleComponent]
    public class NumberBinder : BinderBase
    {
        protected Text text;

        [HideInInspector]
        public BindingField textValue = new BindingField("Text");

        public string format;
        public float timeChange = 0.25f;

        public override void Init(bool forceInit)
        {
            if (CheckInit(forceInit))
            {
                this.text = GetComponent<Text>();
                SubscribeOnChangedEvent(this.textValue, OnUpdateText);
            }
        }

        public void OnUpdateText(object newText)
        {
            if (newText == null)
                return;

            double.TryParse(this.text.text, out var val);

            double.TryParse(newText.ToString(), out var change);

            UITweener.Value(this.gameObject, this.timeChange, (float)val, (float)change).SetOnUpdate(UpdateText).SetOnComplete(() => {
                this.text.text = newText.ToString();
                                                                                                                           });
        }

        private void UpdateText(float val)
        {
            var valueChange = (long)val;

            if (string.IsNullOrEmpty(this.format))
            {
                this.text.text = valueChange.ToString();
            }
            else
            {
                this.text.text = string.Format(this.format, valueChange.ToString());
            }
        }
    }
}