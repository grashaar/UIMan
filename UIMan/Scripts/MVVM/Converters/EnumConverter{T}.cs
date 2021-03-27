using System;

namespace UnuGames.MVVM
{
    [Serializable]
    public class EnumConverter<T> : Converter<T, EnumAdapter<T>> where T : unmanaged, Enum
    {
        public EnumConverter(string label) : base(label) { }

        protected override T ConvertWithoutAdapter(object value, UnityEngine.Object context)
            => EnumAdapter<T>.Convert(value, true, true, default, context);

        protected override object ConvertWithoutAdapter(T value, UnityEngine.Object context)
            => EnumAdapter<T>.Convert(value);
    }
}