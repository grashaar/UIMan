using UnityEngine;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
    [RequireComponent(typeof(Slider))]
    [DisallowMultipleComponent]
    public class SliderBinder : BinderBase
    {
        public float duration = 0.1f;

        [Tooltip("Rescale the value to [0, 1]")]
        public bool normalizeValue;

        protected Slider slider;

        [HideInInspector]
        public BindingField minField = new BindingField("Min");

        [HideInInspector]
        public BindingField maxField = new BindingField("Max");

        [HideInInspector]
        public BindingField valueField = new BindingField("Value");

        [HideInInspector]
        public TwoWayBindingFloat onValueChanged = new TwoWayBindingFloat("On Value Changed");

        [HideInInspector]
        public BindingField durationField = new BindingField("Duration");

        [HideInInspector]
        public FloatConverter minConverter = new FloatConverter("Min");

        [HideInInspector]
        public FloatConverter maxConverter = new FloatConverter("Max");

        [HideInInspector]
        public FloatConverter valueConverter = new FloatConverter("Value");

        [HideInInspector]
        public FloatConverter durationConverter = new FloatConverter("Duration");

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.slider = GetComponent<Slider>();

            SubscribeOnChangedEvent(this.minField, OnUpdateMin);
            SubscribeOnChangedEvent(this.maxField, OnUpdateMax);
            SubscribeOnChangedEvent(this.valueField, OnUpdateValue);
            SubscribeOnChangedEvent(this.durationField, OnUpdateDuration);

            OnValueChanged_OnChanged(this.onValueChanged);
            this.onValueChanged.onChanged += OnValueChanged_OnChanged;
        }

        private float OffsetMax()
        {
            return this.slider.maxValue - this.slider.minValue;
        }

        private float Normalize(float val)
        {
            return (val - this.slider.minValue) / OffsetMax();
        }

        private float Denormalize(float val)
        {
            return val * OffsetMax() + this.slider.minValue;
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
            var valChange = this.valueConverter.Convert(val, this);

            if (this.normalizeValue)
                valChange = Denormalize(valChange);

            if (this.duration <= 0f)
            {
                SetValue(valChange);
                return;
            }

            UITweener.Value(this.gameObject, this.duration, this.slider.value, valChange)
                     .SetOnUpdate(SetValue);
        }

        private void OnUpdateDuration(object val)
        {
            this.duration = this.durationConverter.Convert(val, this);
        }

        private void OnValueChanged(float value)
        {
            if (this.normalizeValue)
                value = Normalize(value);

            SetValue(this.valueField, this.onValueChanged.converter.Convert(value, this));
        }

        private void OnValueChanged_OnChanged(bool value)
        {
            this.slider.onValueChanged.RemoveListener(OnValueChanged);

            if (value)
                this.slider.onValueChanged.AddListener(OnValueChanged);
        }

        private void SetValue(float val)
        {
            this.slider.SetValueWithoutNotify(val);
        }
    }
}