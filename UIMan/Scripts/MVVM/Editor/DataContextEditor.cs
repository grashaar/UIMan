using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnuGames.MVVM
{
    [CustomEditor(typeof(DataContext))]
    public class DataContextEditor : Editor
    {
        private readonly GUIContent lblType = new GUIContent("Type");
        private readonly GUIContent lblContext = new GUIContent("Context");

        private int selected = 0;

        public override void OnInspectorGUI()
        {
            var context = (DataContext)this.target;

            context.type = (ContextType)EditorGUILayout.EnumPopup(this.lblType, context.type);

            if (context.type == ContextType.None)
            {
                context.Clear();
                GUILayout.Label(BindingDefine.NO_CONTEXT_TYPE);
            }
            else if (context.type == ContextType.MonoBehaviour)
            {
                context.viewModel = (ViewModelBehaviour)EditorGUILayout.ObjectField(this.lblContext, (Object)context.viewModel, typeof(ViewModelBehaviour), true);
                if (context.viewModel.GetCachedType() != null)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(" ");
                    EditorGUILayout.LabelField("<color=blue>[" + context.viewModel.GetCachedType().FullName + "]</color>", EditorGUIHelper.RichText());
                    GUILayout.EndHorizontal();
                }
            }
            else if (context.type == ContextType.Property)
            {
                context.viewModel = (ViewModelBehaviour)EditorGUILayout.ObjectField(this.lblContext, (Object)context.viewModel, typeof(ViewModelBehaviour), true);

                var members = context.viewModel.GetAllMembers(false, true, true, MemberTypes.Field, MemberTypes.Property, MemberTypes.Field);
                if (members != null)
                {
                    if (string.IsNullOrEmpty(context.propertyName))
                    {
                        context.propertyName = members[0];
                    }
                    else
                    {
                        for (var i = 0; i < members.Length; i++)
                        {
                            if (members[i] == context.propertyName)
                            {
                                this.selected = i;
                                break;
                            }
                        }
                    }

                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();

                    GUILayout.BeginVertical();
                    GUILayout.Space(5);
                    var newSelected = EditorGUILayout.Popup("Field/Property", this.selected, members);
                    GUILayout.EndVertical();

                    if (this.selected != newSelected)
                    {
                        context.propertyName = members[newSelected];
                        this.selected = newSelected;
                    }

                    if (EditorGUIHelper.QuickPickerButton())
                    {
                        ContextBrowser.Browse(members, selectedMember => {
                            context.propertyName = selectedMember;
                            FilterPopup.Close();
                        });
                    }

                    GUILayout.EndHorizontal();

                    MemberInfo curMember = context.viewModel.GetMemberInfo(members[this.selected], MemberTypes.Property, MemberTypes.Field);
                    if (curMember != null)
                    {
                        var attributes = curMember.GetCustomAttributes(typeof(UIManProperty), false);
                        if (attributes == null || attributes.Length == 0)
                        {
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.PrefixLabel(" ");
                            GUILayout.Label("<color=red>None observable field/property!</color>", EditorGUIHelper.RichText());
                            GUILayout.EndHorizontal();
                        }
                    }
                    GUILayout.EndVertical();
                }

                if (Event.current.type == EventType.Repaint)
                {
                    FilterPopup.SetPopupRect(GUILayoutUtility.GetLastRect());
                }
            }
        }
    }
}