using UnityEditor;
using UnityEngine;

public class CustomSceneView : SceneView
{
    Editor tileMapEditor = null;

    [MenuItem("A/Custom tile map scene view")]
    static void ShowWindow()
    {
        var window = (CustomSceneView)GetWindow(typeof(CustomSceneView));
        window.titleContent = new GUIContent("MyScene");
    }

    protected override void OnSceneGUI()
    {
        base.OnSceneGUI();

        #region <Grid>
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical(GUILayout.Width(100));
        #endregion

        GUILayout.Label("My label");
        TileMapEditor();

        #region <Grid />
        GUILayout.EndVertical();
        GUILayout.Space(15);
        GUILayout.EndHorizontal();
        #endregion

        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        switch (Event.current.GetTypeForControl(controlID))
        {
            case EventType.MouseDown:
                Debug.Log("MouseDown");
                break;
        }
    }

    private void TileMapEditor()
    {
        tileMapEditor = EditorGUILayout.ObjectField(tileMapEditor, typeof(Editor), true) as Editor;
        if (tileMapEditor == null)
            return;

        EditorGUI.BeginChangeCheck();
        tileMapEditor.width = EditorGUILayout.IntField(tileMapEditor.width);
        tileMapEditor.height = EditorGUILayout.IntField(tileMapEditor.height);
        tileMapEditor.cellSize = EditorGUILayout.IntField(tileMapEditor.cellSize);
        if (EditorGUI.EndChangeCheck())
        {
            //Update tileMapEditor.Grid, call a function which does the same as OnValidate
            Debug.Log("C");
        }
    }
}
