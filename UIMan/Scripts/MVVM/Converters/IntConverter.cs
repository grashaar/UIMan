using System;

namespace UnuGames.MVVM
{
    [Serializable]
    public sealed class IntConverter : Converter<int, IntAdapter>
    {
        public IntConverter(string label) : base(label) { }

        public override int Convert(object value, UnityEngine.Object context)
            => IntAdapter.Convert(value, 0, context);
    }
}