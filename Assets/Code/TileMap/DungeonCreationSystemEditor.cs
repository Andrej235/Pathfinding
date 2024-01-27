using Assets.Code.Dungeon.DTOs;
using Assets.Code.Utility;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.UI;
using File = System.IO.File;

namespace Assets.Code.TileMap
{
    [CustomEditor(typeof(DungeonCreationSystem))]
    public class DungeonCreationSystemEditor : Editor
    {
        private const string DEFAULT_NEW_DUNGEON_NAME = "My Dungeon";

        private TileMapDrawingSystem tileMapDrawingSystem;

        private RoomDTO selectedRoom;
        private int selectedRoomIndex = -1;

        private bool pressedDungeonDelete;
        private bool confirmedDungeonDelete;

        private string selectedDungeonFilePath;
        private string selectedDungeonFileName;

        private bool dungeonHasUnsavedChanges;

        public override void OnInspectorGUI()
        {
            //Only allow the user to edit/create dungeons if they are in play mode (if the game is running), this is necessary because certain Unity functions don't work in edit mode
            if (!Application.isPlaying)
            {
                GUILayout.Label("Unable to edit dungeons in edit mode");
                return;
            }

            if (target is not DungeonCreationSystem system)
                return;

            #region Dungeon selection / initialization
            if (tileMapDrawingSystem == null)
            {
                system.InitializeTileMapDrawingSystem();
                tileMapDrawingSystem = system.TileMapDrawingSystem;
            }

            if (string.IsNullOrWhiteSpace(selectedDungeonFilePath))
            {
                GUILayout.Label("Choose a dungeon to edit: ");
                foreach (var dungeonName in DungeonCreationSystem.GetDungeonNames())
                {
                    bool currentDungeonSelected = GUILayout.Button(dungeonName);
                    if (!currentDungeonSelected)
                        continue;

                    //File with name {dungeonName} HAS to exist because it has already been read by the function DungeonCreationSystem.GetDungeonNames()
                    SelectDungeon(system, dungeonName);
                }

                //Creates a new dungeon which isn't saved anywhere until the user clicks the button 'Save'
                if (GUILayout.Button("+"))
                {
                    selectedDungeonFilePath = DungeonCreationSystem.DungeonsJSONFolderPath;
                    system.dto = new(DEFAULT_NEW_DUNGEON_NAME, "", new());
                    system.InitializeTileMapDrawingSystem();
                    tileMapDrawingSystem = system.TileMapDrawingSystem;
                }

                return;
            }
            #endregion

            GUILayout.Space(10);
            EditorGUI.BeginChangeCheck(); //******************************************************************************************************************************************************************************************************************
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

                if (i != selectedRoomIndex && GUILayout.Button("Select"))
                {
                    SelectRoom(i, room);
                }

                if (GUILayout.Button("-", GUILayout.Width(33)))
                {
                    if (i == selectedRoomIndex)
                        DeselectRoom();
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
            if (selectedRoomIndex >= 0)
            {
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
            }
            #endregion

            GUILayout.Space(10);

            if (EditorGUI.EndChangeCheck()) //******************************************************************************************************************************************************************************************
                dungeonHasUnsavedChanges = true;


            #region Save / Desect / Delete buttons
            //If the dungeon doesn't have a name or has the default name save and delete buttons won't be displayed
            if (!string.IsNullOrWhiteSpace(system.dto.Name) && system.dto.Name != DEFAULT_NEW_DUNGEON_NAME)
                if (GUILayout.Button("Save"))
                {
                    try
                    {
                        if (selectedDungeonFileName != system.dto.Name)
                        {
                            if (File.Exists(selectedDungeonFilePath))
                                File.Delete(selectedDungeonFilePath);

                            selectedDungeonFilePath = DungeonCreationSystem.DungeonsJSONFolderPath + DungeonCreationSystem.ConvertNameToFileName(system.dto.Name);
                            selectedDungeonFileName = system.dto.Name;
                        }

                        if (selectedRoom != null)
                            selectedRoom.TileMap = tileMapDrawingSystem.GetDTO();

                        system.dto.MaterialName = tileMapDrawingSystem.BaseMaterial != null ? tileMapDrawingSystem.BaseMaterial.name : "";
                        system.dto.PossibleRooms ??= new();
                        string gridJsonFile = JsonUtility.ToJson(system.dto);

                        File.WriteAllText(selectedDungeonFilePath, gridJsonFile);
                        dungeonHasUnsavedChanges = false;
                    }
                    catch (Exception ex)
                    {
                        ex.LogError();
                    }
                }

            if (GUILayout.Button(dungeonHasUnsavedChanges ? "Discard changes" : "Deselect"))
                DeselectDungeon(system);

            if (!string.IsNullOrWhiteSpace(system.dto?.Name) && system.dto?.Name != DEFAULT_NEW_DUNGEON_NAME)
                if (!pressedDungeonDelete && GUILayout.Button("Delete"))
                    pressedDungeonDelete = true;

            if (pressedDungeonDelete)
            {
                GUILayout.Label("Are you sure you want to permanently delete this dungeon?");
                GUILayout.BeginHorizontal();
                pressedDungeonDelete = !GUILayout.Button("No");
                confirmedDungeonDelete = GUILayout.Button("Yes");
                GUILayout.EndHorizontal();
            }

            if (pressedDungeonDelete && confirmedDungeonDelete)
            {
                if (File.Exists(selectedDungeonFilePath))
                    File.Delete(selectedDungeonFilePath);

                DeselectDungeon(system);
            }
            #endregion

            SaveChanges();
        }

        private void SelectDungeon(DungeonCreationSystem system, string dungeonName)
        {
            selectedDungeonFilePath = DungeonCreationSystem.DungeonsJSONFolderPath + DungeonCreationSystem.ConvertNameToFileName(dungeonName);
            selectedDungeonFileName = dungeonName;
            string dungeonJsonFileContent = File.ReadAllText(selectedDungeonFilePath);

            system.dto = JsonUtility.FromJson<DungeonDTO>(dungeonJsonFileContent) ?? new(DEFAULT_NEW_DUNGEON_NAME, "", new());
            tileMapDrawingSystem.BaseMaterial = Resources.Load<Material>($"Materials/{system.dto.MaterialName}");
        }

        private void DeselectDungeon(DungeonCreationSystem system)
        {
            pressedDungeonDelete = false;
            confirmedDungeonDelete = false;

            selectedDungeonFilePath = "";
            selectedDungeonFileName = "";

            system.dto = null;
            tileMapDrawingSystem.BaseMaterial = null;

            DeselectRoom();
        }

        private void SelectRoom(int roomIndex, RoomDTO room)
        {
            if (selectedRoom != null)
                selectedRoom.TileMap = tileMapDrawingSystem.GetDTO();

            selectedRoomIndex = roomIndex;
            room.TileMap ??= new(0, 0, 0, new List<Vector2>(), new(), new());
            selectedRoom = room;

            tileMapDrawingSystem.LoadDTO(room.TileMap);
        }

        private void DeselectRoom()
        {
            selectedRoomIndex = -1;
            selectedRoom = null;
            tileMapDrawingSystem.UnloadMesh();
        }
    }
}