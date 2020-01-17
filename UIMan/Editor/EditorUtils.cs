using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace UnuGames
{
    public class EditorUtils : Editor
    {
        public static void CreatePath(string path)
        {
            var root = Application.dataPath;
            var folders = Regex.Split(path, "/");
            var curFolder = root + "/";

            for (var i = 0; i < folders.Length; i++)
            {
                if (i == 0 && folders[i] == "Assets")
                    continue;
                curFolder += "/" + folders[i];

                if (!Directory.Exists(curFolder))
                {
                    Directory.CreateDirectory(curFolder);
                }
            }
        }

        public static void OverwriteAsset<T>(string path, string newPath) where T : Object
        {
            if (path != newPath)
            {
                if (AssetDatabase.LoadAssetAtPath<T>(newPath) != null)
                    AssetDatabase.DeleteAsset(newPath);

                AssetDatabase.CopyAsset(path, newPath);
            }
        }
    }
}