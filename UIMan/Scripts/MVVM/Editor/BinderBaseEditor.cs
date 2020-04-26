using System.Reflection;
using UnityEditor;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
#endif

namespace UnuGames.MVVM
{
    [CustomEditor(typeof(BinderBase), true)]
    public class BinderBaseEditor :
#if ODIN_INSPECTOR
        OdinEditor
#else
        Editor
#endif
    {
        public BinderBase binder;

        public override void OnInspectorGUI()
        {
#if ODIN_INSPECTOR
            base.OnInspectorGUI();
#else
            DrawDefaultInspector();
#endif

            this.binder = this.target as BinderBase;

            GUILayout.Space(4);
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Space(4);

            var context = EditorGUILayout.ObjectField(new GUIContent("Data Context"), this.binder.dataContext, typeof(DataContext), true) as DataContext;

            if (context == null)
            {
                GUILayout.Space(4);

                if (GUILayout.Button(BindingDefine.FIND_CONTEXT))
                {
                    context = this.binder.FindDataContext();
                }
            }

            if (this.binder.dataContext != context)
            {
                Undo.RecordObject(this.target, "Select Data Context");
                this.binder.dataContext = context;
            }

            if (this.binder.dataContext == null)
            {
                GUILayout.Space(4);
                GUILayout.EndVertical();
                return;
            }

            BindingField[] arrFields = this.binder.GetBindingFields();

            GUILayout.BeginVertical();

            for (var i = 0; i < arrFields.Length; i++)
            {
                DrawBindingField(arrFields[i]);
            }

            GUILayout.EndVertical();
            GUILayout.Space(4);
            GUILayout.EndVertical();

            if (Event.current.type == EventType.Repaint)
            {
                FilterPopup.SetPopupRect(GUILayoutUtility.GetLastRect());
            }
        }

        public void DrawBindingField(BindingField field)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel(new GUIContent(field.label));
            var curMemberName = field.member;
            if (string.IsNullOrEmpty(curMemberName))
            {
                curMemberName = BindingDefine.SELECT_MEMBER;
            }

            var viewMembers = this.binder.GetMembers(false, true, false, false, MemberTypes.Field, MemberTypes.Property);
            var dataMembers = this.binder.GetMembers(false, false, false, false, MemberTypes.Field, MemberTypes.Property);

            if (dataMembers == null)
            {
                EditorGUILayout.LabelField("<color=red>No target context found!</color>", EditorGUIHelper.RichText());
                GUILayout.EndHorizontal();
            }
            else
            {
                ArrayUtility.Insert(ref dataMembers, 0, "<None>");
                ArrayUtility.Insert(ref viewMembers, 0, "<None>");

                var selectedIndex = 0;

                for (var i = 0; i < dataMembers.Length; i++)
                {
                    if (curMemberName == dataMembers[i])
                    {
                        selectedIndex = i;
                        break;
                    }
                }

                GUILayout.Space(-7);
                EditorGUILayout.BeginVertical();
                GUILayout.Space(5);
                var newSelectedIndex = EditorGUILayout.Popup(selectedIndex, viewMembers);
                if (newSelectedIndex != selectedIndex)
                {
                    Undo.RecordObject(this.target, "Select Binder Member");
                    selectedIndex = newSelectedIndex;
                    field.member = dataMembers[selectedIndex];
                    Apply();
                }

                EditorGUILayout.EndVertical();

                if (EditorGUIHelper.QuickPickerButton())
                {
                    ContextBrowser.Browse(this, field, selectedIndex, true, true, false);
                }

                GUILayout.EndHorizontal();

                MemberInfo curMember = this.binder.GetMemberInfo(dataMembers[selectedIndex], MemberTypes.Property, MemberTypes.Field);
                if (curMember != null)
                {
                    var attributes = curMember.GetCustomAttributes(typeof(UIManPropertyAttribute), false);
                    if (attributes == null || attributes.Length == 0)
                    {
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(" ");
                        GUILayout.Label("<color=red>None observable field!</color>", EditorGUIHelper.RichText());
                        GUILayout.EndHorizontal();
                    }
                }
            }

            GUILayout.EndVertical();
        }

        public void Apply()
        {
            EditorUtility.SetDirty(this.target);
        }
    }
}