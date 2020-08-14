﻿using System.IO;
using System.Text.RegularExpressions;
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

        private readonly string[] arrSupportType = new string[3] {
            nameof(ObservableModel),
            nameof(UIManScreen),
            nameof(UIManDialog)
        };

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
                    this.baseTypePopup = new EditablePopup(this.arrSupportType, config.name, null);
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
                    if (this.baseTypePopup.SelectedItem == this.arrSupportType[0])
                    {
                        lastPath = config.modelScriptFolder;
                        this.typeName = "NewViewModel";
                    }
                    else if (this.baseTypePopup.SelectedItem == this.arrSupportType[1])
                    {
                        lastPath = config.screenScriptFolder;
                        this.typeName = "UINewScreen";
                    }
                    else if (this.baseTypePopup.SelectedItem == this.arrSupportType[2])
                    {
                        lastPath = config.dialogScriptFolder;
                        this.typeName = "UINewDialog";
                    }
                }

                lastPath = EditorUtility.SaveFilePanel("Save script", Application.dataPath + lastPath, this.typeName, "cs");

                if (!string.IsNullOrEmpty(lastPath))
                {
                    this.typeName = Path.GetFileNameWithoutExtension(lastPath);

                    lastPath = Path.GetDirectoryName(lastPath).Replace("\\", "/").Replace(Application.dataPath, "");

                    if (this.baseTypePopup.SelectedItem == this.arrSupportType[0])
                    {
                        config.modelScriptFolder = lastPath;
                        config.generatingTypeIsDialog = false;
                    }
                    else if (this.baseTypePopup.SelectedItem == this.arrSupportType[1])
                    {
                        config.screenScriptFolder = lastPath;
                        config.generatingTypeIsDialog = false;
                    }
                    else if (this.baseTypePopup.SelectedItem == this.arrSupportType[2])
                    {
                        config.dialogScriptFolder = lastPath;
                        config.generatingTypeIsDialog = true;
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
                 !this.baseTypePopup.SelectedItem.Equals(UIGenerator.GetSupportTypeName(0))))
            {
                this.typeName = "UI" + this.typeName;
                warn = true;
            }

            this.baseType = this.baseTypePopup.SelectedItem;

            var config = EditorHelper.GetOrCreateScriptableObject<UIManConfig>(false);
            var savePath = "";

            if (this.baseType.Equals(UIGenerator.GetSupportTypeName(0)))
            {
                savePath = config.modelScriptFolder;
                config.generatingTypeIsDialog = false;
            }
            else if (this.baseType.Equals(UIGenerator.GetSupportTypeName(1)))
            {
                savePath = config.screenScriptFolder;
                config.generatingTypeIsDialog = false;
            }
            else if (this.baseType.Equals(UIGenerator.GetSupportTypeName(2)))
            {
                savePath = config.dialogScriptFolder;
                config.generatingTypeIsDialog = true;
            }

            savePath = Application.dataPath + "/" + savePath + "/" + this.typeName + ".cs";

            if (File.Exists(savePath) || UIGenerator.IsViewModelExisted(this.typeName))
            {
                EditorUtility.DisplayDialog("Error", "A class of the same name has already existed", "OK");
                return;
            }

            var paths = Regex.Split(savePath, "/");
            var inheritance = string.Empty;

            if (this.baseType != this.arrSupportType[0])
                config.generatingType = this.typeName;
            else
                inheritance = $" : {this.baseType}";

            var code = UIManCodeGenerator.GenerateType(this.typeName, inheritance, false, config, this.namespaceField.Text);
            UIManCodeGenerator.SaveScript(savePath, code, true);

            if (this.baseType != this.arrSupportType[0])
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
            var handlerCode = UIManCodeGenerator.GenerateHandler(this.typeName, this.baseType, config, this.namespaceField.Text);

            UIManCodeGenerator.SaveScript(handlerScriptPath, handlerCode, false, this.typeName, this.baseType);
        }
    }
}