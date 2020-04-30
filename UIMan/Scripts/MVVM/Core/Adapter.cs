using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace UnuGames.MVVM
{
    public abstract class Adapter : ScriptableObject { }

    public abstract class Adapter<T> : Adapter
    {
        public abstract T Convert(object value, Object context);

#if UNITY_EDITOR
        /// <summary>
        /// Create an adapter asset. Only use in editor mode.
        /// </summary>
        /// <typeparam name="TAdapter"></typeparam>
        /// <param name="name"></param>
        protected static void CreateAdapter<TAdapter>(string name) where TAdapter : Adapter<T>
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            else if (!string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            var asset = CreateInstance<TAdapter>();
            var uniquePath = AssetDatabase.GenerateUniqueAssetPath($"{path}/{name}.asset");

            AssetDatabase.CreateAsset(asset, uniquePath);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
#endif
    }
}