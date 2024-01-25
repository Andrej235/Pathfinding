using Assets.Code.Grid;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Code.TileMap
{
    public class Testing : MonoBehaviour
    {
        [SerializeField] private Material baseImageMaterial;
        [SerializeField] private GameObject baseImage;
        [SerializeField] private GameObject canvas;

        private void Start()
        {
            Grid<TileMapNode> grid = new(50, 50, 1, (g, x, y) => new(x, y, new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)), new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)), true));
            var mesh = grid.CreateMesh();
            GetComponent<MeshFilter>().mesh = mesh;
            Pathfinding pathfinding = new(grid);

            int fullTextureWidth = baseImageMaterial.mainTexture.width;
            int tiles = fullTextureWidth / 64;
            Vector2 tileTextureScale = new(1f / (tiles + 1), 1);

            var baseImageLocation = baseImage.GetComponent<RectTransform>().position;
            var baseimageWidth = baseImage.GetComponent<RectTransform>().rect.width;

            for (int i = 0; i < tiles; i++)
            {
                var newImage = Instantiate(baseImage, baseImageLocation + new Vector3(baseimageWidth * i, 0), Quaternion.identity, canvas.transform);

                var newMaterial = new Material(baseImageMaterial)
                {
                    mainTextureScale = tileTextureScale,
                    mainTextureOffset = new(i * (1f / tiles) + .01f, 0)
                };

                newImage.GetComponent<Image>().material = newMaterial;
            }
        }
    }
}
