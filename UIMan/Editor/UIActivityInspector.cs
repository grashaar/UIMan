using UnityEditor;
using UnityEngine;
using UnuGames;

[CustomEditor(typeof(UIActivityIndicator))]
public class UIActivityInspector : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal("Box");
        LabelHelper.HeaderLabel("UIMan Activity Indicator");
        GUILayout.EndHorizontal();

        GUILayout.BeginVertical("Box");
        DrawDefaultInspector();
        GUILayout.EndVertical();
    }
}