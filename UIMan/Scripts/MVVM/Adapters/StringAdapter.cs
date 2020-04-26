using UnityEngine;

namespace UnuGames.MVVM
{
    [CreateAssetMenu(menuName = "UIMan/Adapters/String Adapter")]
    public class StringAdapter : Adapter<string>
    {
        [SerializeField]
        private string defaultValue = string.Empty;

        [SerializeField]
        private bool forceToString = true;

        public override string Convert(object value, Object context)
            => Convert(value, this.defaultValue, this.forceToString, context);

        public static string Convert(object value, string defaultValue, bool forceToString, Object context)
        {
            if (value == null)
                return defaultValue;

            if (!(value is string val))
            {
                if (forceToString)
                {
                    val = value.ToString();
                }
                else
                {
                    UnuLogger.LogError($"Cannot convert {value} to string.", context);
                    val = defaultValue;
                }
            }

            return val;
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("UIMan/Adapters/String Adapter")]
        private static void CreateStringAdapterAsset()
            => CreateAdapter<StringAdapter>(nameof(StringAdapter));
#endif
    }
}