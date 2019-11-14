using UnityEditor;
using UnityEngine;

namespace UnuGames
{
    public class ConfigFile
    {
        public static void Create<T>() where T : ScriptableObject
        {
            var files = AssetDatabase.FindAssets(typeof(T).Name + ".asset");
            if (files == null || files.Length == 0)
            {
                ScriptableObjectUtility.CreateAsset<T>();
            }
            else
            {
                var configFile = AssetDatabase.GUIDToAssetPath(files[0]);
                Object configObj = AssetDatabase.LoadAssetAtPath<Object>(configFile);
                Selection.activeObject = configObj;
            }
        }
    }
}