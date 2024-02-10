using Assets.Code.DungeonGeneration.Models;
using Assets.Code.Utility;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static DungeonParametersSO;

[CustomEditor(typeof(DungeonParametersSO))]
public class DungeonParametersSOEditor : Editor
{
    private DungeonParametersSO dungeonParameters;

    private void Awake()
    {
        dungeonParameters = (DungeonParametersSO)target;
        dungeonParameters.propsChance ??= new();
        dungeonParameters.roomParameters ??= new();
        dungeonParameters.enemiesChance ??= new();
    }

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

            DisplayPropChanceList(dungeonParameters.propsChance);
        }

        dungeonParameters.editor_AreEnemiesShown = EditorGUILayout.Foldout(dungeonParameters.editor_AreEnemiesShown, "Enemies");
        if (dungeonParameters.editor_AreEnemiesShown)
        {
            DisplayEnemyChanceList(dungeonParameters.enemiesChance);
            GUILayout.Space(10);
        }

        dungeonParameters.editor_AreRoomsShown = EditorGUILayout.Foldout(dungeonParameters.editor_AreRoomsShown, "Rooms");
        if (dungeonParameters.editor_AreRoomsShown)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginVertical();

            for (int i = 0; i < dungeonParameters.roomParameters.Count; i++)
            {
                var roomTypeChance = dungeonParameters.roomParameters[i];
                var roomName = roomTypeChance.Type.ToFlagString(new List<Room.RoomType> { Room.RoomType.None, Room.RoomType.Everything });
                roomName = string.IsNullOrWhiteSpace(roomName) ? "New room" : roomName;

                roomTypeChance.editor_IsNotCollapsed = EditorGUILayout.Foldout(roomTypeChance.editor_IsNotCollapsed, roomName);

                if (!roomTypeChance.editor_IsNotCollapsed)
                    continue;

                GUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("Room: "), GUILayout.Width(50));
                roomTypeChance.Type = (Room.RoomType)EditorGUILayout.EnumFlagsField(roomTypeChance.Type);

                GUILayout.Label(new GUIContent("Chance: ", "Chance of a room with this particular room type spawning"), GUILayout.Width(50));
                roomTypeChance.Chance = EditorGUILayout.FloatField(roomTypeChance.Chance, GUILayout.Width(50));

                GUILayout.Space(3);

                if (GUILayout.Button("-"))
                    dungeonParameters.roomParameters.Remove(roomTypeChance);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Enemies: ");
                roomTypeChance.MinimumNumberOfEnemies = EditorGUILayout.IntField(roomTypeChance.MinimumNumberOfEnemies);
                GUILayout.Label(" - ");
                roomTypeChance.MaximumNumberOfEnemies = EditorGUILayout.IntField(roomTypeChance.MaximumNumberOfEnemies);
                GUILayout.EndHorizontal();

                roomTypeChance.editor_ArePropsShown = EditorGUILayout.Foldout(roomTypeChance.editor_ArePropsShown, "Specific room props: ");
                if (roomTypeChance.editor_ArePropsShown)
                    DisplayPropChanceList(roomTypeChance.SpecificRoomPropsChance);

                roomTypeChance.editor_AreEnemiesShown = EditorGUILayout.Foldout(roomTypeChance.editor_AreEnemiesShown, "Specific room enemies: ");
                if (roomTypeChance.editor_AreEnemiesShown)
                    DisplayEnemyChanceList(roomTypeChance.SpecificRoomEnemies, false);

                GUILayout.Space(20);
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            if (GUILayout.Button("+"))
                dungeonParameters.roomParameters.Add(new());
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(dungeonParameters);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    private void DisplayPropChanceList(List<PropChance> propsChance)
    {
        for (int i = 0; i < propsChance.Count; i++)
        {
            var propChance = propsChance[i];

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Prop: ", "PropSO reference which will be used to spawn the prop with it's info"), GUILayout.Width(50));
            propChance.Value = (PropSO)EditorGUILayout.ObjectField(propChance.Value, typeof(PropSO), false);

            GUILayout.Label(new GUIContent("Chance: ", "Chance to spawn this specific prop when spawning a prop on a tile"), GUILayout.Width(50));
            propChance.Chance = EditorGUILayout.FloatField(propChance.Chance, GUILayout.Width(50));

            GUILayout.Space(3);

            if (GUILayout.Button("-"))
                propsChance.Remove(propChance);
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(5);
        if (GUILayout.Button("+"))
            propsChance.Add(new());
    }

    private void DisplayEnemyChanceList(List<EnemyChance> enemiesChance, bool includeRoomType = true)
    {
        for (int i = 0; i < enemiesChance.Count; i++)
        {
            var enemyChance = enemiesChance[i];

            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Enemy: ", ""), GUILayout.Width(50));
            enemyChance.Value = (GameObject)EditorGUILayout.ObjectField(enemyChance.Value, typeof(GameObject), false);

            GUILayout.Label(new GUIContent("Chance: ", ""), GUILayout.Width(50));
            enemyChance.Chance = EditorGUILayout.FloatField(enemyChance.Chance, GUILayout.Width(50));

            GUILayout.Space(3);

            if (GUILayout.Button("-"))
                enemiesChance.Remove(enemyChance);
            GUILayout.EndHorizontal();

            if (includeRoomType)
            {
                GUILayout.Space(2);

                GUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("Room type: ", "Given enemy can only spawn in rooms with the following type"), GUILayout.Width(75));
                enemyChance.RoomType = (Room.RoomType)EditorGUILayout.EnumPopup(enemyChance.RoomType);
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(5);
        }

        GUILayout.Space(5);
        if (GUILayout.Button("+"))
            enemiesChance.Add(new());
    }

    private void DisplayTileBase(ref TileBase tileBase, string name)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label($"{name}:", GUILayout.Width(150));
        tileBase = (TileBase)EditorGUILayout.ObjectField(tileBase, typeof(TileBase), false);
        GUILayout.EndHorizontal();
    }
}