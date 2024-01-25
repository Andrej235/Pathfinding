using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Dungeon", menuName = "ScriptableObjects/Dungeon")]
public class DungeonScriptableObject : ScriptableObject
{
    public string Name;
    public List<Room> PossibleRooms;
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
    }
}