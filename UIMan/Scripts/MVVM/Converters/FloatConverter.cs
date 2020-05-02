using System;

namespace UnuGames.MVVM
{
    [Serializable]
    public sealed class FloatConverter : Converter<float, FloatAdapter>
    {
        public FloatConverter(string label) : base(label) { }

        protected override float ConvertWithoutAdapter(object value, UnityEngine.Object context)
            => FloatAdapter.Convert(value, 0f, context);
    }
}