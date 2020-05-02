using System;

namespace UnuGames.MVVM
{
    [Serializable]
    public sealed class StringConverter : Converter<string, StringAdapter>
    {
        public StringConverter(string label) : base(label) { }

        protected override string ConvertWithoutAdapter(object value, UnityEngine.Object context)
            => StringAdapter.Convert(value, string.Empty, true, context);
    }
}