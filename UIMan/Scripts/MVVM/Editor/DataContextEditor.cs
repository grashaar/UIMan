using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnuGames.MVVM
{
    [CustomEditor(typeof(DataContext))]
    public class DataContextEditor : Editor
    {
        private static readonly System.Type _type = typeof(ViewModelBehaviour);

        private readonly GUIContent lblType = new GUIContent("Type");
        private readonly GUIContent lblContext = new GUIContent("Context");

        private int selected = 0;

        public override void OnInspectorGUI()
        {
            var context = (DataContext)this.target;

            EditorGUI.BeginChangeCheck();
            var contextType = (ContextType)EditorGUILayout.EnumPopup(this.lblType, context.type);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(this.target, "Select Context Type");
                context.type = contextType;
            }

            if (context.type == ContextType.None)
            {
                context.Clear();
                GUILayout.Label(BindingDefine.NO_CONTEXT_TYPE);
            }
            else if (context.type == ContextType.MonoBehaviour)
            {
                EditorGUI.BeginChangeCheck();
                var viewModel = EditorGUILayout.ObjectField(this.lblContext, context.viewModel, _type, true);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(this.target, "Select ViewModel");
                    context.viewModel = viewModel as ViewModelBehaviour;
                }

                if (context.viewModel.GetCachedType() != null)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(" ");
                    EditorGUILayout.LabelField("[" + context.viewModel.GetCachedType().FullName + "]",
                                               EditorGUIHelper.RichText(color: CommonColor.GetBlue()));
                    GUILayout.EndHorizontal();
                }
            }
            else if (context.type == ContextType.Property)
            {
                EditorGUI.BeginChangeCheck();
                var viewModel = EditorGUILayout.ObjectField(this.lblContext, context.viewModel, _type, true);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(this.target, "Select ViewModel");
                    context.viewModel = viewModel as ViewModelBehaviour;
                }

                var viewMembers = context.viewModel.GetAllMembers(false, true, false, false, MemberTypes.Field, MemberTypes.Property);
                var dataMembers = context.viewModel.GetAllMembers(false, false, false, false, MemberTypes.Field, MemberTypes.Property);

                if (dataMembers != null)
                {
                    if (string.IsNullOrEmpty(context.propertyName))
                    {
                        context.propertyName = dataMembers[0];
                        this.selected = 0;
                    }
                    else
                    {
                        for (var i = 0; i < dataMembers.Length; i++)
                        {
                            if (dataMembers[i] == context.propertyName)
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
                    var newSelected = EditorGUILayout.Popup("Field/Property", this.selected, viewMembers);
                    GUILayout.EndVertical();

                    if (this.selected != newSelected)
                    {
                        Undo.RecordObject(this.target, "Select Contenxt Member");
                        context.propertyName = dataMembers[newSelected];
                        this.selected = newSelected;
                    }

                    if (EditorGUIHelper.QuickPickerButton())
                    {
                        ContextBrowser.Browse(this.selected, dataMembers, viewMembers, selectedMember => {
                            Undo.RecordObject(this.target, "Select Context Member");
                            context.propertyName = selectedMember;
                            FilterPopup.Close();
                        });
                    }

                    GUILayout.EndHorizontal();

                    var dataMember = dataMembers[this.selected];
                    var curMember = context.viewModel.GetMemberInfo(dataMember, MemberTypes.Property, MemberTypes.Field);

                    if (curMember != null)
                    {
                        var attributes = curMember.GetCustomAttributes(typeof(UIManPropertyAttribute), false);

                        if (attributes == null || attributes.Length == 0)
                        {
                            GUILayout.BeginHorizontal("Box");
                            EditorGUILayout.PrefixLabel(" ");
                            GUILayout.Label("Field/Property must be decorated with either [UIManProperty] or [UIManAutoProperty] attribute!", EditorGUIHelper.RichText(true, CommonColor.GetRed()));
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