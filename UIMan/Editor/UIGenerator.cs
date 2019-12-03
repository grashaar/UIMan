using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnuGames.MVVM;

namespace UnuGames
{
    public class UIGenerator : EditorWindow
    {
        private const string PREFAB_EXT = ".prefab";

        private static string _savedDirectory;
        private static UIGenerator _container;
        private static string[] _types;
        private static float _typeAreaWidth = 250f;
        private static float _propertiesAreaWidth = 640f;
        private static Vector2 _propertiesScrollPos;

        private readonly static Dictionary<Type, EditablePropertyDrawer[]> _propertiesDrawerCache = new Dictionary<Type, EditablePropertyDrawer[]>();
        private static Type _selectedType = null;
        private static CustomPropertyInfo[] _selectedProperties = null;

        private static string _currentScriptPath = null;
        private static string _handlerScriptPath = null;

        private readonly static string[] _arrSupportType = new string[3] {
            nameof(ObservableModel),
            nameof(UIManScreen),
            nameof(UIManDialog)
        };

        private static bool _reload = true;
        private static UIManConfig _config;

        private UISearchField searchField;
        private ListView listTypes;
        private TextFieldHelper namespaceField;
        private EditablePopup baseTypePopup;

        public static string GetSupportTypeName(int index)
        {
            return _arrSupportType[index];
        }

        public static bool IsViewModelExisted(string name)
        {
            return ArrayUtility.Contains(_types, name);
        }

        [MenuItem("UIMan/UI Generator", false, -1)]
        private static void Init()
        {
            UIManEditorReflection.RefreshAssemblies(false);
            _types = UIManEditorReflection.GetAllUIManTypes();
            _container = EditorWindow.GetWindow<UIGenerator>(true, "UIMan - UI Generator");
            _container.minSize = new Vector2(900, 600);
            _container.maxSize = _container.minSize;
            GetConfig();
        }

        [MenuItem("UIMan/Component/", true)]
        private static void ComponentsRoot()
        {
        }

        [MenuItem("UIMan/Component/Data Context", false, 2)]
        private static void AttachDataContext()
        {
            if (Selection.activeGameObject != null)
                Selection.activeGameObject.AddComponent(typeof(DataContext));
        }

        [MenuItem("UIMan/Component/Text Binder", false, 2)]
        private static void AttachTextBinder()
        {
            if (Selection.activeGameObject != null)
                Selection.activeGameObject.AddComponent(typeof(TextBinder));
        }

        [MenuItem("UIMan/Component/Observale List Binder", false, 2)]
        private static void AttachObservaleListBinder()
        {
            if (Selection.activeGameObject != null)
                Selection.activeGameObject.AddComponent(typeof(ObservableListBinder));
        }

        [MenuItem("UIMan/Component/Visible Binder", false, 2)]
        private static void AttachVisibleBinder()
        {
            if (Selection.activeGameObject != null)
                Selection.activeGameObject.AddComponent(typeof(SetActiveBinder));
        }

        [MenuItem("UIMan/Component/Input Binder", false, 2)]
        private static void AttachInputBinder()
        {
            if (Selection.activeGameObject != null)
                Selection.activeGameObject.AddComponent(typeof(InputFieldBinder));
        }

        [MenuItem("UIMan/Component/Internal Image Binder", false, 2)]
        private static void AttachInternalImageBinder()
        {
            if (Selection.activeGameObject != null)
                Selection.activeGameObject.AddComponent(typeof(InternalImageBinder));
        }

        [MenuItem("UIMan/Component/External Image Binder", false, 2)]
        private static void AttachExternalImageBinder()
        {
            if (Selection.activeGameObject != null)
                Selection.activeGameObject.AddComponent(typeof(ExternalImageBinder));
        }

        [MenuItem("UIMan/Component/Progress Binder", false, 2)]
        private static void AttachProgressBinder()
        {
            if (Selection.activeGameObject != null)
                Selection.activeGameObject.AddComponent(typeof(ProgressBarBinder));
        }

        [MenuItem("UIMan/Component/Enable Binder", false, 2)]
        private static void AttachEnableBinder()
        {
            if (Selection.activeGameObject != null)
                Selection.activeGameObject.AddComponent(typeof(InteractableBinder));
        }

        [MenuItem("UIMan/Component/Number Binder", false, 2)]
        private static void AttachNumberBinder()
        {
            if (Selection.activeGameObject != null)
                Selection.activeGameObject.AddComponent(typeof(NumberBinder));
        }

