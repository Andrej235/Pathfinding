using UnityEditor;
using UnityEngine;

public class TileMapEditorWindow : EditorWindow
{
    [MenuItem("A/Custom tile map")]
    static void ShowWindow()
    {
        var window = (TileMapEditorWindow)GetWindow(typeof(TileMapEditorWindow));
        window.titleContent = new GUIContent("Branko");
    }

    void OnGUI()
    {

    }
}
