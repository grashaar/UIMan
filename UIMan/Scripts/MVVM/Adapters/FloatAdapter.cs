using UnityEngine;

namespace UnuGames.MVVM
{
    [CreateAssetMenu(menuName = "UIMan/Adapters/Float Adapter")]
    public class FloatAdapter : Adapter<float>
    {
        [SerializeField]
        private float defaultValue = 0f;

        public override float Convert(object value, Object context)
            => Convert(value, this.defaultValue, context);

        public static float Convert(object value, float defaultValue, Object context)
        {
            if (value == null)
                return defaultValue;

            if (!(value is float valFloat))
            {
                if (value is double valDouble ||
                    double.TryParse(value.ToString(), out valDouble))
                {
                    valFloat = (float)valDouble;
                }
                else
                {
                    UnuLogger.LogError($"Cannot convert {value} to float.", context);
                    valFloat = defaultValue;
                }
            }

            return valFloat;
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("UIMan/Adapters/Float Adapter")]
        private static void CreateFloatAdapterAsset()
            => CreateAdapter<FloatAdapter>(nameof(FloatAdapter));
#endif
    }
}