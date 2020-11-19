using UnityEditor;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
#endif

namespace UnuGames
{
    [CustomEditor(typeof(UITweener), true)]
    public class UITweenerInspector :
#if ODIN_INSPECTOR
        OdinEditor
#else
        Editor
#endif
    {
        public override void OnInspectorGUI()
        {
            var tweener = (UITweener)this.target;
            var orgBgColor = GUI.backgroundColor;

            GUI.backgroundColor = CommonColor.LightOrange;
            GUILayout.BeginHorizontal("Box");
            LabelHelper.HeaderLabel(string.Format("Read-Only Fields (UITweener)"));
            GUILayout.EndHorizontal();

            GUI.backgroundColor = orgBgColor;
            LineHelper.Draw(CommonColor.GetGray());
            EditorGUILayout.Space();

            EditorGUILayout.EnumPopup("Type", tweener.tweenType);
            EditorGUILayout.FloatField("Time", tweener.time);
            EditorGUILayout.FloatField("Elapsed", tweener.elapsed);
            EditorGUILayout.Toggle("Is Ready", tweener.isReady);
            EditorGUILayout.Space();

            if (tweener.tweenType == UITweener.UITweenType.Move)
            {
                EditorGUILayout.Vector3Field("From", tweener.startPosition);
                EditorGUILayout.Vector3Field("To", tweener.endPosition);
                EditorGUILayout.Vector3Field("Current", tweener.currentPosition);
            }
            else
            {
                EditorGUILayout.FloatField("From", tweener.startValue);
                EditorGUILayout.FloatField("To", tweener.endValue);
                EditorGUILayout.FloatField("Current", tweener.currentValue);
            }

            if (this.target.GetType() == typeof(UITweener))
                return;

            EditorGUILayout.Space();
            LabelHelper.HeaderLabel("Custom fields");
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