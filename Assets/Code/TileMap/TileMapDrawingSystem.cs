using Assets.Code.Grid;
using CodeMonkey.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using Directory = System.IO.Directory;
using File = System.IO.File;
using Input = UnityEngine.Input;

namespace Assets.Code.TileMap
{
    public class TileMapDrawingSystem : MonoBehaviour
    {
        public Material baseMaterial;
        public Material colliderDisplayMaterial;
        public GameObject baseImage;
        public Toggle colliderToggle;
        public int width;
        public int height;
        public float cellSize;

        private MeshFilter tileMapMeshFilter;
        private MeshFilter colliderMeshFilter;

        private Mesh mesh;
        private Mesh colliderMesh;

        public Grid<TileMapNode> Grid { get; private set; }
        private Vector2 selectedUVValue = new();
        private bool isColliderToggleChecked;
        private bool areMeshesLoaded = false;

        private void Start()
        {
            GameObject tileMapMeshFilterHolder = new() { name = "TileMapMeshFilterHolder" };
            var tileMapMeshRenderer = tileMapMeshFilterHolder.AddComponent<MeshRenderer>();
            tileMapMeshRenderer.material = baseMaterial;
            tileMapMeshFilter = tileMapMeshFilterHolder.AddComponent<MeshFilter>();

            GameObject colliderMeshFilterHolder = new() { name = "ColliderMeshFilterHolder" };
            colliderMeshFilterHolder.transform.position = colliderMeshFilterHolder.transform.position + new Vector3(0, 0, -1);
            var colliderMeshRenderer = colliderMeshFilterHolder.AddComponent<MeshRenderer>();
            colliderMeshRenderer.material = colliderDisplayMaterial;
            colliderMeshFilter = colliderMeshFilterHolder.AddComponent<MeshFilter>();

            colliderToggle.onValueChanged.AddListener(ColliderToggleValueChanged);

            areMeshesLoaded = true;
            CreateGrid();
            IntializeTileMapButtons();
        }

        private void OnValidate()
        {
            if (Application.isPlaying && areMeshesLoaded)
                CreateGrid();
        }

        public void CreateGrid()
        {
            if (Grid != null)
            {
                if (Grid.Height != height || Grid.Width != width)
                {
                    var newGrid = new Grid<TileMapNode>(width, height, cellSize, (g, x, y) => new(x, y, Vector2.zero, Vector2.zero, true));
                    var minWidth = Math.Min(Grid.Width, width);
                    var minHeight = Math.Min(Grid.Height, height);

                    for (int i = 1; i < minWidth - 1; i++)
                    {
                        for (int j = 1; j < minHeight - 1; j++)
                        {
                            newGrid[i, j].UV00 = Grid[i, j].UV00;
                            newGrid[i, j].UV11 = Grid[i, j].UV11;
                            newGrid[i, j].isWalkable = Grid[i, j].isWalkable;
                        }
                    }

                    Grid = newGrid;
                    Grid.CreateOuterWalls();
                    GenerateMesh();
                    GenerateColliders();
                }

                if (Grid.CellSize != cellSize)
                {
                    Grid.CellSize = cellSize;
                    GenerateMesh();
                    GenerateColliders();
                }
                return;
            }

            /*            Grid = new(width, height, cellSize, (g, x, y) => new(x, y, Vector2.zero, Vector2.zero, true));
                        Grid.CreateOuterWalls();

                        GenerateMesh();
                        GenerateColliders();*/
        }

        public void SetGrid(Grid<TileMapNode> grid)
        {
            Grid = grid;
            Grid.CreateOuterWalls();

            GenerateMesh();
            GenerateColliders();
        }

        private void IntializeTileMapButtons()
        {
            int fullTextureWidth = baseMaterial.mainTexture.width;
            int tiles = fullTextureWidth / 64;
            Vector2 tileTextureScale = new(1f / (tiles + 1), 1);

            for (int i = 0; i < tiles; i++)
            {
                var newImage = Instantiate(baseImage, new(), Quaternion.identity, baseImage.transform.parent);
                Vector2 offset = new(i * (1f / tiles) + .01f, 0);

                var newMaterial = new Material(baseMaterial)
                {
                    mainTextureScale = tileTextureScale,
                    mainTextureOffset = offset
                };

                newImage.name = $"TileMap {i}";
                newImage.GetComponent<Image>().material = newMaterial;
                newImage.GetComponent<Button>().onClick.AddListener(() => selectedUVValue = offset);
            }

            baseImage.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                var (x, y) = Grid.GetXY(UtilsClass.GetMouseWorldPosition());
                if (Grid[x, y] == null)
                    return;

                Grid[x, y].UV00 = selectedUVValue;
                Grid[x, y].UV11 = selectedUVValue;
                GenerateMesh();

                if (isColliderToggleChecked)
                {
                    Grid[x, y].isWalkable = false; //!grid[x, y].isWalkable;
                    GenerateColliders();
                }
            }

            if (tileMapMeshUpdated)
            {
                tileMapMeshFilter.mesh = mesh;
                tileMapMeshUpdated = false;
            }

            if (colliderMeshUpdated)
            {
                colliderMeshFilter.mesh = colliderMesh;
                colliderMeshUpdated = false;
            }
        }

