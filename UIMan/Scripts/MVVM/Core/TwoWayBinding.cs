using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UnuGames.MVVM
{
    [Serializable]
    public class TwoWayBinding
    {
        public string label;

        [SerializeField]
        private bool m_value;

        public bool value
        {
            get => this.m_value;

            set
            {
                var oldVal = this.m_value;
                this.m_value = value;

                if (value != oldVal)
                    this.onChanged?.Invoke(value);
            }
        }

        public event Action<bool> onChanged;

        public TwoWayBinding(string label)
        {
            this.label = label;
        }

        private Converter[] converters;

        public Converter[] GetConverters()
        {
#if !UNITY_EDITOR
            if (this.converters != null)
                return  this.converters;
#endif

            var listConverter = new List<Converter>();
            MemberInfo[] members = GetType().GetMembers();
            var converterType = typeof(Converter);

            for (var i = 0; i < members.Length; i++)
            {
                MemberInfo memberInfo = members[i];

                if (memberInfo.MemberType == MemberTypes.Field)
                {
                    var fieldInfo = memberInfo as FieldInfo;

                    if (converterType.IsAssignableFrom(fieldInfo.FieldType))
                    {
                        listConverter.Add(fieldInfo.GetValue(this) as Converter);
                    }
                }
            }

            this.converters = listConverter.ToArray();
            return this.converters;
        }

        public static implicit operator bool(TwoWayBinding value)
            => value?.m_value ?? false;
    }
}