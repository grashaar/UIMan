using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
    [RequireComponent(typeof(Dropdown))]
    [DisallowMultipleComponent]
    public class DropdownBinder : BinderBase
    {
        protected Dropdown dropdown;

        [HideInInspector]
        public BindingField optionsField = new BindingField("Option Data Source");

        [HideInInspector]
        public BindingField valueField = new BindingField("Value");

        [HideInInspector]
        public TwoWayBinding onValueChanged = new TwoWayBinding("On Value Changed");

        [HideInInspector]
        public DropdownOptionConverter optionDataConverter = new DropdownOptionConverter("Option Data");

        [HideInInspector]
        public IntConverter valueConverter = new IntConverter("Value");

        private readonly List<Dropdown.OptionData> options
            = new List<Dropdown.OptionData>();

        private MemberInfo optionsMember;
        private IObservaleCollection dataSource;

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            this.optionsMember = this.dataContext.viewModel.GetMemberInfo(this.optionsField.member);

            switch (this.optionsMember)
            {
                case FieldInfo sourceField:
                    this.dataSource = sourceField.GetValue(this.dataContext.viewModel)
                                      as IObservaleCollection;
                    break;

                case PropertyInfo sourceProperty:
                    this.dataSource = sourceProperty.GetValue(this.dataContext.viewModel, null)
                                      as IObservaleCollection;
                    break;
            }

            if (this.dataSource != null)
            {
                this.dataSource.OnAddObject += Options_OnChanged;
                this.dataSource.OnRemoveObject += Options_OnChanged;
                this.dataSource.OnRemoveAt += Options_OnChanged;
                this.dataSource.OnInsertObject += Options_OnChanged;
                this.dataSource.OnClearObjects += Options_OnChanged;
                this.dataSource.OnChangeObject += Options_OnChanged;
            }

            this.dropdown = GetComponent<Dropdown>();

            SubscribeOnChangedEvent(this.valueField, OnUpdateValue);

            OnValueChanged_OnChanged(this.onValueChanged);
            this.onValueChanged.onChanged += OnValueChanged_OnChanged;
        }

        private void OnUpdateValue(object val)
        {
            var value = this.valueConverter.Convert(val, this);
            this.dropdown.SetValueWithoutNotify(value);
        }

        private void OnValueChanged(int value)
        {
            SetValue(this.valueField.member, value);
        }

        private void OnValueChanged_OnChanged(bool value)
        {
            this.dropdown.onValueChanged.RemoveListener(OnValueChanged);

            if (value)
                this.dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        private void Options_OnChanged()
        {
            this.options.Clear();
            this.dropdown.ClearOptions();

            foreach (var data in this.dataSource)
            {
                var option = this.optionDataConverter.Convert(data, this);
                this.options.Add(option);
            }

            this.dropdown.AddOptions(this.options);
        }

        private void Options_OnChanged(int arg1, object arg2)
        {
            Options_OnChanged();
        }

        private void Options_OnChanged(int obj)
        {
            Options_OnChanged();
        }

        private void Options_OnChanged(object obj)
        {
            Options_OnChanged();
        }
    }
}