        private HashSet<GameObject> colliderHolderObjects = new();
        private bool tileMapMeshUpdated;
        private bool colliderMeshUpdated;

        public void GenerateMesh()
        {
            if (mesh != null)
                mesh.Clear();

            mesh = Grid.CreateMesh();
            tileMapMeshUpdated = true;
        }

        public void GenerateColliders()
        {
            foreach (var colliderObj in colliderHolderObjects)
                Destroy(colliderObj);

            colliderHolderObjects = Grid.CreateColliders(transform);

            if (colliderMesh != null)
                colliderMesh.Clear();

            colliderMesh = Grid.CreateColliderMesh(true);
            colliderMeshUpdated = true;
        }

        private void ColliderToggleValueChanged(bool isChecked)
        {
            isColliderToggleChecked = isChecked;
            colliderMeshFilter.gameObject.SetActive(isColliderToggleChecked);
        }
    }

    [CustomEditor(typeof(TileMapDrawingSystem))]
    public class TileMapDrawingSystemEditor : Editor
    {
        private DungeonScriptableObject dungeon;

        public override void OnInspectorGUI()
        {
            var system = (TileMapDrawingSystem)target;
            if (system == null)
                return;

            #region Materials
            GUILayout.BeginHorizontal();
            GUILayout.Label("Base", GUILayout.Width(150));
            GUILayout.FlexibleSpace();
            GUILayout.Label("Collider Display", GUILayout.Width(150));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            system.baseMaterial = (Material)EditorGUILayout.ObjectField(system.baseMaterial, typeof(Material), false, GUILayout.Width(150));
            GUILayout.FlexibleSpace();
            system.colliderDisplayMaterial = (Material)EditorGUILayout.ObjectField(system.colliderDisplayMaterial, typeof(Material), false, GUILayout.Width(150));
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
            system.baseImage = (GameObject)EditorGUILayout.ObjectField(system.baseImage, typeof(GameObject), true, GUILayout.Width(150));
            GUILayout.FlexibleSpace();
            system.colliderToggle = (Toggle)EditorGUILayout.ObjectField(system.colliderToggle, typeof(Toggle), true, GUILayout.Width(150));
            GUILayout.EndHorizontal();
            #endregion

            GUILayout.Space(10);

            #region Grid
            EditorGUI.BeginChangeCheck();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Width: ");
            system.width = EditorGUILayout.IntSlider(system.width, 0, 100);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Height: ");
            system.height = EditorGUILayout.IntSlider(system.height, 0, 100);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Cell size: ");
            system.cellSize = EditorGUILayout.Slider(system.cellSize, 0, 15);
            GUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
                system.CreateGrid();
            #endregion

            GUILayout.Space(10);

            if (GUILayout.Button("Load"))
            {
                try
                {
                    string gridJsonFile = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), @"Assets\JSONFiles\asdasd.json"));
                    TileMapGridDTO gridData = JsonUtility.FromJson<TileMapGridDTO>(gridJsonFile);

                    system.width = gridData.GridWidth;
                    system.height = gridData.GridHeight;
                    system.cellSize = gridData.GridCellSize;

                    system.CreateGrid();

                    Grid<TileMapNode> Grid = new(system.width, system.height, system.cellSize, (g, x, y) => new(x, y, Vector2.zero, Vector2.zero, true));
                    system.SetGrid(Grid);
                    Grid.CreateOuterWalls();

                    system.GenerateMesh();
                    system.GenerateColliders();

                    for (int i = 0; i < system.width; i++)
                    {
                        for (int j = 0; j < system.height; j++)
                        {
                            system.Grid[i, j].UV00 = gridData.GridUV00s[i * system.height + j];
                            system.Grid[i, j].UV11 = gridData.GridUV11s[i * system.height + j];

                            system.Grid[i, j].isWalkable = gridData.GridIsWalkables[i * system.height + j];
                        }
                    }

                    system.GenerateMesh();
                    system.GenerateColliders();
                }
                catch (Exception e)
                {
                    Debug.Log(e.InnerException.Message + "-----> : " + e);
                }
            }

            if (GUILayout.Button("Save"))
            {
                List<List<Vector2>> uv00s = new();
                List<List<Vector2>> uv11s = new();

                List<List<bool>> walkable = new();


                for (int i = 0; i < system.width; i++)
                {
                    uv00s.Add(new());
                    uv11s.Add(new());
                    walkable.Add(new());

                    for (int j = 0; j < system.height; j++)
                    {
                        uv00s[i].Add(system.Grid[i, j].UV00);
                        uv11s[i].Add(system.Grid[i, j].UV11);

                        walkable[i].Add(system.Grid[i, j].isWalkable);
                    }
                }

                TileMapGridDTO dto = new(
                    gridWidth: system.width,
                    gridHeight: system.height,
                    gridCellSize: system.cellSize,
                    gridUV00s: uv00s,
                    gridUV11s: uv11s,
                    gridIsWalkables: walkable
                    );

                string gridJsonFile = JsonUtility.ToJson(dto);
                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), @"Assets\JSONFiles\asdasd.json"), gridJsonFile);
            }

            if (GUILayout.Button("Create a new grid"))
                system.SetGrid(new(system.width, system.height, system.cellSize, (g, x, y) => new(x, y, Vector2.zero, Vector2.zero, true)));
            SaveChanges();
        }
    }
}