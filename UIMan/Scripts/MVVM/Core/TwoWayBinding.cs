using System;
using UnityEngine;

namespace UnuGames.MVVM
{
    [Serializable]
    public sealed class TwoWayBinding
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

        public static implicit operator bool(TwoWayBinding value)
            => value?.m_value ?? false;
    }
}