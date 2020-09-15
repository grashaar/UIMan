using UnityEngine;

namespace UnuGames.MVVM
{
    [CreateAssetMenu(menuName = "UIMan/Adapters/Int Adapter")]
    public class IntAdapter : Adapter<int>
    {
        [SerializeField]
        private int defaultValue = 0;

        public override int Convert(object value, Object context)
            => Convert(value, this.defaultValue, context);

        public static int Convert(object value, int defaultValue, Object context)
        {
            if (!(value is int val))
            {
                if (!int.TryParse(value?.ToString(), out val))
                {
                    UnuLogger.LogError($"Cannot convert '{value}' to int.", context);
                    val = defaultValue;
                }
            }

            return val;
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("UIMan/Adapters/Int Adapter")]
        private static void CreateIntAdapterAsset()
            => CreateAdapter<IntAdapter>(nameof(IntAdapter));
#endif
    }
}