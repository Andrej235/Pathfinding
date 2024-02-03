using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "DungeonParameters", menuName = "Scriptable objects/Dungeon parameters")]
public class DungeonParametersSO : ScriptableObject
{
    public TileBase floorTile;
    public TileBase wallTop;
    public TileBase wallSideRight;
    public TileBase wallSideLeft;
    public TileBase wallBottom;
    public TileBase wallFull;
    public TileBase wallInnerCornerDownLeft;
    public TileBase wallInnerCornerDownRight;
    public TileBase wallDiagonalCornerDownRight;
    public TileBase wallDiagonalCornerDownLeft;
    public TileBase wallDiagonalCornerUpRight;
    public TileBase wallDiagonalCornerUpLeft;

    //Derive 2 classes from this one: CorridorFirstDungeonParametersSO and RoomFirstDungeonParametersSO
    public RandomWalkParametersSO randomWalkParameters;

    //public bool editor_AreTileFieldsShown;
}

//[CustomEditor(typeof(DungeonParametersSO))]
/*public class DungeonParametersSOEditor : Editor
{
    private DungeonParametersSO dungeonParameters;

    private void Awake() => dungeonParameters = (DungeonParametersSO)target;

    public override void OnInspectorGUI()
    {
        dungeonParameters.editor_AreTileFieldsShown = EditorGUILayout.Foldout(dungeonParameters.editor_AreTileFieldsShown, "Tiles");
        if (dungeonParameters.editor_AreTileFieldsShown)
        {
            DisplayTileBase(ref dungeonParameters.floorTile, "Floor");
            DisplayTileBase(ref dungeonParameters.wallTop, "Top");
            DisplayTileBase(ref dungeonParameters.wallSideRight, "Right");
            DisplayTileBase(ref dungeonParameters.wallSideLeft, "Left");
            DisplayTileBase(ref dungeonParameters.wallBottom, "Bottom");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Bottom", GUILayout.Width(150));
            dungeonParameters.wallBottom = (TileBase)EditorGUILayout.ObjectField(dungeonParameters.wallBottom, typeof(TileBase), false);
            GUILayout.EndHorizontal();
            DisplayTileBase(ref dungeonParameters.wallFull, "Full");
            DisplayTileBase(ref dungeonParameters.wallInnerCornerDownLeft, "Inner bottom left");
            DisplayTileBase(ref dungeonParameters.wallInnerCornerDownRight, "Inner bottom right");
            DisplayTileBase(ref dungeonParameters.wallDiagonalCornerDownRight, "Bottom right");
            DisplayTileBase(ref dungeonParameters.wallDiagonalCornerDownLeft, "Bottom left");
            DisplayTileBase(ref dungeonParameters.wallDiagonalCornerUpRight, "Top right");
            DisplayTileBase(ref dungeonParameters.wallDiagonalCornerUpLeft, "Top left");
        }

        base.OnInspectorGUI();
    }

    public void DisplayTileBase(ref TileBase tileBase, string name)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label($"{name}:", GUILayout.Width(150));
        tileBase = (TileBase)EditorGUILayout.ObjectField(tileBase, typeof(TileBase), false);
        GUILayout.EndHorizontal();
    }

    public void DisplayTilemap(ref Tilemap tilemap, string name)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label($"{name}:", GUILayout.Width(150));
        tilemap = (Tilemap)EditorGUILayout.ObjectField(tilemap, typeof(Tilemap), true);
        GUILayout.EndHorizontal();
    }
}*/