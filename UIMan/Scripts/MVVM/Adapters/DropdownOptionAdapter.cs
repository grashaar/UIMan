using UnityEngine;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
    [CreateAssetMenu(menuName = "UIMan/Adapters/Dropdown Option Adapter")]
    public class DropdownOptionAdapter : Adapter<Dropdown.OptionData>
    {
        [SerializeField]
        private bool forceToString = true;

        private static readonly Dropdown.OptionData _defaultValue = new Dropdown.OptionData(string.Empty);

        public override Dropdown.OptionData Convert(object value, Object context)
            => Convert(value, this.forceToString, context);

        public static Dropdown.OptionData Convert(object value, bool forceToString, Object context)
        {
            if (value == null)
                return _defaultValue;

            if (!(value is Dropdown.OptionData val))
            {
                if (forceToString)
                {
                    val = new Dropdown.OptionData(value.ToString());
                }
                else
                {
                    UnuLogger.LogError($"Cannot convert '{value}' to string.", context);
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