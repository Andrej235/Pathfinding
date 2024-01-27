using Assets.Code.Grid;
using CodeMonkey.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

            IntializeTileMapButtons();
        }

        public void CreateGrid()
        {
            if (Grid == null)
                return;

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
                    Grid[x, y].isWalkable = false;
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
}