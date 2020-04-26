using System;

namespace UnuGames.MVVM
{
    [Serializable]
    public sealed class BoolConverter : Converter<bool, BoolAdapter>
    {
        public BoolConverter(string label) : base(label) { }

        public override bool Convert(object value, UnityEngine.Object context)
            => BoolAdapter.Convert(value, false, context);
    }
}