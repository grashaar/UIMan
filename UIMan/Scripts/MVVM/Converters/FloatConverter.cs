using System;

namespace UnuGames.MVVM
{
    [Serializable]
    public sealed class FloatConverter : Converter<float, FloatAdapter>
    {
        public FloatConverter(string label) : base(label) { }

        public override float Convert(object value, UnityEngine.Object context)
            => FloatAdapter.Convert(value, 0f, context);
    }
}