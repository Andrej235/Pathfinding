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
        public Material BaseMaterial
        {
            get => baseMaterial;
            set
            {
                if (BaseMaterial == value)
                    return;

                baseMaterial = value;
                IntializeTileMapButtons();
                tileMapMeshRenderer.material = BaseMaterial;
            }
        }
        private Material baseMaterial;

        public Material colliderDisplayMaterial;
        public GameObject baseImage;
        public Toggle colliderToggle;
        public int Width { get; set; }
        public int Height { get; set; }
        public float CellSize { get; set; }

        private MeshFilter tileMapMeshFilter;
        private MeshRenderer tileMapMeshRenderer;
        private MeshFilter colliderMeshFilter;

        private Mesh mesh;
        private Mesh colliderMesh;

        public Grid<TileMapNode> Grid { get; private set; }
        private Vector2 selectedUVValue = new();
        private bool isColliderToggleChecked;

        private void Start()
        {
            GameObject tileMapMeshFilterHolder = new() { name = "TileMapMeshFilterHolder" };
            tileMapMeshRenderer = tileMapMeshFilterHolder.AddComponent<MeshRenderer>();
            tileMapMeshRenderer.material = BaseMaterial;
            tileMapMeshFilter = tileMapMeshFilterHolder.AddComponent<MeshFilter>();

            GameObject colliderMeshFilterHolder = new() { name = "ColliderMeshFilterHolder" };
            colliderMeshFilterHolder.transform.position = colliderMeshFilterHolder.transform.position + new Vector3(0, 0, -1);
            var colliderMeshRenderer = colliderMeshFilterHolder.AddComponent<MeshRenderer>();
            colliderMeshRenderer.material = colliderDisplayMaterial;
            colliderMeshFilter = colliderMeshFilterHolder.AddComponent<MeshFilter>();

            colliderToggle.onValueChanged.AddListener(ColliderToggleValueChanged);
        }

        public void RecreateGrid()
        {
            if (Grid == null)
                return;

            if (Grid.Height != Height || Grid.Width != Width)
            {
                var newGrid = new Grid<TileMapNode>(Width, Height, CellSize, (g, x, y) => new(x, y, Vector2.zero, Vector2.zero, true));
                var minWidth = Math.Min(Grid.Width, Width);
                var minHeight = Math.Min(Grid.Height, Height);

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

            if (Grid.CellSize != CellSize)
            {
                Grid.CellSize = CellSize;
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

        private readonly HashSet<GameObject> tileMapPaletteObjects = new();
        private void IntializeTileMapButtons()
        {
            if (BaseMaterial == null)
                return;

            baseImage.SetActive(true);
            foreach (var x in tileMapPaletteObjects)
                Destroy(x);

            int fullTextureWidth = BaseMaterial.mainTexture.width;
            int tiles = fullTextureWidth / 64;
            Vector2 tileTextureScale = new(1f / (tiles + 1), 1);

            for (int i = 0; i < tiles; i++)
            {
                var newImage = Instantiate(baseImage, new(), Quaternion.identity, baseImage.transform.parent);
                tileMapPaletteObjects.Add(newImage);
                Vector2 offset = new(i * (1f / tiles) + .01f, 0);

                var newMaterial = new Material(BaseMaterial)
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

        public void LoadDTO(TileMapGridDTO tileMapGridDTO)
        {
            Width = tileMapGridDTO.GridWidth;
            Height = tileMapGridDTO.GridHeight;
            CellSize = tileMapGridDTO.GridCellSize;

            Grid = new(Width, Height, CellSize, (g, x, y) => new(x, y, Vector2.zero, Vector2.zero, true));
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Grid[i, j].UV00 = tileMapGridDTO.GridUV00s[i * Height + j];
                    Grid[i, j].UV11 = tileMapGridDTO.GridUV11s[i * Height + j];

                    Grid[i, j].isWalkable = tileMapGridDTO.GridIsWalkables[i * Height + j];
                }
            }

            GenerateMesh();
            GenerateColliders();
        }

        public TileMapGridDTO GetDTO()
        {
            if (Grid is null)
                return null;

            List<List<Vector2>> uv00s = new();
            List<List<Vector2>> uv11s = new();
            List<List<bool>> walkable = new();

            for (int i = 0; i < Width; i++)
            {
                uv00s.Add(new());
                uv11s.Add(new());
                walkable.Add(new());

                for (int j = 0; j < Height; j++)
                {
                    uv00s[i].Add(Grid[i, j].UV00);
                    uv11s[i].Add(Grid[i, j].UV11);
                    walkable[i].Add(Grid[i, j].isWalkable);
                }
            }

            return new(gridWidth: Width,
                       gridHeight: Height,
                       gridCellSize: CellSize,
                       gridUV00s: uv00s,
                       gridUV11s: uv11s,
                       gridIsWalkables: walkable);
        }
    }
}