        [MenuItem("UIMan/Prefab/Edit UIRoot", false, 3)]
        private static void EditRoot()
        {
            Selection.activeObject = UIMan.Instance;
        }

        [MenuItem("UIMan/Prefab/Edit Activity Indicator", false, 4)]
        private static void EditLoading()
        {
            var obj = Resources.Load("UIActivityIndicator") as GameObject;
            if (obj != null)
            {
                var editorPath = AssetDatabase.GetAssetPath(obj);
                var prefab = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(editorPath)) as GameObject;
                prefab.transform.SetParent(UIMan.Instance.transform, false);
                prefab.transform.SetAsLastSibling();
                Selection.activeObject = prefab;
            }
            else
            {
                UnuLogger.LogError("ActivityIndicator prefab could not be found! Please re-install UIMan!");
            }
        }

        [MenuItem("UIMan/Configuration", false)]
        private static void Config()
        {
            GetConfig();
            Selection.activeObject = _config;
        }

        [MenuItem("UIMan/Documentation", false)]
        private static void Docs()
        {
            Application.OpenURL("https://goo.gl/PyXBBU");
        }

        private static void GetConfig()
        {
            _config = Resources.Load<UIManConfig>("UIManConfig");
            if (_config == null)
                ConfigFile.Create<UIManConfig>();
        }

