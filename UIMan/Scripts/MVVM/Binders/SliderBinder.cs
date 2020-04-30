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
        public BindingField valueField = new BindingField("Value");

        [HideInInspector]
        public TwoWayBinding onValueChanged = new TwoWayBinding("On Value Changed");

        [HideInInspector]
        public FloatConverter valueConverter = new FloatConverter("Value");

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.slider = GetComponent<Slider>();

            SubscribeOnChangedEvent(this.valueField, OnUpdateValue);

            OnValueChanged_OnChanged(this.onValueChanged);
            this.onValueChanged.onChanged += OnValueChanged_OnChanged;
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