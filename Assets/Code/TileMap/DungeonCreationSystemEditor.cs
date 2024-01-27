using Assets.Code.Dungeon.DTOs;
using Assets.Code.Grid;
using Assets.Code.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Directory = System.IO.Directory;
using File = System.IO.File;

namespace Assets.Code.TileMap
{
    [CustomEditor(typeof(DungeonCreationSystem))]
    public class DungeonCreationSystemEditor : Editor
    {
        private RoomDTO selectedRoom;
        private TileMapDrawingSystem tileMapDrawingSystem;

        public override void OnInspectorGUI()
        {
            if (target is not DungeonCreationSystem system)
                return;

            tileMapDrawingSystem = (TileMapDrawingSystem)EditorGUILayout.ObjectField(tileMapDrawingSystem, typeof(TileMapDrawingSystem), true);

            if (tileMapDrawingSystem == null)
                return;

            #region Materials
            GUILayout.BeginHorizontal();
            GUILayout.Label("Base", GUILayout.Width(150));
            GUILayout.FlexibleSpace();
            GUILayout.Label("Collider Display", GUILayout.Width(150));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            tileMapDrawingSystem.baseMaterial = (Material)EditorGUILayout.ObjectField(tileMapDrawingSystem.baseMaterial, typeof(Material), false, GUILayout.Width(150));
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

            #region Grid
            EditorGUI.BeginChangeCheck();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Width: ");
            tileMapDrawingSystem.width = EditorGUILayout.IntSlider(tileMapDrawingSystem.width, 0, 100);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Height: ");
            tileMapDrawingSystem.height = EditorGUILayout.IntSlider(tileMapDrawingSystem.height, 0, 100);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Cell size: ");
            tileMapDrawingSystem.cellSize = EditorGUILayout.Slider(tileMapDrawingSystem.cellSize, 0, 15);
            GUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
                tileMapDrawingSystem.CreateGrid();
            #endregion

            GUILayout.Space(10);

            #region Save, load and new grid
            if (GUILayout.Button("Load"))
            {
                try
                {
                    string gridJsonFile = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), @"Assets\JSONFiles\asdasd.json"));
                    TileMapGridDTO gridData = JsonUtility.FromJson<TileMapGridDTO>(gridJsonFile);

                    tileMapDrawingSystem.width = gridData.GridWidth;
                    tileMapDrawingSystem.height = gridData.GridHeight;
                    tileMapDrawingSystem.cellSize = gridData.GridCellSize;

                    tileMapDrawingSystem.CreateGrid();

                    Grid<TileMapNode> Grid = new(tileMapDrawingSystem.width, tileMapDrawingSystem.height, tileMapDrawingSystem.cellSize, (g, x, y) => new(x, y, Vector2.zero, Vector2.zero, true));
                    tileMapDrawingSystem.SetGrid(Grid);

                    for (int i = 0; i < tileMapDrawingSystem.width; i++)
                    {
                        for (int j = 0; j < tileMapDrawingSystem.height; j++)
                        {
                            tileMapDrawingSystem.Grid[i, j].UV00 = gridData.GridUV00s[i * tileMapDrawingSystem.height + j];
                            tileMapDrawingSystem.Grid[i, j].UV11 = gridData.GridUV11s[i * tileMapDrawingSystem.height + j];

                            tileMapDrawingSystem.Grid[i, j].isWalkable = gridData.GridIsWalkables[i * tileMapDrawingSystem.height + j];
                        }
                    }

                    tileMapDrawingSystem.GenerateMesh();
                    tileMapDrawingSystem.GenerateColliders();
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
                    List<List<Vector2>> uv00s = new();
                    List<List<Vector2>> uv11s = new();

                    List<List<bool>> walkable = new();


                    for (int i = 0; i < tileMapDrawingSystem.width; i++)
                    {
                        uv00s.Add(new());
                        uv11s.Add(new());
                        walkable.Add(new());

                        for (int j = 0; j < tileMapDrawingSystem.height; j++)
                        {
                            uv00s[i].Add(tileMapDrawingSystem.Grid[i, j].UV00);
                            uv11s[i].Add(tileMapDrawingSystem.Grid[i, j].UV11);

                            walkable[i].Add(tileMapDrawingSystem.Grid[i, j].isWalkable);
                        }
                    }

                    TileMapGridDTO tileMapGridDTO = new(
                        gridWidth: tileMapDrawingSystem.width,
                        gridHeight: tileMapDrawingSystem.height,
                        gridCellSize: tileMapDrawingSystem.cellSize,
                        gridUV00s: uv00s,
                        gridUV11s: uv11s,
                        gridIsWalkables: walkable);

                    string gridJsonFile = JsonUtility.ToJson(tileMapGridDTO);
                    File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), @"Assets\JSONFiles\asdasd.json"), gridJsonFile);
                }
                catch (Exception ex)
                {
                    ex.LogError();
                }
            }

            if (GUILayout.Button("Create a new grid"))
                tileMapDrawingSystem.SetGrid(new(tileMapDrawingSystem.width, tileMapDrawingSystem.height, tileMapDrawingSystem.cellSize, (g, x, y) => new(x, y, Vector2.zero, Vector2.zero, true)));
            SaveChanges();
            #endregion
        }
    }
}