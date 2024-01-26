using Assets.Code.Grid;
using CodeMonkey.Utils;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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

        private Grid<TileMapNode> grid;
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
            if (grid != null)
            {
                if (grid.Height != height)
                {
                    var newGrid = new Grid<TileMapNode>(width, height, cellSize, (g, x, y) => new(x, y, Vector2.zero, Vector2.zero, true));

                    for (int i = 0; i < Math.Min(grid.Width, width); i++)
                    {
                        for (int j = 0; j < Math.Min(grid.Height, height) - 1; j++)
                        {
                            newGrid[newGrid.Width - 1 - i, j].UV00 = grid[grid.Width - 1 - i, j].UV00;
                            newGrid[newGrid.Width - 1 - i, j].UV11 = grid[grid.Width - 1 - i, j].UV11;
                        }
                    }

                    grid = newGrid;
                    grid.CreateOuterWalls();
                    GenerateMesh();
                    GenerateColliders();
                }

                if (grid.Width != width)
                {
                    var newGrid = new Grid<TileMapNode>(width, height, cellSize, (g, x, y) => new(x, y, Vector2.zero, Vector2.zero, true));

                    for (int i = 0; i < Math.Min(grid.Width, width) - 1; i++)
                    {
                        for (int j = 0; j < Math.Min(grid.Height, height); j++)
                        {
                            newGrid[newGrid.Width - 1 - i, j].UV00 = grid[grid.Width - 1 - i, j].UV00;
                            newGrid[newGrid.Width - 1 - i, j].UV11 = grid[grid.Width - 1 - i, j].UV11;
                        }
                    }

                    grid = newGrid;
                    grid.CreateOuterWalls();
                    GenerateMesh();
                    GenerateColliders();
                }

                if (grid.CellSize != cellSize)
                {
                    grid.CellSize = cellSize;
                    GenerateMesh();
                    GenerateColliders();
                }
                return;
            }

            grid = new(width, height, cellSize, (g, x, y) => new(x, y, Vector2.zero, Vector2.zero, true));
            grid.CreateOuterWalls();

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
                var (x, y) = grid.GetXY(UtilsClass.GetMouseWorldPosition());
                if (grid[x, y] == null)
                    return;

                grid[x, y].UV00 = selectedUVValue;
                grid[x, y].UV11 = selectedUVValue;
                GenerateMesh();

                if (isColliderToggleChecked)
                {
                    grid[x, y].isWalkable = false; //!grid[x, y].isWalkable;
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

        private void GenerateMesh()
        {
            if (mesh != null)
                mesh.Clear();

            mesh = grid.CreateMesh();
            tileMapMeshUpdated = true;
        }

        private void GenerateColliders()
        {
            foreach (var colliderObj in colliderHolderObjects)
                Destroy(colliderObj);

            colliderHolderObjects = grid.CreateColliders(transform);

            if (colliderMesh != null)
                colliderMesh.Clear();

            colliderMesh = grid.CreateColliderMesh(true);
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
            system.baseImage = (GameObject)EditorGUILayout.ObjectField(system.baseImage, typeof(GameObject), false, GUILayout.Width(150));
            GUILayout.FlexibleSpace();
            system.colliderToggle = (Toggle)EditorGUILayout.ObjectField(system.colliderToggle, typeof(Toggle), false, GUILayout.Width(150));
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
        }
    }
}
