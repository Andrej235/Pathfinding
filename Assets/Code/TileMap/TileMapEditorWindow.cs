using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TileMapEditorWindow : EditorWindow
{
    [MenuItem("A/Custom tile map")]
    static void ShowWindow()
    {
        var window = (TileMapEditorWindow)GetWindow(typeof(TileMapEditorWindow));
        window.titleContent = new GUIContent("Branko");

        SceneView.beforeSceneGui += SceneView_beforeSceneGui;
        SceneView.duringSceneGui += SceneView_duringSceneGui;
    }

    private static void SceneView_duringSceneGui(SceneView obj)
    {
        var e = Event.current;
        if (e.isMouse)
        {
            if (e.keyCode == KeyCode.Mouse0)
            {
                Debug.Log("Click during");
            }
        }
    }

    private static void SceneView_beforeSceneGui(SceneView obj)
    {
        var e = Event.current;
        if (e.isMouse)
        {
            if (e.keyCode == KeyCode.Mouse0)
            {
                Debug.Log("Click");
            }
        }
    }

    Editor tileMapEditor = null;

    void OnGUI()
    {
        GUILayout.Label("Branko je gay");
        tileMapEditor = EditorGUILayout.ObjectField(tileMapEditor, typeof(Editor), true) as Editor;

        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        switch (Event.current.GetTypeForControl(controlID))
        {
            case EventType.MouseDown:
                GUIUtility.hotControl = controlID;
                Debug.Log("MouseDown");
                Event.current.Use();
                break;

            case EventType.MouseUp:
                GUIUtility.hotControl = 0;
                Event.current.Use();
                break;
        }
    }
}