        private void OnGUI()
        {
            if (EditorApplication.isCompiling)
            {
                ShowNotification(new GUIContent("Unity is compiling..."));
                GUI.enabled = false;
            }
            else
            {
                GUI.enabled = true;
            }

            if (_container == null)
            {
                Init();
            }

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical("Box", GUILayout.Width(_typeAreaWidth), GUILayout.ExpandHeight(true));
            TypeWindow();
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box", GUILayout.Width(_propertiesAreaWidth), GUILayout.ExpandHeight(true));
            PropertiesWindow();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        private void TypeWindow(int id = 0)
        {
            GUILayout.BeginVertical();

            GUILayout.Space(2);

            GUILayout.EndVertical();

            if (this.searchField == null)
                this.searchField = new UISearchField(OnSearchType, OnClickCreateType);

            this.searchField.Draw();

            if (this.listTypes == null)
                this.listTypes = new ListView();

            this.listTypes.SetData(-1, _types, false, OnSelecType, this.searchField.KeyWord, this);
            this.listTypes.Draw();

            if (_reload && _config != null)
            {
                if (!string.IsNullOrEmpty(_config.selectedType))
                    this.listTypes.Select(_config.selectedType);

                MakePrefab();

                _reload = false;
            }
        }

        private void LayoutWindow(int id = 1)
        {
            GUILayout.Label("NO LAYOUT HAS BEEN SELECTED!");
        }

        private void PropertiesWindow(int id = 2)
        {
            GUILayout.BeginVertical();

            if (this.listTypes != null && !string.IsNullOrEmpty(this.listTypes.SelectedViewItem))
            {
                DrawSelectedType(id);
            }
            else
            {
                GUILayout.Label("NO DATA FOR PREVIEW!");
            }

            GUILayout.EndVertical();
        }

        private void DrawSelectedType(int id)
        {
            if (_selectedType != null)
            {
                DrawSelectedTypeHeader();
            }

            GUILayout.Space(10);

            // Add property
            if (ColorButton.Draw("Add New Property", CommonColor.LightGreen, GUILayout.Height(30)))
            {
                var newIndex = 0;
                var strNewIndex = "";
                for (var i = 0; i < _selectedProperties.Length; i++)
                {
                    if (_selectedProperties[i].LastName.Contains("NewProperty"))
                        newIndex++;
                }
                if (newIndex > 0)
                    strNewIndex = newIndex.ToString();
                var newProperty = new CustomPropertyInfo("", typeof(string)) {
                    LastName = "NewProperty" + strNewIndex
                };
                ArrayUtility.Add(ref _selectedProperties, newProperty);
                CachePropertiesDrawer();
            }

            //Save all change
            var changeList = new CustomPropertyInfo[0];
            var selectedList = new CustomPropertyInfo[0];

            for (var i = 0; i < _selectedProperties.Length; i++)
            {
                if (_selectedProperties[i].HasChange)
                    ArrayUtility.Add(ref changeList, _selectedProperties[i]);
                if (_selectedProperties[i].IsSelected)
                    ArrayUtility.Add(ref selectedList, _selectedProperties[i]);
            }

            GUILayout.Space(10);
            LineHelper.Draw(Color.gray);
            GUILayout.Space(5);

            if (changeList.Length > 0 || !string.Equals(_selectedType.Namespace, this.namespaceField.Text))
            {
                if (ColorButton.Draw("Save All Changes", CommonColor.LightGreen, GUILayout.Height(30)))
                {
                    for (var i = 0; i < changeList.Length; i++)
                    {
                        changeList[i].CommitChange();
                    }
                    SaveCurrentType(true, this.baseTypePopup.SelectedItem);
                }
            }

            if (selectedList.Length > 0)
            {
                if (ColorButton.Draw("Delete Selected Properties", CommonColor.LightRed, GUILayout.Height(30)))
                {
                    for (var i = 0; i < selectedList.Length; i++)
                    {
                        ArrayUtility.Remove(ref _selectedProperties, selectedList[i]);
                    }
                    SaveCurrentType(true, this.baseTypePopup.SelectedItem);
                    CachePropertiesDrawer(true);
                }
            }

            if (_selectedProperties.Length > 0)
            {
                if (ColorButton.Draw("Delete All Properties", CommonColor.LightRed, GUILayout.Height(30)))
                {
                    while (_selectedProperties.Length > 0)
                    {
                        ArrayUtility.Clear(ref _selectedProperties);
                        SaveCurrentType();
                        CachePropertiesDrawer(true);
                    }
                }
            }
        }

        private void DrawSelectedTypeHeader()
        {
            // Title
            GUILayout.Space(2);
            LabelHelper.TitleLabel(_selectedType.Name);
            LineHelper.Draw(Color.gray);

            // Common
            GUILayout.Space(2);

            if (_selectedType.BaseType != typeof(ObservableModel))
            {
                DrawHeaderButtons();
            }
            else
            {
                DrawHeaderButtonsObservableModel();
            }

            // Base type
            GUILayout.Space(10);
            LabelHelper.HeaderLabel("Type");
            LineHelper.Draw(Color.gray);
            this.baseTypePopup.Draw();

            if (this.baseTypePopup.SelectedItem != nameof(ObservableModel))
            {
                if (!System.IO.File.Exists(_handlerScriptPath))
                {
                    if (GUILayout.Button("Generate Handler"))
                    {
                        var backupCode = UIManCodeGenerator.DeleteScript(_handlerScriptPath);
                        GenerateViewModelHandler(backupCode, _selectedType.BaseType.Name);
                    }
                }
            }

            // Namespace
            GUILayout.Space(10);
            LabelHelper.HeaderLabel("Namespace");
            LineHelper.Draw(Color.gray);
            this.namespaceField.Draw(GUIContent.none, 0);

            if (this.baseTypePopup.SelectedItem != nameof(ObservableModel) &&
                !string.Equals(_selectedType.Namespace, this.namespaceField.Text))
            {
                EditorGUILayout.HelpBox($"Must manually change the namespace in {_selectedType.Name}.Handler.cs", MessageType.Warning);
            }

            // Properties
            GUILayout.Space(10);
            LabelHelper.HeaderLabel("Properties");
            LineHelper.Draw(Color.gray);

            _propertiesScrollPos = EditorGUILayout.BeginScrollView(_propertiesScrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            if (_propertiesDrawerCache.ContainsKey(_selectedType))
            {
                EditablePropertyDrawer[] props = _propertiesDrawerCache[_selectedType];
                for (var i = 0; i < props.Length; i++)
                {
                    props[i].Draw(_propertiesAreaWidth);
                }
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawHeaderButtonsObservableModel()
        {
            if (ColorButton.Draw("Migrate To New Version", CommonColor.LightOrange, GUILayout.Height(30)))
            {
                SaveCurrentType(true, this.baseTypePopup.SelectedItem);
                CachePropertiesDrawer(true);
            }

            LineHelper.Draw(Color.gray);
        }

        private void DrawHeaderButtons()
        {
            GUILayout.BeginHorizontal();
            if (ColorButton.Draw("Edit View Logic (Handler)", CommonColor.LightBlue, GUILayout.Height(30)))
            {
                var handler = UIManCodeGenerator.GetScriptPathByType(_selectedType);
                handler = handler.Replace(".cs", ".Handler.cs");
                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(handler, 1);
            }

            if (ColorButton.Draw("Edit View (UI)", CommonColor.LightBlue, GUILayout.Height(30)))
            {
                GameObject prefabInstance;
                UnityEngine.Object obj = FindObjectOfType(_selectedType);
                if (obj != null)
                {
                    prefabInstance = ((MonoBehaviour)obj).gameObject;
                }
                else
                {
                    var isDialog = _selectedType.BaseType == typeof(UIManDialog);
                    var prefabFolder = GetUIPrefabPath(_selectedType, isDialog);
                    var prefabFile = _selectedType.Name + PREFAB_EXT;
                    var prefabPath = Path.Combine(prefabFolder, prefabFile);
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                    if (prefab == null)
                    {
                        prefab = FindAssetObject<GameObject>(_selectedType.Name, PREFAB_EXT);
                    }

                    prefabInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    if (isDialog)
                        prefabInstance.transform.SetParent(UIMan.Instance.dialogRoot, false);
                    else
                        prefabInstance.transform.SetParent(UIMan.Instance.screenRoot, false);
                }
                Selection.activeGameObject = prefabInstance;
            }

            if (ColorButton.Draw("Migrate To New Version", CommonColor.LightOrange, GUILayout.Height(30)))
            {
                SaveCurrentType(true, this.baseTypePopup.SelectedItem);
                CachePropertiesDrawer(true);
            }

            if (ColorButton.Draw("Delete", CommonColor.LightRed, GUILayout.Height(30)))
            {
                var cs = UIManCodeGenerator.GetScriptPathByType(_selectedType);
                var handler = cs.Replace(".cs", ".Handler.cs");
                AssetDatabase.DeleteAsset(cs);
                AssetDatabase.DeleteAsset(handler);

                var isDialog = _selectedType.BaseType == typeof(UIManDialog);
                var prefabFolder = GetUIPrefabPath(_selectedType, isDialog);
                var prefabFile = _selectedType.Name + PREFAB_EXT;
                var prefabPath = UIManDefine.ASSETS_FOLDER + prefabFolder + prefabFile;
                AssetDatabase.DeleteAsset(prefabPath);
                AssetDatabase.Refresh();
            }

            GUILayout.EndHorizontal();
            LineHelper.Draw(Color.gray);
        }

        private void CachePropertiesDrawer(bool clearCurrentCache = false)
        {
            if (_selectedType == null)
                return;
            if (clearCurrentCache)
                _propertiesDrawerCache.Clear();
            if (!_propertiesDrawerCache.ContainsKey(_selectedType))
            {
                _propertiesDrawerCache.Add(_selectedType, new EditablePropertyDrawer[0]);
            }
            var drawers = new EditablePropertyDrawer[0];
            for (var i = 0; i < _selectedProperties.Length; i++)
            {
                var drawer = new EditablePropertyDrawer(_config, _selectedType, _selectedProperties[i], OnApplyPropertyChanged, OnPropertyDelete);
                ArrayUtility.Add(ref drawers, drawer);
            }
            _propertiesDrawerCache[_selectedType] = drawers;
        }

        public void OnSearchType(string keyword)
        {
        }

        public void OnClickCreateType(object obj)
        {
            GetWindow<TypeCreatorPopup>(true, "Create new type", true);
        }

        public void OnChangeBaseType(string newBaseType)
        {
            SaveCurrentType(true, newBaseType);
        }

        public void OnSelecType(string typeName)
        {
            _config.selectedType = typeName;
            _selectedType = UIManEditorReflection.GetTypeByName(typeName);
            _selectedProperties = _selectedType.GetUIManProperties(true);
            this.namespaceField = new TextFieldHelper(_selectedType.Namespace);
            this.baseTypePopup = new EditablePopup(_arrSupportType, _selectedType.BaseType.Name, OnChangeBaseType);
            _currentScriptPath = UIManCodeGenerator.GetScriptPathByType(_selectedType);
            _handlerScriptPath = UIManCodeGenerator.GeneratPathWithSubfix(_currentScriptPath, ".Handler.cs");
            CachePropertiesDrawer();
        }

        public void OnApplyPropertyChanged(CustomPropertyInfo newInfo)
        {
            SaveCurrentType(true);
        }

        public void OnPropertyDelete(CustomPropertyInfo property)
        {
            ArrayUtility.Remove(ref _selectedProperties, property);
            SaveCurrentType();
            CachePropertiesDrawer();
        }

        public void SaveCurrentType(bool warning = false, string baseType = null)
        {
            // Verify properties list
            for (var i = 0; i < _selectedProperties.Length; i++)
            {
                CustomPropertyInfo property = _selectedProperties[i];
                if (string.IsNullOrEmpty(property.Name) || char.IsNumber(property.Name[0]))
                {
                    property.Name = "";
                    if (warning)
                        EditorUtility.DisplayDialog("Save script error", "Property name cannot be a digit, null or empty!", "OK");
                    return;
                }

                for (var j = 0; j < _selectedProperties.Length; j++)
                {
                    if (j != i && _selectedProperties[i].Name.ToLower() == _selectedProperties[j].Name.ToLower())
                    {
                        _selectedProperties[j].Name = "";
                        if (warning)
                            EditorUtility.DisplayDialog("Save script error", "There are one or more properties are have the same name!", "OK");
                        return;
                    }
                }
            }

            if (baseType == null)
                baseType = _selectedType.BaseType.Name;

            var inheritance = baseType == nameof(ObservableModel) ? $": {baseType}" : string.Empty;

            if (!string.IsNullOrEmpty(_currentScriptPath))
            {
                var backupCode = UIManCodeGenerator.DeleteScript(_handlerScriptPath);
                var code = UIManCodeGenerator.GenerateScript(_selectedType.Name, inheritance, _config, this.namespaceField.Text, _selectedProperties);

                var saved = UIManCodeGenerator.SaveScript(_currentScriptPath, code, true);

                if (baseType != nameof(ObservableModel))
                {
                    GenerateViewModelHandler(backupCode, baseType);
                    saved = false;
                }

                if (saved)
                {
                    AssetDatabase.Refresh(ImportAssetOptions.Default);
                }
            }
        }

        public void GenerateViewModelHandler(string backupCode, string baseType = null)
        {
            if (string.IsNullOrEmpty(baseType))
                baseType = _selectedType.BaseType.Name;

            var handlerCode = backupCode;

            if (string.IsNullOrEmpty(handlerCode))
                handlerCode = UIManCodeGenerator.GenerateViewModelHandler(_selectedType.Name, baseType, _config, this.namespaceField.Text);
            else
                handlerCode = handlerCode.Replace($": {_selectedType.BaseType.Name}", $": {baseType}");

            var saved = UIManCodeGenerator.SaveScript(_handlerScriptPath, handlerCode, false, _selectedType.BaseType.Name, baseType);

            if (saved)
            {
                AssetDatabase.Refresh(ImportAssetOptions.Default);
            }
        }

        private void MakePrefab()
        {
            if (!string.IsNullOrEmpty(_config.generatingType))
            {
                GameObject prefabTemplate = FindAssetObject<GameObject>("@UI_PREFAB_TEMPLATE", PREFAB_EXT);
                if (prefabTemplate != null)
                {
                    GameObject newPrefab = Instantiate(prefabTemplate);
                    Type generatedType = UIManEditorReflection.GetTypeByName(_config.generatingType);
                    if (generatedType != null)
                    {
                        var newVM = (ViewModelBehaviour)newPrefab.AddComponent(generatedType);
                        newPrefab.name = _config.generatingType;
                        newPrefab.GetComponent<DataContext>().viewModel = newVM;
                    }

                    var newPrefabPath = UIManDefine.ASSETS_FOLDER + (_config.generatingTypeIsDialog ? _config.dialogPrefabFolder : _config.screenPrefabFolder);
                    EditorUtils.CreatePath(newPrefabPath);
                    PrefabUtility.SaveAsPrefabAsset(newPrefab, newPrefabPath + "/" + _config.generatingType + PREFAB_EXT);

                    DestroyImmediate(newPrefab);
                }

                _config.generatingType = null;
            }
        }

        private T FindAssetObject<T>(string name, string extension) where T : UnityEngine.Object
        {
            var obj = default(T);
            var files = AssetDatabase.FindAssets(name);
            if (files != null && files.Length > 0)
            {
                foreach (var file in files)
                {
                    var filePath = AssetDatabase.GUIDToAssetPath(file);
                    if (filePath.EndsWith(extension))
                    {
                        obj = AssetDatabase.LoadAssetAtPath<T>(filePath);
                        break;
                    }
                }
            }

            return obj;
        }

        private string GetUIPrefabPath(Type uiType, bool isDialog)
        {
            var attributes = uiType.GetCustomAttributes(typeof(UIDescriptorAttribute), true);
            string url;
            if (attributes != null && attributes.Length > 0)
            {
                url = ((UIDescriptorAttribute)attributes[0]).Url;
            }
            else
            {
                if (isDialog)
                {
                    url = _config.dialogPrefabFolder;
                }
                else
                {
                    url = _config.screenPrefabFolder;
                }
            }

            return url;
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            UIManEditorReflection.RefreshAssemblies(true);
        }
    }
}