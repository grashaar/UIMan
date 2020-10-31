using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
#endif

namespace UnuGames
{
    [CustomEditor(typeof(UIActivity), true)]
    public class UIActivityInspector :
#if ODIN_INSPECTOR
        OdinEditor
#else
        Editor
#endif
    {
        public override void OnInspectorGUI()
        {
            var uiActivity = (UIActivity)this.target;
            var orgBgColor = GUI.backgroundColor;

            GUI.backgroundColor = CommonColor.LightOrange;
            GUILayout.BeginHorizontal("Box");
            LabelHelper.HeaderLabel(string.Format("UIMan View Model (Activity)"));
            GUILayout.EndHorizontal();

            GUI.backgroundColor = orgBgColor;

            LineHelper.Draw(CommonColor.GetGray());

            EditorGUILayout.Space();
            LabelHelper.HeaderLabel("General");
            GUILayout.BeginVertical("Box");

            EditorGUI.BeginChangeCheck();

            var cover = EditorGUILayout.ObjectField("Cover", uiActivity.cover, typeof(Image), true) as Image;

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(uiActivity, nameof(cover));
                uiActivity.cover = cover;
            }

            EditorGUI.BeginChangeCheck();

            var icon = EditorGUILayout.ObjectField("Icon", uiActivity.icon, typeof(GameObject), true) as GameObject;

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(uiActivity, nameof(icon));
                uiActivity.icon = icon;
            }

            if (!uiActivity.useBackgroundBinding)
            {
                EditorGUI.BeginChangeCheck();

                var background = EditorGUILayout.ObjectField("Background", uiActivity.background, typeof(Image), true) as Image;

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(uiActivity, nameof(background));
                    uiActivity.background = background;
                }
            }

            EditorGUI.BeginChangeCheck();

            var useBackgroundBinding = EditorGUILayout.Toggle("Use Background Binding", uiActivity.useBackgroundBinding);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(uiActivity, nameof(useBackgroundBinding));
                uiActivity.useBackgroundBinding = useBackgroundBinding;
            }

            EditorGUI.BeginChangeCheck();

            var canFade = EditorGUILayout.Toggle("Can Fade", uiActivity.canFade);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(uiActivity, nameof(canFade));
                uiActivity.canFade = canFade;
            }

            if (canFade)
            {
                EditorGUILayout.LabelField("Duration");
                EditorGUI.indentLevel += 1;

                EditorGUI.BeginChangeCheck();

                var showDuration = EditorGUILayout.FloatField("Show", uiActivity.showDuration);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(uiActivity, nameof(showDuration));
                    uiActivity.showDuration = Mathf.Max(showDuration, 0f);
                }

                EditorGUI.BeginChangeCheck();

                var hideDuration = EditorGUILayout.FloatField("Hide", uiActivity.hideDuration);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(uiActivity, nameof(hideDuration));
                    uiActivity.hideDuration = Mathf.Max(hideDuration, 0f);
                }

                EditorGUI.indentLevel -= 1;
            }

            EditorGUI.BeginChangeCheck();

            var hideDelay = EditorGUILayout.FloatField("Hide Delay", uiActivity.hideDelay);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(uiActivity, nameof(hideDelay));
                uiActivity.hideDelay = Mathf.Max(hideDelay, 0f);
            }

            GUILayout.Space(2f);
            GUILayout.EndVertical();

            if (this.target.GetType() == typeof(UIActivity))
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

            EditorGUILayout.Space();

            if (ColorButton.Draw("Edit Handler (View Logic)", CommonColor.LightGreen, GUILayout.Height(25)))
            {
                var handler = UIManCodeGenerator.GetScriptPathByType(this.target.GetType());
                handler = handler.Replace(".cs", ".Handler.cs");

                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(handler, 1, 1);
            }

            if (ColorButton.Draw("Edit View", CommonColor.LightGreen, GUILayout.Height(25)))
            {
                AssetDatabase.OpenAsset(this.target);
            }

            if (ColorButton.Draw("Edit View (Within Context)", CommonColor.LightGreen, GUILayout.Height(25)))
            {
                GameObject prefabInstance;
                Object obj = FindObjectOfType(uiActivity.Type);

                if (obj != null)
                {
                    prefabInstance = ((MonoBehaviour)obj).gameObject;
                }
                else
                {
                    prefabInstance = PrefabUtility.InstantiatePrefab(uiActivity.gameObject) as GameObject;
                    prefabInstance.transform.SetParent(UIMan.Instance.activityRoot, false);
                }
                Selection.activeGameObject = prefabInstance;
            }
        }
    }
}