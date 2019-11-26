using System;
using UnityEditor;
using UnityEngine;

namespace UnuGames
{
    public class UISearchField
    {
        private string keyWord = "";

        public string KeyWord
        {
            get { return this.keyWord; }
            set { this.keyWord = value; }
        }

        private string oldKeyWord = "";
        private GUIStyle searchFieldStyle;
        private GUIStyle searchCancelButtonStyle;
        private GUIStyle toolbarStyle;
        private GUIStyle toolbarButtonStyle;
        private GUIStyle toolbarDropDownStyle;
        private Action<string> _onKeyWordChange;
        private Action<object> _onLeftButtonClick;
        private string _leftButtonText = "Create";

        public UISearchField(Action<string> onKeyWordChange, Action<object> onLeftButtonClick, string leftButtonText = "Create")
        {
            this._onKeyWordChange = onKeyWordChange;
            this._onLeftButtonClick = onLeftButtonClick;

            this.searchFieldStyle = GUI.skin.FindStyle("ToolbarSeachTextField");
            this.searchCancelButtonStyle = GUI.skin.FindStyle("ToolbarSeachCancelButton");
            this.toolbarStyle = GUI.skin.FindStyle("Toolbar");
            this.toolbarButtonStyle = GUI.skin.FindStyle("toolbarbutton");
            this.toolbarDropDownStyle = GUI.skin.FindStyle("ToolbarDropDown");
            this._leftButtonText = leftButtonText;
        }

        public void Draw()
        {
            GUILayout.BeginHorizontal(this.toolbarStyle);

            if (this._onLeftButtonClick != null && !string.IsNullOrEmpty(this._leftButtonText))
            {
                if (GUILayout.Button(this._leftButtonText, this.toolbarButtonStyle, GUILayout.Width(50)))
                {
                    if (this._onLeftButtonClick != null)
                        this._onLeftButtonClick(null);
                }
                GUILayout.Label("", this.toolbarDropDownStyle, GUILayout.Width(6));
                GUILayout.Space(5);
            }

            this.oldKeyWord = this.KeyWord;

            this.KeyWord = GUILayout.TextField(this.KeyWord, this.searchFieldStyle);

            if (GUILayout.Button("", this.searchCancelButtonStyle))
            {
                this.KeyWord = "";
                GUI.FocusControl(null);
            }

            if (!this.oldKeyWord.Equals(this.KeyWord))
            {
                if (this._onKeyWordChange != null)
                {
                    this._onKeyWordChange(this.KeyWord);
                }
            }

            GUILayout.EndHorizontal();
        }
    }

    public class ListView
    {
        private GUIStyle menuItemStyle;
        private string[] _items;
        private bool _selectOnMouseHover;
        private string _filter;
        private Vector2 _scrollPosition;
        private EditorWindow _window;

        public EditorWindow Window
        {
            get { return this._window; }
        }

        private Action<string> _onSelected;
        private Action<int> _onSelectedIndex;

        private int selectedIndex = -1;
        public string SelectedItem { get; set; }

        public ListView()
        {
            this.menuItemStyle = new GUIStyle(GUI.skin.FindStyle("MenuItem"));
        }

        public void SetData(string[] items, bool selectOnMouseHover, Action<string> onSelected, string filterString, EditorWindow window, Action<int> onSelectedIndex = null)
        {
            this._selectOnMouseHover = selectOnMouseHover;
            this._filter = filterString;
            this._items = items;
            this._window = window;
            this._onSelected = onSelected;
            this._onSelectedIndex = onSelectedIndex;
        }

