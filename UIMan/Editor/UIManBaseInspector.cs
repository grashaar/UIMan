using UnityEditor;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
#endif

namespace UnuGames
{
    [CustomEditor(typeof(UIManBase), true)]
    public class UIManBaseInspector :
#if ODIN_INSPECTOR
        OdinEditor
#else
        Editor
#endif
    {
        private readonly GUIContent animator = new GUIContent("Animator", "Animator component to do custom animation");
        private readonly GUIContent show = new GUIContent("Show", "Animation to do when UI is show");
        private readonly GUIContent showTime = new GUIContent("Time", "Total time to do built-in or custom script animation");
        private readonly GUIContent hide = new GUIContent("Hide", "Animation to do when UI is hide");
        private readonly GUIContent hideTime = new GUIContent("Time", "Total time to do built-in or custom script animation");
        private readonly GUIContent idle = new GUIContent("Idle", "Animation to do when UI is idle");
        private readonly GUIContent position = new GUIContent("Show Position", "Target position to show UI");
        private readonly GUIContent cover = new GUIContent("Use Cover", "Show gray cover after dialog to prevent click behind elements");
        private readonly GUIContent background = new GUIContent("Use Background", "Setting background image behine your screen elements");

        private Color orgBgColor;

        public override void OnInspectorGUI()
        {
            var uiManBase = (UIManBase)this.target;
            this.orgBgColor = GUI.backgroundColor;

            GUI.backgroundColor = CommonColor.LightOrange;
            GUILayout.BeginHorizontal("Box");
            LabelHelper.HeaderLabel(string.Format("UIMan View Model ({0})", uiManBase.GetUIBaseType()));
            GUILayout.EndHorizontal();

            GUI.backgroundColor = this.orgBgColor;

            LineHelper.Draw(Color.gray);

            EditorGUILayout.Space();
            LabelHelper.HeaderLabel("General");
            GUILayout.BeginVertical("Box");

            EditorGUI.BeginChangeCheck();

            if (uiManBase is UIManDialog dialog)
            {
                dialog.useCover = EditorGUILayout.Toggle(this.cover, dialog.useCover);
            }
            else if (uiManBase is UIManScreen screen)
            {
                screen.useBackground = EditorGUILayout.Toggle(this.background, screen.useBackground);
                if (screen.useBackground)
                    screen.background = EditorGUILayout.TextField(screen.background);
            }

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(this.target);

            if (uiManBase.motionShow == UIMotion.CustomMecanimAnimation || uiManBase.motionHide == UIMotion.CustomMecanimAnimation)
            {
                if (uiManBase.gameObject != null)
                {
                    uiManBase.animRoot = uiManBase.gameObject.GetComponent<Animator>();
                }

                uiManBase.animRoot = EditorGUILayout.ObjectField(this.animator, uiManBase.animRoot, typeof(Animator), true) as Animator;

                if (uiManBase.animRoot == null || uiManBase.animRoot.runtimeAnimatorController == null)
                {
                    if (GUILayout.Button("Generate Animator"))
                    {
                        AnimationEditorUtils.GenerateAnimator(uiManBase.gameObject, UIManDefine.ANIM_SHOW, UIManDefine.ANIM_HIDE, UIManDefine.ANIM_IDLE);
                    }
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(this.show, GUILayout.MaxWidth(50f));
            uiManBase.motionShow = (UIMotion)EditorGUILayout.EnumPopup(uiManBase.motionShow);
            GUILayout.Space(25f);
            EditorGUILayout.LabelField(this.showTime, GUILayout.MaxWidth(50f));
            uiManBase.animShowTime = EditorGUILayout.FloatField(uiManBase.animShowTime);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(this.hide, GUILayout.MaxWidth(50f));
            uiManBase.motionHide = (UIMotion)EditorGUILayout.EnumPopup(uiManBase.motionHide);
            GUILayout.Space(25f);
            EditorGUILayout.LabelField(this.hideTime, GUILayout.MaxWidth(50f));
            uiManBase.animHideTime = EditorGUILayout.FloatField(uiManBase.animHideTime);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(this.idle, GUILayout.MaxWidth(50f));
            uiManBase.motionIdle = (UIMotion)EditorGUILayout.EnumPopup(uiManBase.motionIdle);
            EditorGUILayout.EndHorizontal();

            var motions = new UIMotion[3] { uiManBase.motionShow, uiManBase.motionHide, uiManBase.motionIdle };
            var haveMecanimAnim = false;
            var haveTweenAnim = false;

            foreach (UIMotion m in motions)
            {
                if ((int)m == 7)
                    haveMecanimAnim = true;
                else
                    haveTweenAnim = true;
            }

            EditorGUILayout.Space();

            if (haveTweenAnim && haveMecanimAnim)
            {
                GUILayout.BeginHorizontal("Box");
                EditorGUILayout.LabelField("<color=red><b>Warning: </b>Your motion type is not match with each others so it maybe cause unexpected error!\nPlease select all motion type as Mecanim if you want to make you animation manually with Unity animation editor!</color>", EditorGUIHelper.RichText(true));
                GUILayout.EndHorizontal();
            }

            if (uiManBase.motionIdle != UIMotion.CustomMecanimAnimation && uiManBase.motionIdle != UIMotion.None)
            {
                GUILayout.BeginHorizontal("Box");
                EditorGUILayout.LabelField("<color=red><b>Warning: </b>Idle motion is now only support Mecanim animation!</color>", EditorGUIHelper.RichText(true));
                GUILayout.EndHorizontal();
            }

            uiManBase.showPosition = EditorGUILayout.Vector3Field(this.position, uiManBase.showPosition);

            GUILayout.Space(2f);
            GUILayout.EndVertical();
            LineHelper.Draw(Color.gray);

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
            if (ColorButton.Draw("Edit View (UI)", CommonColor.LightGreen, GUILayout.Height(25)))
            {
                GameObject prefabInstance;
                Object obj = FindObjectOfType(uiManBase.UIType);
                if (obj != null)
                {
                    prefabInstance = ((MonoBehaviour)obj).gameObject;
                }
                else
                {
                    var isDialog = uiManBase.GetUIBaseType() == UIBaseType.Dialog;
                    prefabInstance = PrefabUtility.InstantiatePrefab(uiManBase.gameObject) as GameObject;
                    if (isDialog)
                        prefabInstance.transform.SetParent(UIMan.Instance.dialogRoot, false);
                    else
                        prefabInstance.transform.SetParent(UIMan.Instance.screenRoot, false);
                }
                Selection.activeGameObject = prefabInstance;
            }
            if (ColorButton.Draw("Edit View Logic (Handler)", CommonColor.LightGreen, GUILayout.Height(25)))
            {
                var handler = UIManCodeGenerator.GetScriptPathByType(this.target.GetType());
                handler = handler.Replace(".cs", ".Handler.cs");
                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(handler, 1);
            }
        }
    }
}