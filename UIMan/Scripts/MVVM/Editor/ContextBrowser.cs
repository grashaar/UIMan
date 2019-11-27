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
        public static void Browse(BinderBaseEditor binderEditor, BindingField field, bool boldName = false, bool withType = false, bool asPath = false)
        {
            curBinderEditor = binderEditor;
            curField = field;

            members = binderEditor.binder.GetMembers(boldName, withType, asPath, MemberTypes.Field, MemberTypes.Property);

            FilterPopup.Browse(members, OnMemberSelected);
        }

        public static void Browse(string[] members, Action<string> onSelected)
        {
            FilterPopup.Browse(members, onSelected);
        }

        public static void OnMemberSelected(string member)
        {
            curField.member = member;
            curBinderEditor.Apply();
            FilterPopup.Close();
        }
    }
}