        public void Draw()
        {
            //List of items
            this._scrollPosition = GUILayout.BeginScrollView(this._scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            for (var i = 0; i < this._items.Length; i++)
            {
                // Filter item by keyword
                if (!string.IsNullOrEmpty(this._filter))
                {
                    if (this._items[i].ToLower().IndexOf(this._filter.ToLower(), StringComparison.Ordinal) < 0)
                        continue;
                }

                // Draw suitable items
                if (this._selectOnMouseHover)
                {
                    if (GUILayout.Button(this._items[i], this.menuItemStyle))
                    {
                        DoSelect(i);
                    }
                }
                else
                {
                    var val = i == this.selectedIndex ? true : false;
                    var newVal = GUILayout.Toggle(val, this._items[i], "Button");
                    if (val != newVal && newVal == true)
                    {
                        DoSelect(i);
                    }
                }

                // Update button's status (for hover event)
                if (this._selectOnMouseHover)
                    this._window.Repaint();
            }

            GUILayout.EndScrollView();
        }

        private void DoSelect(int i)
        {
            if (i > -1)
            {
                this.selectedIndex = i;
                if (this._onSelected != null)
                {
                    this.SelectedItem = this._items[i];
                    this._onSelected(this._items[i]);
                }

                if (this._onSelectedIndex != null)
                {
                    this.SelectedItem = this._items[i];
                    this._onSelectedIndex(i);
                }
            }
        }

        public void Select(string item)
        {
            var itemIndex = ArrayUtility.IndexOf(this._items, item);
            DoSelect(itemIndex);
        }
    }

    public class EditablePropertyDrawer
    {
        private Type _viewModelType;
        private CustomPropertyInfo _property;
        private int selectedType = 0;
        private string[] observableTypes;
        private Action<CustomPropertyInfo> _onPropertyChanged;
        private Action<CustomPropertyInfo> _onPropertyDelete;

        public EditablePropertyDrawer(Type viewModelType, CustomPropertyInfo property, Action<CustomPropertyInfo> onPropertyChanged, Action<CustomPropertyInfo> onPropertyDelete)
        {
            this._viewModelType = viewModelType;
            this._property = property;
            this._onPropertyChanged = onPropertyChanged;
            this._onPropertyDelete = onPropertyDelete;

            this.observableTypes = ReflectionUtils.GetAllObservableTypes(this._viewModelType);
            for (var i = 0; i < this.observableTypes.Length; i++)
            {
                if (this._property.LastPropertyType.GetAllias() == this.observableTypes[i])
                {
                    this.selectedType = i;
                    break;
                }
            }
        }

        public void Draw(float totalWidth)
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Space(3);
            this._property.IsSelected = EditorGUILayout.Toggle(this._property.IsSelected);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Space(4);

            GUILayoutOption nameWidth = GUILayout.Width(totalWidth * 1 / 3 - 10);
            GUILayoutOption typeWidth = GUILayout.Width(totalWidth / 6 - 10);
            GUILayoutOption defaultValWidth = GUILayout.Width(totalWidth * 1 / 3 - 10);
            GUILayoutOption buttonWidth = GUILayout.Width(totalWidth / 16 - 5);

            // Property name
            this._property.LastName = EditorGUILayout.TextField(this._property.LastName, nameWidth);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Space(4.5f);

            // Property type
            this.selectedType = EditorGUILayout.Popup(this.selectedType, this.observableTypes, typeWidth);
            this._property.LastPropertyType = ReflectionUtils.GetTypeByName(this.observableTypes[this.selectedType]);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Space(4);

            // Default value
            if (this._property.LastPropertyType == typeof(int))
            {
                this._property.SetLastValueAs<int>(EditorGUILayout.IntField(this._property.GetLastValueAs<int>(), defaultValWidth));
            }
            else if (this._property.LastPropertyType == typeof(long))
            {
                this._property.SetLastValueAs<long>(EditorGUILayout.LongField(this._property.GetLastValueAs<long>(), defaultValWidth));
            }
            else if (this._property.LastPropertyType == typeof(float))
            {
                this._property.SetLastValueAs<float>(EditorGUILayout.FloatField(this._property.GetLastValueAs<float>(), defaultValWidth));
            }
            else if (this._property.LastPropertyType == typeof(double))
            {
                this._property.SetLastValueAs<double>(EditorGUILayout.DoubleField(this._property.GetLastValueAs<double>(), defaultValWidth));
            }
            else if (this._property.LastPropertyType == typeof(string))
            {
                if (this._property.DefaltValue == null)
                {
                    this._property.DefaltValue = string.Empty;
                    this._property.LastValue = string.Empty;
                }
                this._property.SetLastValueAs<string>(EditorGUILayout.TextField(this._property.GetLastValueAs<string>(), defaultValWidth));
            }
            else if (this._property.LastPropertyType == typeof(bool))
            {
                this._property.SetLastValueAs<bool>(EditorGUILayout.Toggle(this._property.GetLastValueAs<bool>(), defaultValWidth));
            }
            else
            {
                GUILayout.Label("Undefined!", defaultValWidth);
            }
            GUILayout.EndVertical();

