using UnityEngine;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
    using OptionData = Dropdown.OptionData;

    [CreateAssetMenu(menuName = "UIMan/Adapters/Dropdown Option Adapter")]
    public class DropdownOptionAdapter : Adapter<OptionData>
    {
        [SerializeField]
        private bool forceToString = true;

        private static readonly OptionData _defaultValue = new OptionData(string.Empty);

        public override OptionData Convert(object value, Object context)
            => Convert(value, this.forceToString, context);

        public static OptionData Convert(object value, bool forceToString, Object context)
        {
            if (!(value is OptionData val))
            {
                if (forceToString)
                {
                    val = new OptionData(value?.ToString() ?? string.Empty);
                }
                else
                {
                    UnuLogger.LogError($"Cannot convert '{value}' to {nameof(Dropdown)}.{nameof(OptionData)}", context);
                    val = _defaultValue;
                }
            }

            return val;
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("UIMan/Adapters/Dropdown Option Adapter")]
        private static void CreateDropdownOptionAdapterAsset()
            => CreateAdapter<DropdownOptionAdapter>(nameof(DropdownOptionAdapter));
#endif
    }
}