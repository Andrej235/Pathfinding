using System;
using System.Collections.Generic;
using UnityEditor;
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

    public double chanceToSpawnAProp;
    public List<PropSOChance> propsChance;

    public bool editor_AreTilesShown;
    public bool editor_ArePropsShown;

    [Serializable]
    public class PropSOChance
    {
        public double chance;
        public PropSO prop;

        public PropSOChance() { }

        public PropSOChance(double chance, PropSO prop)
        {
            this.chance = chance;
            this.prop = prop;
        }
    }
}

[CustomEditor(typeof(DungeonParametersSO))]
public class DungeonParametersSOEditor : Editor
{
    private DungeonParametersSO dungeonParameters;

    private void Awake() => dungeonParameters = (DungeonParametersSO)target;

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        dungeonParameters.editor_AreTilesShown = EditorGUILayout.Foldout(dungeonParameters.editor_AreTilesShown, "Tiles");
        if (dungeonParameters.editor_AreTilesShown)
        {
            DisplayTileBase(ref dungeonParameters.floorTile, "Floor");
            DisplayTileBase(ref dungeonParameters.wallTop, "Top");
            DisplayTileBase(ref dungeonParameters.wallSideRight, "Right");
            DisplayTileBase(ref dungeonParameters.wallSideLeft, "Left");
            DisplayTileBase(ref dungeonParameters.wallBottom, "Bottom");
            DisplayTileBase(ref dungeonParameters.wallFull, "Full");
            DisplayTileBase(ref dungeonParameters.wallInnerCornerDownLeft, "Inner bottom left");
            DisplayTileBase(ref dungeonParameters.wallInnerCornerDownRight, "Inner bottom right");
            DisplayTileBase(ref dungeonParameters.wallDiagonalCornerDownRight, "Bottom right");
            DisplayTileBase(ref dungeonParameters.wallDiagonalCornerDownLeft, "Bottom left");
            DisplayTileBase(ref dungeonParameters.wallDiagonalCornerUpRight, "Top right");
            DisplayTileBase(ref dungeonParameters.wallDiagonalCornerUpLeft, "Top left");
        }

        dungeonParameters.editor_ArePropsShown = EditorGUILayout.Foldout(dungeonParameters.editor_ArePropsShown, "Props");
        if (dungeonParameters.editor_ArePropsShown)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Chance to spawn a prop: ", "Chance to spawn a prop on a specific tile\n\nIf the dungeon generator decides to spawn a prop on a tile it will than start going through each prop in a random order and attempting to spawn it"));
            dungeonParameters.chanceToSpawnAProp = EditorGUILayout.DoubleField(dungeonParameters.chanceToSpawnAProp);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            for (int i = 0; i < dungeonParameters.propsChance.Count; i++)
            {
                var propChance = dungeonParameters.propsChance[i];

                GUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("Prop: ", "PropSO reference which will be used to spawn the prop with it's info"), GUILayout.Width(50));
                propChance.prop = (PropSO)EditorGUILayout.ObjectField(propChance.prop, typeof(PropSO), false);

                GUILayout.Label(new GUIContent("Chance: ", "Chance to spawn this specific prop when spawning a prop on a tile"), GUILayout.Width(50));
                propChance.chance = EditorGUILayout.DoubleField(propChance.chance, GUILayout.Width(50));

                GUILayout.Space(3);

                if (GUILayout.Button("-"))
                {
                    dungeonParameters.propsChance.Remove(propChance);
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(5);
            if (GUILayout.Button("+"))
            {
                dungeonParameters.propsChance.Add(new());
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(dungeonParameters);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
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
}