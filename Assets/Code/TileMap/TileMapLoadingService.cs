using Assets.Code.Grid;
using UnityEngine;

namespace Assets.Code.TileMap
{
    public static class TileMapLoadingService
    {
        public static Mesh CreateMesh<T>(this Grid<T> grid) where T : TileMapNode
        {
            Vector2 quadSize = new(grid.CellSize, grid.CellSize);
            Mesh mesh = new();
            MeshUtils.CreateEmptyMeshArrays(grid.Width * grid.Height, out Vector3[] vertices, out Vector2[] uvs, out int[] triangles);
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    int index = x * grid.Height + y;

                    MeshUtils.AddToMeshArrays(vertices, uvs, triangles, index, grid.GetWorldPosition(x, y) + quadSize * .5f, 0, quadSize, grid[x, y].UV00, grid[x, y].UV11);
                }
            }

            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            return mesh;
        }
    }
}
