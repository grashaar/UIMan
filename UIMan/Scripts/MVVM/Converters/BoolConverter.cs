using System;

namespace UnuGames.MVVM
{
    [Serializable]
    public sealed class BoolConverter : Converter<bool, BoolAdapter>
    {
        public BoolConverter(string label) : base(label) { }

        protected override bool ConvertWithoutAdapter(object value, UnityEngine.Object context)
            => BoolAdapter.Convert(value, false, context);
    }
}