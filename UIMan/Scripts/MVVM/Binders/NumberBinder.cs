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
        public BindingField valueField = new BindingField("Number");

        public string format;
        public float timeChange = 0.25f;

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.text = GetComponent<Text>();
            SubscribeOnChangedEvent(this.valueField, OnUpdateNumber);
        }

        public void OnUpdateNumber(object newVal)
        {
            if (newVal == null)
                return;

            if (!double.TryParse(this.text.text, out var val))
            {
                UnuLogger.LogError($"Cannot convert {this.text.text} to number.", this);
                val = 0.0;
            }

            if (!(newVal is float change))
            {
                if (!(newVal is double changeD))
                {
                    if (!double.TryParse(newVal.ToString(), out changeD))
                    {
                        UnuLogger.LogError($"Cannot convert {newVal} to number.", this);
                        changeD = 0.0;
                    }
                }

                change = (float)changeD;
            }

            UITweener.Value(this.gameObject, this.timeChange, (float)val, change)
                     .SetOnUpdate(OnUpdate)
                     .SetOnComplete(() => OnComplete(newVal));
        }

        private void OnUpdate(float val)
        {
            if (string.IsNullOrEmpty(this.format))
            {
                this.text.text = val.ToString();
            }
            else
            {
                this.text.text = string.Format(this.format, val);
            }
        }

        private void OnComplete(object newNumber)
        {
            var text = newNumber.ToString();

            if (string.IsNullOrEmpty(this.format))
            {
                this.text.text = text;
            }
            else
            {
                this.text.text = string.Format(this.format, text);
            }
        }
    }
}