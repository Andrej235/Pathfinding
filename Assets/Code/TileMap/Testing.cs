using Assets.Code.Grid;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.TileMap
{
    public class Testing : MonoBehaviour
    {
        [SerializeField] private Material baseImageMaterial;
        [SerializeField] private GameObject baseImage;
        private Grid<TileMapNode> grid;
        private Mesh mesh;
        private Vector2 selectedUVValue = new();

        private void Start()
        {
            grid = new(5, 5, 15, (g, x, y) => new(x, y, Vector2.zero, Vector2.zero, true));
            Pathfinding pathfinding = new(grid);
            GenerateMesh();

            int fullTextureWidth = baseImageMaterial.mainTexture.width;
            int tiles = fullTextureWidth / 64;
            Vector2 tileTextureScale = new(1f / (tiles + 1), 1);

            for (int i = 0; i < tiles; i++)
            {
                var newImage = Instantiate(baseImage, new(), Quaternion.identity, baseImage.transform.parent);
                Vector2 offset = new(i * (1f / tiles) + .01f, 0);

                var newMaterial = new Material(baseImageMaterial)
                {
                    mainTextureScale = tileTextureScale,
                    mainTextureOffset = offset
                };

                newImage.name = $"TileMap {i}";
                newImage.GetComponent<Image>().material = newMaterial;
                newImage.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Debug.Log(offset);
                    selectedUVValue = offset;
                });
            }

            baseImage.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Click <---> Testing");

                var (x, y) = grid.GetXY(UtilsClass.GetMouseWorldPosition());
                if (grid[x, y] == null)
                    return;

                grid[x, y].UV00 = selectedUVValue;
                grid[x, y].UV11 = selectedUVValue;
                GenerateMesh();
            }
        }

        private void GenerateMesh()
        {
            if (mesh != null)
                mesh.Clear();

            mesh = grid.CreateMesh();
            GetComponent<MeshFilter>().mesh = mesh;
        }
    }
}
