using System;
using UnityEngine;

namespace UnuGames.MVVM
{
    [Serializable]
    public sealed class ColorConverter : Converter<Color, ColorAdapter>
    {
        public ColorConverter(string label) : base(label) { }

        protected override Color ConvertWithoutAdapter(object value, UnityEngine.Object context)
            => ColorAdapter.Convert(value, Color.white, true, context);

        protected override object ConvertWithoutAdapter(Color value, UnityEngine.Object context)
            => ColorAdapter.Convert(value);
    }
}