            Color textColor = Color.gray;
            if (this._property.HasChange)
                textColor = Color.black;

            if (ColorButton.Draw("S", CommonColor.LightGreen, textColor, buttonWidth))
            {
                if (this._property.HasChange)
                {
                    this._property.CommitChange();
                    if (this._onPropertyChanged != null)
                        this._onPropertyChanged(this._property);
                }
            }

            if (ColorButton.Draw("X", CommonColor.LightRed, buttonWidth))
            {
                if (this._onPropertyDelete != null)
                    this._onPropertyDelete(this._property);
            }

            GUILayout.EndHorizontal();
        }
    }

    public class EditablePopup
    {
        private string[] _arrItems;
        private string _currentItem;
        private int selectedIndex = 0;
        private Action<string> _onSave;

        public string SelectedItem
        {
            get
            {
                return this._arrItems[this.selectedIndex];
            }
        }

        public EditablePopup(string[] items, string currentItem, Action<string> onSave)
        {
            this._currentItem = currentItem;
            this._arrItems = items;
            this._onSave = onSave;
            for (var i = 0; i < this._arrItems.Length; i++)
            {
                if (this._arrItems[i] == currentItem)
                {
                    this.selectedIndex = i;
                    break;
                }
            }
        }

        public void Draw()
        {
            this.selectedIndex = EditorGUILayout.Popup(this.selectedIndex, this._arrItems);

            if (this._arrItems[this.selectedIndex] != this._currentItem && this._onSave != null)
            {
                if (GUILayout.Button("Save"))
                {
                    if (this._onSave != null)
                        this._onSave(this._arrItems[this.selectedIndex]);
                }
            }
        }
    }

    public class LineHelper
    {
        public static void Draw(Color color, float width, float height = 1)
        {
            Color backupColor = GUI.color;
            GUI.color = color;
            GUILayout.Box(Texture2D.whiteTexture, GUILayout.Width(width - 20), GUILayout.Height(height));
            GUI.color = backupColor;
        }

        public static void Draw(Color color)
        {
            Color backupColor = GUI.color;
            GUI.color = color;
            GUILayout.Box(Texture2D.whiteTexture, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
            GUI.color = backupColor;
        }
    }

    public class LabelHelper
    {
        private static GUIStyle headerLabel;
        private static GUIStyle titleLabel;

        public static void HeaderLabel(string text, GUILayoutOption width = null)
        {
            if (headerLabel == null)
            {
                headerLabel = new GUIStyle();
                headerLabel.normal.textColor = Color.black;
                headerLabel.fontStyle = FontStyle.Bold;
                headerLabel.alignment = TextAnchor.MiddleLeft;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            if (width != null)
                GUILayout.Label(text, headerLabel, width);
            else
                GUILayout.Label(text, headerLabel);
            GUILayout.EndHorizontal();
        }

        public static void TitleLabel(string text)
        {
            if (titleLabel == null)
            {
                titleLabel = new GUIStyle();
                titleLabel.normal.textColor = Color.black;
                titleLabel.fontStyle = FontStyle.Bold;
                titleLabel.fontSize = titleLabel.fontSize + 15;
                titleLabel.alignment = TextAnchor.MiddleCenter;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            GUILayout.Label(text, titleLabel);
            GUILayout.EndHorizontal();
        }

        public static void ColumnLabel(string text, GUILayoutOption width)
        {
            EditorGUILayout.LabelField("<b>" + text + "</b>", EditorGUIHelper.RichText(), width);
        }
    }

    public class TextFieldHelper
    {
        public string Text { get; set; }

        public string Draw(GUIContent label)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(100));
            this.Text = EditorGUILayout.TextField(this.Text);

            GUILayout.EndHorizontal();

            return this.Text;
        }

        public TextFieldHelper(string text)
        {
            this.Text = text;
        }
    }

    public class PathBrowser
    {
        public string SelectedPath { get; set; }
        public string StripPattern { get; set; }

