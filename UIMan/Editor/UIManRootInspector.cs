using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UnuGames
{
    [CustomEditor(typeof(UIMan))]
    public class UIManInspector : Editor
    {
        private readonly GUIContent uiRoot = new GUIContent("UI Root", "The root transform of all UI element");
        private readonly GUIContent screenRoot = new GUIContent("Screen Root", "The root transform of all screen");
        private readonly GUIContent dialogRoot = new GUIContent("Dialog Root", "The root transform of all dialog");
        private readonly GUIContent backgroundImg = new GUIContent("Background", "The Image to render background of any Screen");
        private readonly GUIContent coverTrans = new GUIContent("Cover", "The transform of object cover all UI behind Dialog");

        public override void OnInspectorGUI()
        {
            var uiManager = (UIMan)this.target;

            GUILayout.BeginHorizontal("Box");
            LabelHelper.HeaderLabel("UIMan Root");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box");
            uiManager.uiRoot = EditorGUILayout.ObjectField(this.uiRoot, uiManager.uiRoot, typeof(Transform), true) as Transform;
            uiManager.screenRoot = EditorGUILayout.ObjectField(this.screenRoot, uiManager.screenRoot, typeof(Transform), true) as Transform;
            uiManager.dialogRoot = EditorGUILayout.ObjectField(this.dialogRoot, uiManager.dialogRoot, typeof(Transform), true) as Transform;
            uiManager.background = EditorGUILayout.ObjectField(this.backgroundImg, uiManager.background, typeof(Image), true) as Image;
            uiManager.cover = EditorGUILayout.ObjectField(this.coverTrans, uiManager.cover, typeof(RectTransform), true) as RectTransform;
            GUILayout.EndHorizontal();
        }
    }
}