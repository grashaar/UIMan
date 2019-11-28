using System;
using System.Reflection;

namespace UnuGames.MVVM
{
    public class ContextBrowser
    {
        private static BinderBaseEditor curBinderEditor;
        private static BindingField curField;
        private static string[] viewMembers;
        private static string[] dataMembers;

        /// <summary>
        /// Browse for field/property
        /// </summary>
        /// <param name="binderEditor"></param>
        /// <param name="field"></param>
        public static void Browse(BinderBaseEditor binderEditor, BindingField field, int selectedIndex, bool boldName = false, bool withReturnType = false, bool withDeclaringType = false, bool asPath = false)
        {
            curBinderEditor = binderEditor;
            curField = field;

            dataMembers = binderEditor.binder.GetMembers(false, false, false, false, MemberTypes.Field, MemberTypes.Property);
            viewMembers = binderEditor.binder.GetMembers(boldName, withReturnType, withDeclaringType, asPath, MemberTypes.Field, MemberTypes.Property);

            FilterPopup.Browse(selectedIndex, dataMembers, viewMembers, OnMemberSelected);
        }

        public static void Browse(int selectedIndex, string[] dataMembers, string[] viewMembers, Action<string> onSelected)
        {
            FilterPopup.Browse(selectedIndex, dataMembers, viewMembers, onSelected);
        }

        public static void OnMemberSelected(string member)
        {
            UnityEditor.Undo.RecordObject(curBinderEditor.target, "Select Binder Member");
            curField.member = member;
            curBinderEditor.Apply();
            FilterPopup.Close();
        }
    }
}