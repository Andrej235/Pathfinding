using Assets.Code.Grid;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.TileMap
{
    public static class TileMapLoadingService
    {
        public static Mesh CreateMesh<T>(this IGrid<T> grid) where T : TileMapNode
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

        private static readonly List<GameObject> Colliders = new();
        public static void CreateColliders<T>(this IGrid<T> grid, Transform parent = null) where T : PathNode
        {
            Vector2 quadSize = new(grid.CellSize, grid.CellSize);
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    if (grid[x, y].isWalkable)
                        continue;

                    var newColliderObj = new GameObject
                    {
                        name = $"ColliderHolder",
                    };
                    newColliderObj.transform.position = grid.GetWorldPosition(x, y) + quadSize * .5f;
                    newColliderObj.transform.parent = parent;
                    var newCollider = newColliderObj.AddComponent<BoxCollider2D>();
                    newCollider.size = quadSize;

                    Colliders.Add(newColliderObj);
                }
            }
        }
    }
}
