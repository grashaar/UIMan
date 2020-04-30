using System;
using UnityEngine;

namespace UnuGames.MVVM
{
    [Serializable]
    public abstract class Converter<TValue, TAdapter> : Converter
        where TAdapter : Adapter<TValue>
    {
        [SerializeField]
        private TAdapter m_adapter = null;

        public TAdapter adapter
            => this.m_adapter;

        public Converter(string label) : base(label) { }

        public abstract TValue Convert(object value, UnityEngine.Object context);

        public sealed override Adapter GetAdapter()
            => this.m_adapter;

        public sealed override Type GetAdapterType()
            => typeof(TAdapter);

        public sealed override void SetAdapter(Adapter adapter)
            => this.m_adapter = adapter is TAdapter adapterT ? adapterT : null;
    }
}