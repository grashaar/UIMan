using System;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
    [Serializable]
    public sealed class DropdownOptionConverter : Converter<Dropdown.OptionData, DropdownOptionAdapter>
    {
        public DropdownOptionConverter(string label) : base(label) { }

        protected override Dropdown.OptionData ConvertWithoutAdapter(object value, UnityEngine.Object context)
            => DropdownOptionAdapter.Convert(value, true, context);

        protected override object ConvertWithoutAdapter(Dropdown.OptionData value, UnityEngine.Object context)
            => DropdownOptionAdapter.Convert(value);
    }
}