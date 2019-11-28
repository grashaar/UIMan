using System;
using UnityEditor;
using UnityEngine;

namespace UnuGames
{
    public class FilterPopup : EditorWindow
    {
        private static string[] mViewItems;
        private static string[] mDataItems;

        private static Action<string> OnSelected { get; set; }

        private const int MEMBER_HEIGHT = 37;
        private static Rect inspectorRect;
        private static Vector2 inspectorPos;
        private static UISearchField searchField;
        private static ListView listView;
        private static int selected;

        /// <summary>
        /// Show window as dropdown popup
        /// </summary>
        private static void Popup()
        {
            var fp = CreateInstance<FilterPopup>();

            var minHeight = mViewItems.Length * MEMBER_HEIGHT + MEMBER_HEIGHT * 2;
            var bestHeight = (int)(Screen.currentResolution.height / 2.5f);

            if (minHeight > bestHeight)
                minHeight = bestHeight;

            inspectorPos = GUIUtility.GUIToScreenPoint(new Vector2(inspectorRect.x, inspectorRect.y));
            fp.ShowAsDropDown(new Rect(inspectorPos, inspectorRect.size), new Vector2(inspectorRect.width, minHeight));
        }

        /// <summary>
        /// Browse for field/property
        /// </summary>
        /// <param name="binderEditor"></param>
        /// <param name="field"></param>
        public static void Browse(int selectedIndex, string[] items, Action<string> onSelected)
        {
            selected = selectedIndex;
            searchField = new UISearchField(Filter, null, null);
            OnSelected = onSelected;
            mDataItems = items;
            mViewItems = items;

            if (items != null && items.Length > 0)
                Popup();
        }

        /// <summary>
        /// Browse for field/property
        /// </summary>
        /// <param name="binderEditor"></param>
        /// <param name="field"></param>
        public static void Browse(int selectedIndex, string[] dataItems, string[] viewItems, Action<string> onSelected)
        {
            selected = selectedIndex;
            searchField = new UISearchField(Filter, null, null);
            OnSelected = onSelected;
            mDataItems = dataItems;
            mViewItems = viewItems;

            if (dataItems != null && dataItems.Length > 0)
                Popup();
        }

        private void OnGUI()
        {
            if (Event.current.keyCode == KeyCode.Escape)
                Close();

            if (mViewItems == null)
                return;

            if (listView == null)
                listView = new ListView();

            //Search field
            searchField.Draw();
            listView.SetData(selected, mDataItems, mViewItems, true, OnSelected, searchField.KeyWord, this);
            listView.Draw();
        }

        /// <summary>
        /// Set the window's rectangle
        /// </summary>
        /// <param name="rect"></param>
        public static void SetPopupRect(Rect rect)
        {
            inspectorRect = rect;
        }

        public static void SetShowPosition()
        {
            SetPopupRect(new Rect(GUILayoutUtility.GetLastRect().x, Event.current.mousePosition.y, GUILayoutUtility.GetLastRect().width, 10));
        }

        /// <summary>
        /// Filter items by keyword
        /// </summary>
        /// <param name="keyWord"></param>
        private static void Filter(string keyWord)
        {
        }

        new public static void Close()
        {
            if (listView != null && listView.Window != null)
                listView.Window.Close();
        }
    }
}