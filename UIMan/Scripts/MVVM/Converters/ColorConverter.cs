using System;
using UnityEngine;

namespace UnuGames.MVVM
{
    [Serializable]
    public sealed class ColorConverter : Converter<Color, ColorAdapter>
    {
        public ColorConverter(string label) : base(label) { }

        public override Color Convert(object value, UnityEngine.Object context)
            => ColorAdapter.Convert(value, Color.white, true, context);
    }
}