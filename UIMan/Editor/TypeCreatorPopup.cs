using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace UnuGames
{
    public class TypeCreatorPopup : EditorWindow
    {
        private string baseType = "UIManDialog";

        private EditablePopup baseTypePopup;

        private readonly string[] arrSupportType = new string[3] {
            "ObservableModel",
            "UIManScreen",
            "UIManDialog"
        };

        private bool inited = false;
        private string typeName = "NewViewModel";

        private void Init()
        {
            if (!this.inited)
            {
                if (this.baseTypePopup == null)
                {
                    this.baseTypePopup = new EditablePopup(this.arrSupportType, "UIManDialog", null);
                }
                this.minSize = new Vector2(300, 80);
                this.maxSize = this.minSize;
                this.inited = true;
            }
        }

        private void OnGUI()
        {
            Init();
            GUILayout.Space(10);
            this.baseTypePopup.Draw();
            LineHelper.Draw(Color.black);
            GUILayout.Space(10);

            if (ColorButton.Draw("Create", CommonColor.LightGreen, GUILayout.Height(30)))
            {
                var lastPath = "";
                UIManConfig uiManConfig = Resources.Load<UIManConfig>("UIManConfig");
                if (uiManConfig != null)
                {
                    if (this.baseTypePopup.SelectedItem == this.arrSupportType[0])
                        lastPath = uiManConfig.modelScriptFolder;
                    else if (this.baseTypePopup.SelectedItem == this.arrSupportType[1])
                        lastPath = uiManConfig.screenScriptFolder;
                    else if (this.baseTypePopup.SelectedItem == this.arrSupportType[2])
                        lastPath = uiManConfig.dialogScriptFolder;
                }

                lastPath = EditorUtility.SaveFilePanel("Save script", Application.dataPath + lastPath, this.typeName, "cs");

                if (!string.IsNullOrEmpty(lastPath))
                {
                    this.typeName = Path.GetFileNameWithoutExtension(lastPath);

                    lastPath = Path.GetDirectoryName(lastPath).Replace("\\", "/").Replace(Application.dataPath, "");

                    if (this.baseTypePopup.SelectedItem == this.arrSupportType[0])
                    {
                        uiManConfig.modelScriptFolder = lastPath;
                        uiManConfig.generatingTypeIsDialog = false;
                    }
                    else if (this.baseTypePopup.SelectedItem == this.arrSupportType[1])
                    {
                        uiManConfig.screenScriptFolder = lastPath;
                        uiManConfig.generatingTypeIsDialog = false;
                    }
                    else if (this.baseTypePopup.SelectedItem == this.arrSupportType[2])
                    {
                        uiManConfig.dialogScriptFolder = lastPath;
                        uiManConfig.generatingTypeIsDialog = true;
                    }
                    EditorUtility.SetDirty(uiManConfig);

                    GenerateViewModel();
                }
            }
        }

        public void GenerateViewModel()
        {
            if (this.typeName.Contains(" "))
            {
                EditorUtility.DisplayDialog("Error", "View model name cannot constain special character", "OK");
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

            UIManConfig config = Resources.Load<UIManConfig>("UIManConfig");

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
                EditorUtility.DisplayDialog("Error", "View model name is already exist, please input other name!", "OK");
                return;
            }

            var paths = Regex.Split(savePath, "/");
            var scriptName = paths[paths.Length - 1].Replace(".cs", "");

            if (this.baseType != this.arrSupportType[0])
                config.generatingType = this.typeName;

            var code = CodeGenerationHelper.GenerateScript(this.typeName, this.baseType);
            CodeGenerationHelper.SaveScript(savePath, code, true);

            if (this.baseType != this.arrSupportType[0])
                GenerateViewModelHandler(savePath);

            AssetDatabase.Refresh(ImportAssetOptions.Default);

            if (warn)
            {
                Debug.LogWarning("Code generation warning: Invalid name detected, auto generate is activated!");
            }

            Close();
        }

        public void GenerateViewModelHandler(string scriptPath)
        {
            var handlerScriptPath = CodeGenerationHelper.GeneratPathWithSubfix(scriptPath, ".Handler.cs");
            var handlerCode = "";

            if (string.IsNullOrEmpty(handlerCode))
                handlerCode = CodeGenerationHelper.GenerateViewModelHandler(this.typeName, this.baseType);
            else
                handlerCode = handlerCode.Replace(": " + this.typeName, ": " + this.baseType);

            CodeGenerationHelper.SaveScript(handlerScriptPath, handlerCode, false, this.typeName, this.baseType);
        }
    }
}