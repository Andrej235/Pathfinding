using Assets.Code.DungeonGeneration.Models;
using Assets.Code.Utility;
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
    public List<PropChance> propsChance;
    public List<RoomTypeChance> roomTypesChance;

    public bool editor_AreTilesShown;
    public bool editor_ArePropsShown;
    public bool editor_AreRoomsShown;

    [Serializable]
    public class PropChance : IChance<PropSO>
    {
        [SerializeField] private float chance;
        [SerializeField] private PropSO value;

        public PropSO Value
        {
            get => value;
            set => this.value = value;
        }

        public float Chance
        {
            get => chance;
            set => chance = value;
        }
    }

    [Serializable]
    public class RoomTypeChance : IChance<Room.RoomType>
    {
        [SerializeField] private float chance;
        [SerializeField] private Room.RoomType value;

        public Room.RoomType Value
        {
            get => value;
            set => this.value = value;
        }

        public float Chance
        {
            get => chance;
            set => chance = value;
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
            GUILayout.Label(new GUIContent("Chance to spawn a prop: ", "Chance to spawn a prop on a specific tile\n\nIf the dungeon generator decides to spawn a prop on a tile it will than select a random prop and attempt to spawn it"));
            dungeonParameters.chanceToSpawnAProp = EditorGUILayout.DoubleField(dungeonParameters.chanceToSpawnAProp);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            for (int i = 0; i < dungeonParameters.propsChance.Count; i++)
            {
                var propChance = dungeonParameters.propsChance[i];

                GUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("Prop: ", "PropSO reference which will be used to spawn the prop with it's info"), GUILayout.Width(50));
                propChance.Value = (PropSO)EditorGUILayout.ObjectField(propChance.Value, typeof(PropSO), false);

                GUILayout.Label(new GUIContent("Chance: ", "Chance to spawn this specific prop when spawning a prop on a tile"), GUILayout.Width(50));
                propChance.Chance = EditorGUILayout.FloatField(propChance.Chance, GUILayout.Width(50));

                GUILayout.Space(3);

                if (GUILayout.Button("-"))
                    dungeonParameters.propsChance.Remove(propChance);
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(5);
            if (GUILayout.Button("+"))
                dungeonParameters.propsChance.Add(new());
        }

        dungeonParameters.editor_AreRoomsShown = EditorGUILayout.Foldout(dungeonParameters.editor_AreRoomsShown, "Rooms");
        if (dungeonParameters.editor_AreRoomsShown)
        {
            for (int i = 0; i < dungeonParameters.roomTypesChance.Count; i++)
            {
                var roomTypeChance = dungeonParameters.roomTypesChance[i];

                GUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("Room: "), GUILayout.Width(50));
                roomTypeChance.Value = (Room.RoomType)EditorGUILayout.EnumFlagsField(roomTypeChance.Value);

                GUILayout.Label(new GUIContent("Chance: ", "Chance of a room with this particular room type spawning"), GUILayout.Width(50));
                roomTypeChance.Chance = EditorGUILayout.FloatField(roomTypeChance.Chance, GUILayout.Width(50));

                GUILayout.Space(3);

                if (GUILayout.Button("-"))
                    dungeonParameters.roomTypesChance.Remove(roomTypeChance);
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(5);
            if (GUILayout.Button("+"))
                dungeonParameters.roomTypesChance.Add(new());
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