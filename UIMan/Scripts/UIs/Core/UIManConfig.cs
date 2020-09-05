using UnityEngine;

namespace UnuGames
{
    [CreateAssetMenu(fileName = "UIManConfig", menuName = "UIMan/UIMan Config")]
    public class UIManConfig : ScriptableObject
    {
        public string classNamespace;
        public string screenPrefabFolder;
        public string dialogPrefabFolder;
        public string activityPrefabFolder;
        public string backgroundRootFolder;
        public string animRootFolder;

#if UNITY_EDITOR
        public string modelScriptFolder;
        public string screenScriptFolder;
        public string dialogScriptFolder;
        public string activityScriptFolder;
        public string selectedType;
        public string generatingType;
        public string generatingBaseType;
#endif
    }
}