        public string Draw(GUIContent label)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(100));
            this.SelectedPath = EditorGUILayout.TextField(this.SelectedPath);
            if (GUILayout.Button("Browse", GUILayout.Height(15)))
            {
                var newPath = EditorUtility.OpenFolderPanel("Select folder", this.SelectedPath, "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    if (!newPath.Contains(Application.dataPath))
                    {
                        EditorUtility.DisplayDialog("Error", "Cannot save file outside of project's asset folder!", "OK");
                    }
                    else
                    {
                        newPath = NormalizePath(newPath);
                        this.SelectedPath = newPath;
                    }
                }
            }

            GUILayout.EndHorizontal();

            return this.SelectedPath;
        }

        public PathBrowser(string defaultPath, string stripPattern)
        {
            this.SelectedPath = defaultPath;
            this.StripPattern = stripPattern;
        }

        public string NormalizePath(string path)
        {
            if (!string.IsNullOrEmpty(this.StripPattern))
            {
                var normalized = path.Replace(this.StripPattern, "");

                if (normalized != null && normalized.Length >= 1 && normalized.Substring(0, 1).Equals("/"))
                    normalized = normalized.Substring(1, normalized.Length - 1);

                return normalized + "/";
            }

            return "";
        }
    }

    public class EditorGUIHelper
    {
        private static GUISkin skin;

        public static GUIStyle RichText(bool wordWrap = false)
        {
            var style = new GUIStyle();
            style.richText = true;
            style.wordWrap = wordWrap;
            return style;
        }

        public static bool QuickPickerButton()
        {
            return GUILayout.Button("Browse...");
        }

        public static Vector3 DrawVector3(string label, float x, float y, float z)
        {
            return EditorGUILayout.Vector3Field(label, new Vector3(x, y, z));
        }

        public static Vector3 DrawArc(string label, float angle, float radius, float height)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.PrefixLabel(label);
            GUILayout.Space(-4);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("A", GUILayout.Width(12));
            angle = EditorGUILayout.FloatField(angle);
            EditorGUILayout.LabelField("R", GUILayout.Width(12));
            radius = EditorGUILayout.FloatField(radius);
            EditorGUILayout.LabelField("H", GUILayout.Width(12));
            height = EditorGUILayout.FloatField(height);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            return new Vector3(angle, radius, height);
        }

        public static float DrawFloat(string label, float value)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.PrefixLabel(label);
            GUILayout.Space(-4);
            value = EditorGUILayout.FloatField(value);
            EditorGUILayout.EndVertical();

            return value;
        }
    }

    public class ColorButton
    {
        private static Color orgBgColor;
        private static Color orgTextColor;

        public static bool Draw(string text, Color color, params GUILayoutOption[] options)
        {
            orgBgColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            var press = GUILayout.Button(text, options);
            GUI.backgroundColor = orgBgColor;
            return press;
        }

        public static bool Draw(string text, Color color, Color textColor, params GUILayoutOption[] options)
        {
            orgBgColor = GUI.backgroundColor;
            orgTextColor = GUI.contentColor;
            GUI.backgroundColor = color;
            GUI.contentColor = textColor;
            var press = GUILayout.Button(text, options);
            GUI.backgroundColor = orgBgColor;
            GUI.contentColor = orgTextColor;
            return press;
        }

        public static bool Draw(string text, Color color)
        {
            orgBgColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            var press = GUILayout.Button(text);
            GUI.backgroundColor = orgBgColor;
            return press;
        }
    }

    public class NamingBox
    {
        private string name;

        public void Draw(Action<string> onCreate, Action onCancel)
        {
            this.name = EditorGUILayout.TextField(this.name);
            if (GUILayout.Button("Create"))
            {
                if (onCreate != null)
                    onCreate(this.name);
            }
            if (GUILayout.Button("Cancel"))
            {
                if (onCancel != null)
                    onCancel();
            }
        }
    }

    public static class CommonColor
    {
        public static Color LightGreen = new Color(0.2f, 1, 0.35f);
        public static Color LightRed = new Color(1, 0.3f, 0.3f);
        public static Color LightBlue = new Color(0, 0.85f, 1);
        public static Color LightOrange = new Color(1, 0.56f, 0.14f);
    }
}