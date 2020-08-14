using System.IO;
using UnityEditor;
using UnityEngine;
using UnuGames.MVVM;

namespace UnuGames
{
    public class TypeCreatorPopup : EditorWindow
    {
        private string baseType = nameof(UIManDialog);

        private TextFieldHelper namespaceField;
        private EditablePopup baseTypePopup;

        private bool inited = false;
        private string typeName = "NewViewModel";

        private void Initialize()
        {
            if (!this.inited)
            {
                if (this.baseTypePopup == null)
                {
                    var config = EditorHelper.GetOrCreateScriptableObject<UIManConfig>();
                    this.namespaceField = new TextFieldHelper(config.classNamespace);
                    this.baseTypePopup = new EditablePopup(UIGenerator.GetSupportTypes(), config.name, null);
                }
                this.minSize = new Vector2(300, 160);
                this.maxSize = this.minSize;
                this.inited = true;
            }
        }

        private void OnGUI()
        {
            Initialize();

            GUILayout.Space(10);
            LabelHelper.HeaderLabel("Type");
            LineHelper.Draw(CommonColor.GetGray());
            this.baseTypePopup.Draw();

            GUILayout.Space(10);
            LabelHelper.HeaderLabel("Namespace");
            LineHelper.Draw(CommonColor.GetGray());
            this.namespaceField.Draw(GUIContent.none, 0);

            GUILayout.Space(10);

            if (ColorButton.Draw("Create", CommonColor.LightGreen, GUILayout.Height(30)))
            {
                var lastPath = "";
                var config = EditorHelper.GetOrCreateScriptableObject<UIManConfig>(false);

                if (config != null)
                {
                    if (UIGenerator.IsObservableModelType(this.baseTypePopup.SelectedItem))
                    {
                        lastPath = config.modelScriptFolder;
                        this.typeName = "NewViewModel";
                    }
                    else if (UIGenerator.IsScreenType(this.baseTypePopup.SelectedItem))
                    {
                        lastPath = config.screenScriptFolder;
                        this.typeName = "UINewScreen";
                    }
                    else if (UIGenerator.IsDialogType(this.baseTypePopup.SelectedItem))
                    {
                        lastPath = config.dialogScriptFolder;
                        this.typeName = "UINewDialog";
                    }
                    else if (UIGenerator.IsActivityType(this.baseTypePopup.SelectedItem))
                    {
                        lastPath = config.activityScriptFolder;
                        this.typeName = "UINewActivity";
                    }
                }

                lastPath = EditorUtility.SaveFilePanel("Save script", Application.dataPath + lastPath, this.typeName, "cs");

                if (!string.IsNullOrEmpty(lastPath))
                {
                    this.typeName = Path.GetFileNameWithoutExtension(lastPath);

                    lastPath = Path.GetDirectoryName(lastPath).Replace("\\", "/").Replace(Application.dataPath, "");

                    if (UIGenerator.IsObservableModelType(this.baseTypePopup.SelectedItem))
                    {
                        config.modelScriptFolder = lastPath;
                    }
                    else if (UIGenerator.IsScreenType(this.baseTypePopup.SelectedItem))
                    {
                        config.screenScriptFolder = lastPath;
                    }
                    else if (UIGenerator.IsDialogType(this.baseTypePopup.SelectedItem))
                    {
                        config.dialogScriptFolder = lastPath;
                    }
                    else if (UIGenerator.IsActivityType(this.baseTypePopup.SelectedItem))
                    {
                        config.activityScriptFolder = lastPath;
                    }

                    EditorUtility.SetDirty(config);
                    GenerateType();
                }
            }
        }

        public void GenerateType()
        {
            if (this.typeName.Contains(" "))
            {
                EditorUtility.DisplayDialog("Error", "Class name cannot constain special characters", "OK");
                return;
            }

            var warn = false;

            if (this.typeName.Length <= 1 ||
                (!this.typeName.Substring(0, 2).Equals("UI") &&
                 !UIGenerator.IsObservableModelType(this.baseTypePopup.SelectedItem)))
            {
                this.typeName = "UI" + this.typeName;
                warn = true;
            }

            this.baseType = this.baseTypePopup.SelectedItem;

            var config = EditorHelper.GetOrCreateScriptableObject<UIManConfig>(false);
            var savePath = "";

            if (UIGenerator.IsObservableModelType(this.baseType))
            {
                savePath = config.modelScriptFolder;
            }
            else if (UIGenerator.IsScreenType(this.baseType))
            {
                savePath = config.screenScriptFolder;
            }
            else if (UIGenerator.IsDialogType(this.baseType))
            {
                savePath = config.dialogScriptFolder;
            }
            else if (UIGenerator.IsActivityType(this.baseType))
            {
                savePath = config.activityScriptFolder;
            }

            savePath = Application.dataPath + "/" + savePath + "/" + this.typeName + ".cs";

            if (File.Exists(savePath) || UIGenerator.IsViewModelExisted(this.typeName))
            {
                EditorUtility.DisplayDialog("Error", "A class of the same name has already existed", "OK");
                return;
            }

            var inheritance = string.Empty;
            config.generatingBaseType = this.baseType;

            if (!UIGenerator.IsObservableModelType(this.baseType))
                config.generatingType = this.typeName;
            else
            {
                config.generatingType = null;
                inheritance = $" : {this.baseType}";
            }

            var code = UIManCodeGenerator.GenerateType(this.typeName, inheritance, false, config, this.namespaceField.Text);
            UIManCodeGenerator.SaveScript(savePath, code, true);

            if (!UIGenerator.IsObservableModelType(this.baseType))
                GenerateHandler(savePath);

            AssetDatabase.Refresh(ImportAssetOptions.Default);

            if (warn)
            {
                UnuLogger.LogWarning("Code generation warning: Class name is invalid. New name is generated.");
            }

            Close();
        }

        public void GenerateHandler(string scriptPath)
        {
            var handlerScriptPath = UIManCodeGenerator.GeneratPathWithSubfix(scriptPath, ".Handler.cs");
            var config = EditorHelper.GetOrCreateScriptableObject<UIManConfig>(false);
            var isActivity = UIGenerator.IsActivityType(this.baseType);
            var handlerCode = UIManCodeGenerator.GenerateHandler(this.typeName, this.baseType, isActivity, config, this.namespaceField.Text);

            UIManCodeGenerator.SaveScript(handlerScriptPath, handlerCode, false, this.typeName, this.baseType);
        }
    }
}