using CodeMonkey.Utils;
using UnityEditor;
using UnityEngine;

#nullable enable
public class CustomSceneView : SceneView
{
    private TileMapVisual? tileMapVisual = null;

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
        if (Event.current.GetTypeForControl(controlID) == EventType.MouseDown)
        {
            Debug.Log("MouseDown" + Camera.main.ScreenToWorldPoint(Event.current.mousePosition));
            if (tileMapVisual == null || tileMapVisual.Grid is null)
                return;

            var mouseScreenPos = Event.current.mousePosition;
            var pos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

            var (x, y) = tileMapVisual.Grid.GetXY(pos);
            tileMapVisual.Grid[x, y].isWalkable = !tileMapVisual.Grid[x, y].isWalkable;
            tileMapVisual.RegenerateMesh();
        }
    }

    private void TileMapEditor()
    {
        EditorGUI.BeginChangeCheck();
        tileMapVisual = EditorGUILayout.ObjectField(tileMapVisual, typeof(TileMapVisual), true) as TileMapVisual;
        if (EditorGUI.EndChangeCheck() && tileMapVisual != null)
            tileMapVisual.GenerateMesh();

        if (tileMapVisual == null)
            return;

        EditorGUI.BeginChangeCheck();
        tileMapVisual.Width = EditorGUILayout.IntField(tileMapVisual.Width);
        tileMapVisual.Height = EditorGUILayout.IntField(tileMapVisual.Height);
        tileMapVisual.CellSize = EditorGUILayout.FloatField(tileMapVisual.CellSize);
        tileMapVisual.ShowGrid = EditorGUILayout.Toggle(tileMapVisual.ShowGrid);
        if (EditorGUI.EndChangeCheck())
        {
            //Update tileMapEditor.Grid, call a function which does the same as OnValidate
            Debug.Log("C");
            tileMapVisual.RegenerateMesh();
        }
    }
}
