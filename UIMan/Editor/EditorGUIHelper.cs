using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UnuGames
{
    public class NamePopupWindow : EditorWindow
    {
        public int selectedIndex { get; private set; }

        public Type[] types { get; private set; }

        private readonly List<SingleType> singleTypes = new List<SingleType>();

        private readonly Dictionary<string, List<SingleType>> typeDict = new Dictionary<string, List<SingleType>>();

        private Action<NamePopupWindow> onSelect;
        private Vector2 scrollPosition;
        private GUIStyle annotationStyle;
        private GUIStyle typeNameStyle;
        private GUIStyle selectedBackgroundStyle;
        private GUIStyle normalBackgroundStyle;
        private bool isInitedStype = false;
        private bool isSelected = false;
        private GUIStyle searchToobar;
        private string searchText = string.Empty;
        private SingleType? selectedType = null;

        public void Initialize(int selectedIndex, Type[] types, Vector2 windowSize, Action<NamePopupWindow> onSelect)
        {
            this.selectedIndex = selectedIndex;
            this.types = types;
            this.onSelect = onSelect;
            this.singleTypes.Clear();
            this.typeDict.Clear();
            this.selectedType = null;

            for (var i = 0; i < types.Length; i++)
            {
                var name = types[i];
                var single = new SingleType {
                    type = name,
                    index = i
                };

                if (i == selectedIndex)
                {
                    single.isSelect = true;
                    this.selectedType = single;
                }
                else
                {
                    single.isSelect = false;
                }

                if (!this.typeDict.ContainsKey(single.type.Namespace))
                {
                    this.typeDict.Add(single.type.Namespace, new List<SingleType>());
                }

                var list = this.typeDict[single.type.Namespace];
                list.Add(single);
            }

            foreach (var list in this.typeDict.Values)
            {
                list.Sort();
                this.singleTypes.AddRange(list);
            }

            int realIndex;

            if (!this.selectedType.HasValue)
            {
                realIndex = 0;
            }
            else
            {
                realIndex = this.singleTypes.IndexOf(this.selectedType.Value);
            }

            var itemPerPage = windowSize.y / 16;
            var offset = 0f;

            if (itemPerPage > 0f)
            {
                var page = realIndex / itemPerPage;
                var floorPage = Mathf.FloorToInt(page);
                var pageOffset = page - floorPage;

                if (pageOffset > 0.5f)
                {
                    var middlePage = floorPage + 0.5f;
                    var itemToMiddle = middlePage * itemPerPage;
                    offset = (realIndex - itemToMiddle) * 1.5f;
                }
            }

            this.scrollPosition.y = 16 * realIndex + offset * 16;
            this.isSelected = false;

            var type = typeof(EditorStyles);
            var property = type.GetProperty(nameof(EditorStyles.toolbarSearchField), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            this.searchToobar = property.GetValue(null, null) as GUIStyle;
            this.searchText = string.Empty;
        }

        private void OnGUI()
        {
            InitializeTextStyle();

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUI.backgroundColor = new Color(1f, 1f, 1f, 0.5f);
            GUI.SetNextControlName("Search");
            this.searchText = EditorGUILayout.TextField("", this.searchText, this.searchToobar, GUILayout.MinWidth(95));
            EditorGUI.FocusTextInControl("Search");

            if (GUILayout.Button("S", EditorStyles.toolbarButton, GUILayout.Width(16)))
            {
                int realIndex;

                if (!this.selectedType.HasValue)
                {
                    realIndex = 0;
                }
                else
                {
                    realIndex = this.singleTypes.IndexOf(this.selectedType.Value);
                }

                this.scrollPosition.y = 16 * realIndex;
            }

            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;

            this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);

            for (var i = 0; i < this.singleTypes.Count; i++)
            {
                var single = this.singleTypes[i];

                if (!string.IsNullOrEmpty(this.searchText) && !single.type.FullName.ToLower().Contains(this.searchText))
                {
                    continue;
                }

                Rect rect;

                if (single.isSelect)
                {
                    rect = EditorGUILayout.BeginHorizontal(this.selectedBackgroundStyle);
                }
                else
                {
                    rect = EditorGUILayout.BeginHorizontal(this.normalBackgroundStyle);
                }

                GUILayout.BeginHorizontal();

                var annotation = "";

                if (!single.type.IsPrimitive())
                {
                    if (single.type.IsEnum)
                    {
                        annotation = single.type.GetCustomAttribute<FlagsAttribute>() == null ? "enum" : "flag";
                    }
                    else
                    {
                        annotation = single.type.IsValueType ? "struct" : "class";
                    }
                }

                GUILayout.Label(annotation, this.annotationStyle);
                GUILayout.Label($"<b>{single.type.GetNiceName()}</b> : {single.type.Namespace}", this.typeNameStyle);

                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();

                if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
                {
                    this.selectedIndex = single.index;
                    this.onSelect?.Invoke(this);
                    this.isSelected = true;
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            if (this.isSelected)
            {
                this.isSelected = false;
                Close();
            }
        }

        private void InitializeTextStyle()
        {
            if (!this.isInitedStype)
            {
                this.annotationStyle = new GUIStyle(EditorStyles.label) {
                    fixedHeight = 16,
                    alignment = TextAnchor.MiddleLeft,
                };

                var size = this.annotationStyle.CalcSize(new GUIContent("struct"));
                this.annotationStyle.fixedWidth = size.x;

                this.typeNameStyle = new GUIStyle(EditorStyles.label) {
                    fixedHeight = 16,
                    alignment = TextAnchor.MiddleLeft,
                    richText = true
                };

                var selectedBg = new Texture2D(32, 32, TextureFormat.RGB24, false);
                var hightLightBg = new Texture2D(32, 32, TextureFormat.RGB24, false);
                if (EditorGUIUtility.isProSkin)
                {
                    selectedBg.LoadImage(Convert.FromBase64String(s_SelectedBg_Pro));
                    hightLightBg.LoadImage(Convert.FromBase64String(s_HightLightBg_Pro));
                }
                else
                {
                    selectedBg.LoadImage(Convert.FromBase64String(s_SelectedBg_Light));
                    hightLightBg.LoadImage(Convert.FromBase64String(s_HightLightBg_Light));
                }
                this.selectedBackgroundStyle = new GUIStyle();
                this.selectedBackgroundStyle.normal.background = selectedBg;
                this.normalBackgroundStyle = new GUIStyle();
                this.normalBackgroundStyle.hover.background = hightLightBg;

                this.isInitedStype = true;
            }
        }

        private void Update()
        {
            Repaint();
        }

        private const string s_SelectedBg_Pro = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAIAAAD8GO2jAAAAQklEQVRIDe3SsQkAAAgDQXWN7L+nOMFXdm8dIhzpJPV581l+3T5AYYkkQgEMuCKJUAADrkgiFMCAK5IIBTDgipBoAWXpAJEoZnl3AAAAAElFTkSuQmCC";
        private const string s_HightLightBg_Pro = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAIAAAD8GO2jAAAAQklEQVRIDe3SsQkAAAgDQXXD7L+MOMFXdm8dIhzpJPV581l+3T5AYYkkQgEMuCKJUAADrkgiFMCAK5IIBTDgipBoARFdATMHrayuAAAAAElFTkSuQmCC";
        private const string s_SelectedBg_Light = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAIAAAD8GO2jAAAAQUlEQVRIDe3SsQkAAAgDQXV/yMriBF/ZvXWIcKST1OfNZ/l1+wCFJZIIBTDgiiRCAQy4IolQAAOuSCIUwIArQqIF36EB7diYDg8AAAAASUVORK5CYII=";
        private const string s_HightLightBg_Light = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAIAAAD8GO2jAAAAQklEQVRIDe3SsQkAAAgDQXX/ETOMOMFXdm8dIhzpJPV581l+3T5AYYkkQgEMuCKJUAADrkgiFMCAK5IIBTDgipBoAc9YAtQLJ3kPAAAAAElFTkSuQmCC";

        private struct SingleType : IEquatable<SingleType>, IEqualityComparer<SingleType>, IComparable<SingleType>
        {
            public Type type;

            public int index;

            public bool isSelect;

            public override int GetHashCode()
            {
                return this.type == null ? 0 : this.type.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj is SingleType other)
                    return object.ReferenceEquals(this.type, other.type);

                return false;
            }

            public bool Equals(SingleType other)
            {
                return object.ReferenceEquals(this.type, other.type);
            }

            public bool Equals(SingleType x, SingleType y)
            {
                return object.ReferenceEquals(x.type, y.type);
            }

            public int GetHashCode(SingleType obj)
            {
                return obj.GetHashCode();
            }

            public int CompareTo(SingleType other)
            {
                return string.Compare(this.type.Name, other.type.Name);
            }
        }
    }

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
                    this._onLeftButtonClick?.Invoke(null);
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
                this._onKeyWordChange?.Invoke(this.KeyWord);
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
        private Type[] observableTypes;
        private Action<CustomPropertyInfo> _onPropertyChanged;
        private Action<CustomPropertyInfo> _onPropertyDelete;

        public EditablePropertyDrawer(UIManConfig config, Type viewModelType, CustomPropertyInfo property, Action<CustomPropertyInfo> onPropertyChanged, Action<CustomPropertyInfo> onPropertyDelete)
        {
            this._viewModelType = viewModelType;
            this._property = property;
            this._onPropertyChanged = onPropertyChanged;
            this._onPropertyDelete = onPropertyDelete;

            this.observableTypes = UIManEditorReflection.GetAllObservableTypes(this._viewModelType, config.classNamespace);

            for (var i = 0; i < this.observableTypes.Length; i++)
            {
                if (this._property.LastPropertyType == this.observableTypes[i])
                {
                    this.selectedType = i;
                    break;
                }
            }
        }

        private void Popup(int selectedIndex, Type[] types, Rect position, Action<NamePopupWindow> onSelect)
        {
            var max = 0;

            for (var i = 0; i < types.Length; i++)
            {
                if (types[i].FullName.Length > types[max].FullName.Length)
                    max = i;
            }

            Rect prefixPosition = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), GUIContent.none);
            var type = types[selectedIndex];

            var buttonStyle = new GUIStyle(EditorStyles.popup);
            buttonStyle.padding.top = 2;
            buttonStyle.padding.bottom = 2;
            var baseSize = GUI.skin.textField.CalcSize(new GUIContent(types[max].FullName));
            var popRect = new Rect(prefixPosition.x, position.y, position.x + position.width - prefixPosition.x, baseSize.y);

            if (GUI.Button(popRect, UIManReflection.GetAllias(type, false), buttonStyle))
            {
                var window = ScriptableObject.CreateInstance<NamePopupWindow>();
                var windowRect = prefixPosition;
                var windowSize = new Vector2(windowRect.width + baseSize.x, 400);

                window.Initialize(selectedIndex, types, windowSize, onSelect);
                windowRect.position = GUIUtility.GUIToScreenPoint(windowRect.position);
                windowRect.height = popRect.height + 1;
                window.ShowAsDropDown(windowRect, windowSize);
            }
        }

        // Converts the field value to a LayerMask
        private static LayerMask FieldToLayerMask(int field)
        {
            LayerMask mask = 0;
            var layers = InternalEditorUtility.layers;
            for (var c = 0; c < layers.Length; c++)
            {
                if ((field & (1 << c)) != 0)
                {
                    mask |= 1 << LayerMask.NameToLayer(layers[c]);
                }
            }
            return mask;
        }
        // Converts a LayerMask to a field value
        private static int LayerMaskToField(LayerMask mask)
        {
            var field = 0;
            var layers = InternalEditorUtility.layers;
            for (var c = 0; c < layers.Length; c++)
            {
                if ((mask & (1 << LayerMask.NameToLayer(layers[c]))) != 0)
                {
                    field |= 1 << c;
                }
            }
            return field;
        }

        private void OnSelectType(NamePopupWindow window)
        {
            this.selectedType = window.selectedIndex;
            this._property.LastPropertyType = this.observableTypes[this.selectedType];

            if (this._property.LastPropertyType.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                this._property.SetLastValueAs<UnityEngine.Object>(null);
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

            GUILayoutOption nameWidth = GUILayout.Width(totalWidth * 1 / 4 - 10);
            GUILayoutOption typeWidth = GUILayout.Width(totalWidth / 5 - 10);
            GUILayoutOption defaultValWidth = GUILayout.Width(totalWidth * 2 / 5 - 10);
            GUILayoutOption buttonWidth = GUILayout.Width(totalWidth / 16 - 5);

            // Property name
            this._property.LastName = EditorGUILayout.TextField(this._property.LastName, nameWidth);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Space(4.5f);

            // Property type
            var rect = EditorGUILayout.GetControlRect(typeWidth);
            Popup(this.selectedType, this.observableTypes, rect, OnSelectType);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Space(4);

            var type = this._property.LastPropertyType;

            // Default value
            if (type == typeof(bool))
            {
                this._property.SetLastValueAs(EditorGUILayout.Toggle(this._property.GetLastValueAs<bool>(), defaultValWidth));
            }
            else if (type == typeof(byte))
            {
                this._property.SetLastValueAs(EditorGUILayout.IntField(this._property.GetLastValueAs<byte>(), defaultValWidth));
            }
            else if (type == typeof(sbyte))
            {
                this._property.SetLastValueAs(EditorGUILayout.IntField(this._property.GetLastValueAs<sbyte>(), defaultValWidth));
            }
            else if (type == typeof(short))
            {
                this._property.SetLastValueAs(EditorGUILayout.IntField(this._property.GetLastValueAs<short>(), defaultValWidth));
            }
            else if (type == typeof(ushort))
            {
                this._property.SetLastValueAs(EditorGUILayout.IntField(this._property.GetLastValueAs<ushort>(), defaultValWidth));
            }
            else if (type == typeof(int))
            {
                this._property.SetLastValueAs(EditorGUILayout.IntField(this._property.GetLastValueAs<int>(), defaultValWidth));
            }
            else if (type == typeof(uint))
            {
                this._property.SetLastValueAs(EditorGUILayout.LongField(this._property.GetLastValueAs<uint>(), defaultValWidth));
            }
            else if (type == typeof(long))
            {
                this._property.SetLastValueAs(EditorGUILayout.LongField(this._property.GetLastValueAs<long>(), defaultValWidth));
            }
            else if (type == typeof(float))
            {
                this._property.SetLastValueAs(EditorGUILayout.FloatField(this._property.GetLastValueAs<float>(), defaultValWidth));
            }
            else if (type == typeof(double))
            {
                this._property.SetLastValueAs(EditorGUILayout.DoubleField(this._property.GetLastValueAs<double>(), defaultValWidth));
            }
            else if (type == typeof(char))
            {
                if (this._property.DefaltValue == null)
                {
                    this._property.DefaltValue = string.Empty;
                    this._property.LastValue = string.Empty;
                }

                var val = EditorGUILayout.TextField(this._property.GetLastValueAs<char>().ToString(), defaultValWidth);
                this._property.SetLastValueAs(string.IsNullOrEmpty(val) ? (char)0 : val[0]);
            }
            else if (type == typeof(string))
            {
                if (this._property.DefaltValue == null)
                {
                    this._property.DefaltValue = string.Empty;
                    this._property.LastValue = string.Empty;
                }

                this._property.SetLastValueAs(EditorGUILayout.TextField(this._property.GetLastValueAs<string>(), defaultValWidth));
            }
            else if (type.IsEnum)
            {
                var mask = type.GetCustomAttribute<FlagsAttribute>();
                var value = this._property.GetLastValueAs<Enum>();

                if (value == null)
                {
                    value = type.GetEnumValues().GetValue(0) as Enum;
                }

                if (mask == null)
                    this._property.SetLastValueAs(EditorGUILayout.EnumPopup(value, defaultValWidth));
                else
                    this._property.SetLastValueAs(EditorGUILayout.EnumFlagsField(value, defaultValWidth));
            }
            else if (type == typeof(Color))
            {
                this._property.SetLastValueAs(EditorGUILayout.ColorField(this._property.GetLastValueAs<Color>(), defaultValWidth));
            }
            else if (type == typeof(Vector2))
            {
                this._property.SetLastValueAs(EditorGUILayout.Vector2Field(string.Empty, this._property.GetLastValueAs<Vector2>(), defaultValWidth));
            }
            else if (type == typeof(Vector2Int))
            {
                this._property.SetLastValueAs(EditorGUILayout.Vector2IntField(string.Empty, this._property.GetLastValueAs<Vector2Int>(), defaultValWidth));
            }
            else if (type == typeof(Vector3))
            {
                this._property.SetLastValueAs(EditorGUILayout.Vector3Field(string.Empty, this._property.GetLastValueAs<Vector3>(), defaultValWidth));
            }
            else if (type == typeof(Vector3Int))
            {
                this._property.SetLastValueAs(EditorGUILayout.Vector3IntField(string.Empty, this._property.GetLastValueAs<Vector3Int>(), defaultValWidth));
            }
            else if (type == typeof(Vector4))
            {
                this._property.SetLastValueAs(EditorGUILayout.Vector4Field(string.Empty, this._property.GetLastValueAs<Vector4>(), defaultValWidth));
            }
            else if (type == typeof(Bounds))
            {
                this._property.SetLastValueAs(EditorGUILayout.BoundsField(this._property.GetLastValueAs<Bounds>(), defaultValWidth));
            }
            else if (type == typeof(BoundsInt))
            {
                this._property.SetLastValueAs(EditorGUILayout.BoundsIntField(this._property.GetLastValueAs<BoundsInt>(), defaultValWidth));
            }
            else if (type == typeof(Rect))
            {
                this._property.SetLastValueAs(EditorGUILayout.RectField(this._property.GetLastValueAs<Rect>(), defaultValWidth));
            }
            else if (type == typeof(RectInt))
            {
                this._property.SetLastValueAs(EditorGUILayout.RectIntField(this._property.GetLastValueAs<RectInt>(), defaultValWidth));
            }
            else if (type == typeof(LayerMask))
            {
                EditorGUI.BeginChangeCheck();
                var maskField = EditorGUILayout.MaskField(LayerMaskToField(this._property.GetLastValueAs<LayerMask>()), InternalEditorUtility.layers, defaultValWidth);

                if (EditorGUI.EndChangeCheck())
                {
                    this._property.SetLastValueAs(FieldToLayerMask(maskField));
                }
            }
            else if (type.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                this._property.SetLastValueAs(EditorGUILayout.ObjectField(this._property.GetLastValueAs<UnityEngine.Object>(), type, false, defaultValWidth));
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
                    this._onPropertyChanged?.Invoke(this._property);
                }
            }

            if (ColorButton.Draw("X", CommonColor.LightRed, buttonWidth))
            {
                this._onPropertyDelete?.Invoke(this._property);
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
                    this._onSave?.Invoke(this._arrItems[this.selectedIndex]);
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
                onCreate?.Invoke(this.name);
            }
            if (GUILayout.Button("Cancel"))
            {
                onCancel?.Invoke();
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