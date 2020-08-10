using UnityEditor;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
#endif

namespace UnuGames
{
    [CustomEditor(typeof(UIActivity))]
    public class UIActivityInspector :
#if ODIN_INSPECTOR
        OdinEditor
#else
        Editor
#endif
    {
        public override void OnInspectorGUI()
        {
            var orgBgColor = GUI.backgroundColor;

            GUI.backgroundColor = CommonColor.LightOrange;
            GUILayout.BeginHorizontal("Box");
            LabelHelper.HeaderLabel(string.Format("UIMan Activity"));
            GUILayout.EndHorizontal();

            GUI.backgroundColor = orgBgColor;

            LineHelper.Draw(Color.gray);

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