using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnuGames.MVVM
{
    [CustomEditor(typeof(BinderBase), true)]
    public class BinderBaseEditor : Editor
    {
        public BinderBase binder;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            this.binder = this.target as BinderBase;

            var context = EditorGUILayout.ObjectField(new GUIContent("Data Context"), this.binder.dataContext, typeof(DataContext), true) as DataContext;

            if (context == null)
            {
                GUILayout.Label(BindingDefine.FIND_CONTEXT_AUTO);
                if (GUILayout.Button(BindingDefine.FIND_CONTEXT))
                {
                    context = this.binder.FindDataContext();
                }
            }

            if (this.binder.dataContext != context)
            {
                this.binder.dataContext = context;
                EditorUtility.SetDirty(this.target);
            }

            if (this.binder.dataContext == null)
                return;

            BindingField[] arrFields = this.binder.GetBindingFields();

            GUILayout.BeginVertical();

            for (var i = 0; i < arrFields.Length; i++)
            {
                DrawBindingField(arrFields[i]);
            }

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

            var members = this.binder.GetMembers(MemberTypes.Field, MemberTypes.Property);
            if (members == null)
            {
                EditorGUILayout.LabelField("<color=red>No target context found!</color>", EditorGUIHelper.RichText());
                GUILayout.EndHorizontal();
            }
            else
            {
                ArrayUtility.Insert(ref members, 0, "Null");

                var selectedIndex = 0;
                for (var i = 0; i < members.Length; i++)
                {
                    if (curMemberName == members[i])
                    {
                        selectedIndex = i;
                        break;
                    }
                }

                GUILayout.Space(-7);
                EditorGUILayout.BeginVertical();
                GUILayout.Space(5);
                var newSelectedIndex = EditorGUILayout.Popup(selectedIndex, members);
                if (newSelectedIndex != selectedIndex)
                {
                    selectedIndex = newSelectedIndex;
                    field.member = members[selectedIndex];
                    Apply();
                }

                EditorGUILayout.EndVertical();

                if (EditorGUIHelper.QuickPickerButton())
                {
                    ContextBrowser.Browse(this, field);
                }

                GUILayout.EndHorizontal();

                MemberInfo curMember = this.binder.GetMemberInfo(members[selectedIndex], MemberTypes.Property, MemberTypes.Field);
                if (curMember != null)
                {
                    var attributes = curMember.GetCustomAttributes(typeof(UIManProperty), false);
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