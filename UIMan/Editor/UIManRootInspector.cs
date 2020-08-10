using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UnuGames
{
    [CustomEditor(typeof(UIMan))]
    public class UIManInspector : Editor
    {
        private readonly GUIContent uiRoot = new GUIContent("UI Root", "The root transform of all UI element");
        private readonly GUIContent screenRoot = new GUIContent("Screen Root", "The root transform of all Screen");
        private readonly GUIContent dialogRoot = new GUIContent("Dialog Root", "The root transform of all Dialog");
        private readonly GUIContent activityRoot = new GUIContent("Activity Root", "The root transform of all Activity");
        private readonly GUIContent backgroundImg = new GUIContent("Background", "The Image to render background of any Screen");
        private readonly GUIContent coverTrans = new GUIContent("Cover", "The transform of object cover all UI behind Dialog");

        public override void OnInspectorGUI()
        {
            var uiManager = (UIMan)this.target;

            GUILayout.BeginHorizontal("Box");
            LabelHelper.HeaderLabel("UIMan Root");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box");

            EditorGUI.BeginChangeCheck();
            var uiRoot = EditorGUILayout.ObjectField(this.uiRoot, uiManager.uiRoot, typeof(Transform), true) as Transform;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(this.target, nameof(uiRoot));
                uiManager.uiRoot = uiRoot;
            }

            EditorGUI.BeginChangeCheck();
            var screenRoot = EditorGUILayout.ObjectField(this.screenRoot, uiManager.screenRoot, typeof(Transform), true) as Transform;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(this.target, nameof(screenRoot));
                uiManager.screenRoot = screenRoot;
            }

            EditorGUI.BeginChangeCheck();
            var dialogRoot = EditorGUILayout.ObjectField(this.dialogRoot, uiManager.dialogRoot, typeof(Transform), true) as Transform;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(this.target, nameof(dialogRoot));
                uiManager.dialogRoot = dialogRoot;
            }

            EditorGUI.BeginChangeCheck();
            var activityRoot = EditorGUILayout.ObjectField(this.activityRoot, uiManager.activityRoot, typeof(Transform), true) as Transform;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(this.target, nameof(activityRoot));
                uiManager.activityRoot = activityRoot;
            }

            EditorGUI.BeginChangeCheck();
            var background = EditorGUILayout.ObjectField(this.backgroundImg, uiManager.background, typeof(Image), true) as Image;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(this.target, nameof(background));
                uiManager.background = background;
            }

            EditorGUI.BeginChangeCheck();
            var cover = EditorGUILayout.ObjectField(this.coverTrans, uiManager.cover, typeof(RectTransform), true) as RectTransform;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(this.target, nameof(cover));
                uiManager.cover = cover;
            }

            GUILayout.EndHorizontal();
        }
    }
}