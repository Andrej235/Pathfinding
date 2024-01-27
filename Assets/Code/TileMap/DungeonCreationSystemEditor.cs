using Assets.Code.Dungeon.DTOs;
using Assets.Code.Utility;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using File = System.IO.File;

namespace Assets.Code.TileMap
{
    [CustomEditor(typeof(DungeonCreationSystem))]
    public class DungeonCreationSystemEditor : Editor
    {
        private RoomDTO selectedRoom;
        private TileMapDrawingSystem tileMapDrawingSystem;
        private int selectedRoomIndex = -1;

        public override void OnInspectorGUI()
        {
            if (!Application.isPlaying)
            {
                GUILayout.Label("Unable to edit dungeons in edit mode");
                return;
            }

            if (target is not DungeonCreationSystem system)
                return;

            if (tileMapDrawingSystem == null)
            {
                foreach (var dungeonName in DungeonCreationSystem.GetDungeonNames())
                {
                    if (GUILayout.Button(dungeonName))
                    {
                        try
                        {
                            string dungeonFileName = DungeonCreationSystem.DungeonsJSONFolderPath + DungeonCreationSystem.ConvertNameToFileName(dungeonName);
                            string dungeonJsonFileContent = File.ReadAllText(dungeonFileName);
                            system.dto = JsonUtility.FromJson<DungeonDTO>(dungeonJsonFileContent) ?? new("", 0, new());

                            system.dto.Name ??= "My dungeon";
                            system.dto.PossibleRooms ??= new();

                            system.InitializeTileMapDrawingSystem();
                            tileMapDrawingSystem = system.TileMapDrawingSystem;

                            //TODO: Set baseMaterial equal to the saved material (by id??)
                            //baseMaterial = tileMapDrawingSystem.BaseMaterial;
                        }
                        catch (Exception ex)
                        {
                            ex.LogError();
                        }
                    }
                }

                return;
            }

            #region Materials
            GUILayout.BeginHorizontal();
            GUILayout.Label("Base", GUILayout.Width(150));
            GUILayout.FlexibleSpace();
            GUILayout.Label("Collider Display", GUILayout.Width(150));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            tileMapDrawingSystem.BaseMaterial = (Material)EditorGUILayout.ObjectField(tileMapDrawingSystem.BaseMaterial, typeof(Material), false, GUILayout.Width(150));
            GUILayout.FlexibleSpace();
            tileMapDrawingSystem.colliderDisplayMaterial = (Material)EditorGUILayout.ObjectField(tileMapDrawingSystem.colliderDisplayMaterial, typeof(Material), false, GUILayout.Width(150));
            GUILayout.EndHorizontal();
            #endregion

            GUILayout.Space(10);

            #region Image and Toggle
            GUILayout.BeginHorizontal();
            GUILayout.Label("Base image", GUILayout.Width(150));
            GUILayout.FlexibleSpace();
            GUILayout.Label("Collider toggle", GUILayout.Width(150));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            tileMapDrawingSystem.baseImage = (GameObject)EditorGUILayout.ObjectField(tileMapDrawingSystem.baseImage, typeof(GameObject), true, GUILayout.Width(150));
            GUILayout.FlexibleSpace();
            tileMapDrawingSystem.colliderToggle = (Toggle)EditorGUILayout.ObjectField(tileMapDrawingSystem.colliderToggle, typeof(Toggle), true, GUILayout.Width(150));
            GUILayout.EndHorizontal();
            #endregion

            GUILayout.Space(10);

            #region Dungeon
            system.dto.Name = EditorGUILayout.TextField("Dungeon name: ", system.dto.Name);

            GUILayout.Label("Rooms: ");

            for (int i = 0; i < system.dto.PossibleRooms.Count; i++)
            {
                RoomDTO room = system.dto.PossibleRooms[i];

                GUILayout.BeginHorizontal();
                if (i == selectedRoomIndex)
                    GUILayout.Label("---> ", GUILayout.Width(33));

                room.Type = (RoomDTO.RoomType)EditorGUILayout.EnumFlagsField(room.Type);

                if (i != selectedRoomIndex)
                {
                    if (GUILayout.Button("Select"))
                    {
                        if (selectedRoom != null)
                            selectedRoom.TileMap = tileMapDrawingSystem.GetDTO();

                        selectedRoomIndex = i;
                        room.TileMap ??= new(0, 0, 0, new List<Vector2>(), new(), new());
                        selectedRoom = room;

                        tileMapDrawingSystem.LoadDTO(room.TileMap);
                    }
                }

                if (GUILayout.Button("-", GUILayout.Width(33)))
                {
                    if (i == selectedRoomIndex)
                    {
                        selectedRoomIndex = -1;
                        selectedRoom = null;
                        //TODO: Somehow hide the tilemap
                    }
                    else if (i < selectedRoomIndex)
                        selectedRoomIndex--;

                    system.dto.PossibleRooms.Remove(room);
                }

                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+"))
                system.dto.PossibleRooms.Add(new RoomDTO());

            #endregion

            GUILayout.Space(10);

            #region Grid
            EditorGUI.BeginChangeCheck();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Width: ");
            tileMapDrawingSystem.Width = EditorGUILayout.IntSlider(tileMapDrawingSystem.Width, 0, 100);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Height: ");
            tileMapDrawingSystem.Height = EditorGUILayout.IntSlider(tileMapDrawingSystem.Height, 0, 100);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Cell size: ");
            tileMapDrawingSystem.CellSize = EditorGUILayout.Slider(tileMapDrawingSystem.CellSize, 0, 15);
            GUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
                tileMapDrawingSystem.RecreateGrid();
            #endregion

            GUILayout.Space(10);

            if (string.IsNullOrWhiteSpace(system.dto.Name))
                return;

            #region Save, load and new grid
            if (GUILayout.Button("Load"))
            {
                try
                {
                    string gridJsonFile = File.ReadAllText(DungeonCreationSystem.DungeonsJSONFolderPath);
                    system.dto = JsonUtility.FromJson<DungeonDTO>(gridJsonFile);
                }
                catch (Exception ex)
                {
                    ex.LogError();
                }
            }

            if (GUILayout.Button("Save"))
            {
                try
                {
                    var filePath = DungeonCreationSystem.DungeonsJSONFolderPath + $"\\DungeonPreset_{system.dto.Name}.json";

                    if (selectedRoom != null)
                        selectedRoom.TileMap = tileMapDrawingSystem.GetDTO();

                    string gridJsonFile = JsonUtility.ToJson(system.dto);

                    /*                    if (!File.Exists(filePath))
                                            File.Create(filePath);*/

                    File.WriteAllText(filePath, gridJsonFile);
                }
                catch (Exception ex)
                {
                    ex.LogError();
                }
            }

            SaveChanges();
            #endregion
        }
    }
}