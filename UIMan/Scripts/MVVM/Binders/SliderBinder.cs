using UnityEngine;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
    [RequireComponent(typeof(Slider))]
    [DisallowMultipleComponent]
    public class SliderBinder : BinderBase
    {
        protected Slider slider;

        [HideInInspector]
        public BindingField minField = new BindingField("Min");

        [HideInInspector]
        public BindingField maxField = new BindingField("Max");

        [HideInInspector]
        public BindingField valueField = new BindingField("Value");

        [HideInInspector]
        public TwoWayBinding onValueChanged = new TwoWayBinding("On Value Changed");

        [HideInInspector]
        public FloatConverter minConverter = new FloatConverter("Min");

        [HideInInspector]
        public FloatConverter maxConverter = new FloatConverter("Max");

        [HideInInspector]
        public FloatConverter valueConverter = new FloatConverter("Value");

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.slider = GetComponent<Slider>();

            SubscribeOnChangedEvent(this.minField, OnUpdateMin);
            SubscribeOnChangedEvent(this.maxField, OnUpdateMax);
            SubscribeOnChangedEvent(this.valueField, OnUpdateValue);

            OnValueChanged_OnChanged(this.onValueChanged);
            this.onValueChanged.onChanged += OnValueChanged_OnChanged;
        }

        private void OnUpdateMin(object val)
        {
            var value = this.minConverter.Convert(val, this);
            this.slider.minValue = value;
        }

        private void OnUpdateMax(object val)
        {
            var value = this.maxConverter.Convert(val, this);
            this.slider.maxValue = value;
        }

        private void OnUpdateValue(object val)
        {
            var value = this.valueConverter.Convert(val, this);
            this.slider.SetValueWithoutNotify(value);
        }

        private void OnValueChanged(float value)
        {
            SetValue(this.valueField.member, value);
        }

        private void OnValueChanged_OnChanged(bool value)
        {
            this.slider.onValueChanged.RemoveListener(OnValueChanged);

            if (value)
                this.slider.onValueChanged.AddListener(OnValueChanged);
        }
    }
}