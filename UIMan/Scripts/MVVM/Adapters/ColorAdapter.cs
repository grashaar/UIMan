using UnityEngine;

namespace UnuGames.MVVM
{
    [CreateAssetMenu(menuName = "UIMan/Adapters/Color Adapter")]
    public class ColorAdapter : Adapter<Color>
    {
        [SerializeField]
        private Color defaultValue = Color.white;

        [SerializeField]
        private bool tryParse = false;

        public override Color Convert(object value, Object context)
            => Convert(value, this.defaultValue, this.tryParse, context);

        public static Color Convert(object value, Color defaultValue, bool tryParse, Object context)
        {
            if (!(value is Color val))
            {
                if (tryParse)
                {
                    val = TryParse(value, defaultValue, context);
                }
                else
                {
                    UnuLogger.LogError($"Cannot convert '{value}' to Color.", context);
                    val = defaultValue;
                }
            }

            return val;
        }

        private static Color TryParse(object value, Color defaultValue, Object context)
        {
            if (!(value is string valStr))
            {
                valStr = value?.ToString();
            }

            if (valStr.Length > 0 && valStr[0] != '#')
            {
                valStr = $"#{valStr}";
            }

            if (!ColorUtility.TryParseHtmlString(valStr, out var val))
            {
                UnuLogger.LogError($"Cannot convert {value} to Color.", context);
                val = defaultValue;
            }

            return val;
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("UIMan/Adapters/Color Adapter")]
        private static void CreateColorAdapterAsset()
            => CreateAdapter<ColorAdapter>(nameof(ColorAdapter));
#endif
    }
}