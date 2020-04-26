using UnityEditor;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
#endif

namespace UnuGames
{
    [CustomEditor(typeof(UIActivityIndicator))]
    public class UIActivityInspector :
#if ODIN_INSPECTOR
        OdinEditor
#else
        Editor
#endif
    {
        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal("Box");
            LabelHelper.HeaderLabel("UIMan Activity Indicator");
            GUILayout.EndHorizontal();

#if ODIN_INSPECTOR
            EditorGUILayout.Space();
            base.OnInspectorGUI();
#else
            GUILayout.BeginVertical("Box");
            DrawDefaultInspector();
            GUILayout.EndVertical();
#endif
        }
    }
}