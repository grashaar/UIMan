using UnityEngine;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
    [RequireComponent(typeof(Scrollbar))]
    [DisallowMultipleComponent]
    public class ScrollbarBinder : BinderBase
    {
        protected Scrollbar scrollbar;

        [HideInInspector]
        public BindingField valueField = new BindingField("Value");

        [HideInInspector]
        public TwoWayBindingFloat onValueChanged = new TwoWayBindingFloat("On Value Changed");

        [HideInInspector]
        public FloatConverter valueConverter = new FloatConverter("Value");

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.scrollbar = GetComponent<Scrollbar>();

            SubscribeOnChangedEvent(this.valueField, OnUpdateValue);

            OnValueChanged_OnChanged(this.onValueChanged);
            this.onValueChanged.onChanged += OnValueChanged_OnChanged;
        }

        private void OnUpdateValue(object val)
        {
            var value = this.valueConverter.Convert(val, this);
            this.scrollbar.SetValueWithoutNotify(value);
        }

        private void OnValueChanged(float value)
        {
            SetValue(this.valueField, this.onValueChanged.converter.Convert(value, this));
        }

        private void OnValueChanged_OnChanged(bool value)
        {
            this.scrollbar.onValueChanged.RemoveListener(OnValueChanged);

            if (value)
                this.scrollbar.onValueChanged.AddListener(OnValueChanged);
        }
    }
}