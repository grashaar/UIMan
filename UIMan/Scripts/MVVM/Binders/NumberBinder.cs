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

        [HideInInspector]
        public FloatConverter valueConverter = new FloatConverter("Number");

        public string format;
        public float timeChange = 0.25f;

        private float value = 0f;

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.text = GetComponent<Text>();
            SubscribeOnChangedEvent(this.valueField, OnUpdateNumber);
        }

        private void OnUpdateNumber(object newVal)
        {
            var oldValue = this.value;
            var newValue = this.valueConverter.Convert(newVal, this);

            UITweener.Value(this.gameObject, this.timeChange, oldValue, newValue)
                     .SetOnUpdate(SetValue)
                     .SetOnComplete(() => SetValue(newValue));
        }

        private void SetValue(float value)
        {
            this.value = value;

            if (string.IsNullOrEmpty(this.format))
            {
                this.text.text = value.ToString();
            }
            else
            {
                this.text.text = string.Format(this.format, value);
            }
        }
    }
}