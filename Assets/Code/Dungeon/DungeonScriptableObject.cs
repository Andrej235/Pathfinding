using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Dungeon", menuName = "ScriptableObjects/Dungeon")]
[Serializable]
public class DungeonScriptableObject : ScriptableObject
{
    public string Name { get; set; }
    public List<Room> PossibleRooms { get; set; }

    public int GridWidth;
    public int GridHeight;
    public float GridCellSize;

    public Vector2[,] GridUV00s;
    public Vector2[,] GridUV11s;
    public bool[,] GridIsWalkables;
}

[CustomEditor(typeof(DungeonScriptableObject))]
public class DungeonScriptableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var dungeon = (DungeonScriptableObject)target;

        GUILayout.Label(dungeon.name);
        dungeon.Name = EditorGUILayout.TextField(dungeon.Name);

        dungeon.PossibleRooms ??= new();
        foreach (var room in dungeon.PossibleRooms)
        {
            EditorGUILayout.BeginHorizontal();
            room.Type = (Room.RoomType)EditorGUILayout.EnumFlagsField(room.Type);
            var minusClicked = GUILayout.Button("-");
            if (minusClicked)
            {
                dungeon.PossibleRooms.Remove(room);
            }
            EditorGUILayout.EndHorizontal();
        }

        var plusClicked = GUILayout.Button("+");
        if (plusClicked)
        {
            dungeon.PossibleRooms.Add(new());
        }

        if (GUILayout.Button("Test"))
        {
            dungeon.GridUV00s = new Vector2[10, 10];
        }
    }
}