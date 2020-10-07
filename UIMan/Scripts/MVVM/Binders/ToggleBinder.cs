using UnityEngine;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
    [RequireComponent(typeof(Toggle))]
    [DisallowMultipleComponent]
    public class ToggleBinder : BinderBase
    {
        public bool ignoreGroup = false;

        protected Toggle toggle;

        [HideInInspector]
        public BindingField valueField = new BindingField("Value");

        [HideInInspector]
        public TwoWayBindingBool onValueChanged = new TwoWayBindingBool("On Value Changed");

        [HideInInspector]
        public BoolConverter valueConverter = new BoolConverter("Value");

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.toggle = GetComponent<Toggle>();

            SubscribeOnChangedEvent(this.valueField, OnUpdateValue);

            OnValueChanged_OnChanged(this.onValueChanged);
            this.onValueChanged.onChanged += OnValueChanged_OnChanged;
        }

        private void OnUpdateValue(object val)
        {
            var value = this.valueConverter.Convert(val, this);
            this.toggle.SetIsOnWithoutNotify(value);
        }

        private void OnValueChanged(bool value)
        {
            if (!this.ignoreGroup && !value && this.toggle.group)
                return;

            SetValue(this.valueField, this.onValueChanged.converter.Convert(value, this));
        }

        private void OnValueChanged_OnChanged(bool value)
        {
            this.toggle.onValueChanged.RemoveListener(OnValueChanged);

            if (value)
                this.toggle.onValueChanged.AddListener(OnValueChanged);
        }
    }
}