using UnityEngine;

namespace UnuGames.MVVM
{
    [CreateAssetMenu(menuName = "UIMan/Adapters/Bool Adapter")]
    public class BoolAdapter : Adapter<bool>
    {
        [SerializeField]
        private bool defaultValue = false;

        public override bool Convert(object value, Object context)
            => Convert(value, this.defaultValue, context);

        public static bool Convert(object value, bool defaultValue, Object context)
        {
            if (!(value is bool val))
            {
                if (!bool.TryParse(value?.ToString(), out val))
                {
                    UnuLogger.LogError($"Cannot convert '{value}' to boolean.", context);
                    val = defaultValue;
                }
            }

            return val;
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("UIMan/Adapters/Bool Adapter")]
        private static void CreateBoolAdapterAsset()
            => CreateAdapter<BoolAdapter>(nameof(BoolAdapter));
#endif
    }
}