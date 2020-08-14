using UnityEditor;
using UnityEngine;
using UnuGames;

[CustomEditor(typeof(UIManConfig))]
public class UIManConfigEditor : Editor
{
    private readonly GUIContent namespaceGUI = new GUIContent("Namespace: ", "The default namespace for generated classes is UnuGames");
    private readonly GUIContent screenGUI = new GUIContent("Screen: ", "The default path for the system find and load screen's prefab");
    private readonly GUIContent dialogGUI = new GUIContent("Dialog: ", "The default path for the system find and load dialog's prefab");
    private readonly GUIContent bgGUI = new GUIContent("Background: ", "The default path for the system find and load background image");
    private readonly GUIContent animRootGUI = new GUIContent("Animation: ", "The default path for the system to generate animator and animations into that");

    private TextFieldHelper namespaceField;
    private PathBrowser screenPath;
    private PathBrowser dialogPath;
    private PathBrowser bgPath;
    private PathBrowser animPath;

    public override void OnInspectorGUI()
    {
        var config = this.target as UIManConfig;

        if (this.namespaceField == null || this.screenPath == null || this.dialogPath == null || this.bgPath == null)
        {
            this.namespaceField = new TextFieldHelper(config.classNamespace);
            this.screenPath = new PathBrowser(config.screenPrefabFolder, Application.dataPath);
            this.dialogPath = new PathBrowser(config.dialogPrefabFolder, Application.dataPath);
            this.bgPath = new PathBrowser(config.backgroundRootFolder, Application.dataPath);
            this.animPath = new PathBrowser(config.animRootFolder, Application.dataPath);
        }

        LabelHelper.TitleLabel("UIMan Configuration");
        LineHelper.Draw(CommonColor.GetBlue());
        EditorGUILayout.Space();

        GUILayout.BeginVertical("Box");
        config.classNamespace = this.namespaceField.Draw(this.namespaceGUI);
        config.screenPrefabFolder = this.screenPath.Draw(this.screenGUI);
        config.dialogPrefabFolder = this.dialogPath.Draw(this.dialogGUI);
        config.backgroundRootFolder = this.bgPath.Draw(this.bgGUI);
        config.animRootFolder = this.animPath.Draw(this.animRootGUI);
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal("Box");
        GUILayout.Label("<b>Warning:</b> This configuration use to set default path of prefabs/images for UI.\n\n" +
                        "If you don't want to use this default path for your Screen/Dialog, apply UIDescriptor to your class to define custom path.",
                        EditorGUIHelper.RichText(true, CommonColor.GetOrange()));
        GUILayout.EndHorizontal();

        EditorUtility.SetDirty(this.target);
    }
}