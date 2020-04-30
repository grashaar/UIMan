using System;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
    [Serializable]
    public sealed class DropdownOptionConverter : Converter<Dropdown.OptionData, DropdownOptionAdapter>
    {
        public DropdownOptionConverter(string label) : base(label) { }

        public override Dropdown.OptionData Convert(object value, UnityEngine.Object context)
            => DropdownOptionAdapter.Convert(value, true, context);
    }
}