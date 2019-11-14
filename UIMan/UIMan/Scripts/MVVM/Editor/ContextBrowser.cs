using System;
using System.Reflection;

namespace UnuGames.MVVM
{
    public class ContextBrowser
    {
        private static BinderBaseEditor curBinderEditor;
        private static BindingField curField;
        private static string[] members;

        /// <summary>
        /// Browse for field/property
        /// </summary>
        /// <param name="binderEditor"></param>
        /// <param name="field"></param>
        static public void Browse(BinderBaseEditor binderEditor, BindingField field)
        {
            curBinderEditor = binderEditor;
            curField = field;

            members = binderEditor.binder.GetMembers(MemberTypes.Field, MemberTypes.Property);

            FilterPopup.Browse(members, OnMemberSelected);
        }

        static public void Browse(string[] members, Action<string> onSelected)
        {
            FilterPopup.Browse(members, onSelected);
        }

        static public void OnMemberSelected(string member)
        {
            curField.member = member;
            curBinderEditor.Apply();
            FilterPopup.Close();
        }
    }
}