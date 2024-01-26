using Assets.Code.Grid;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.TileMap
{
    public static class TileMapLoadingService
    {
        public static Mesh CreateMesh<T>(this IGrid<T> grid, Mesh mesh = null) where T : TileMapNode
        {
            Vector2 quadSize = new(grid.CellSize, grid.CellSize);
            mesh = mesh != null ? mesh : new();

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

        public static Mesh CreateColliderMesh<T>(this IGrid<T> grid, bool excludeWalkable = false) where T : PathNode
        {
            try
            {
                Vector2 quadSize = new(grid.CellSize, grid.CellSize);
                Mesh mesh = new();
                MeshUtils.CreateEmptyMeshArrays(grid.Width * grid.Height, out Vector3[] vertices, out Vector2[] uvs, out int[] triangles);

                if (!excludeWalkable)
                {
                    for (int x = 0; x < grid.Width; x++)
                    {
                        for (int y = 0; y < grid.Height; y++)
                        {
                            int index = x * grid.Height + y;
                            Vector2 uv = grid[x, y].isWalkable ? new(1, 0) : new(0, 0);
                            MeshUtils.AddToMeshArrays(vertices, uvs, triangles, index, grid.GetWorldPosition(x, y) + quadSize * .5f, 0, quadSize, uv, uv);
                        }
                    }
                }
                else
                {
                    for (int x = 0; x < grid.Width; x++)
                    {
                        for (int y = 0; y < grid.Height; y++)
                        {
                            int index = x * grid.Height + y;
                            if (!grid[x, y].isWalkable)
                                MeshUtils.AddToMeshArrays(vertices, uvs, triangles, index, grid.GetWorldPosition(x, y) + quadSize * .5f, 0, quadSize, new(), new());
                        }
                    }
                }

                mesh.vertices = vertices;
                mesh.uv = uvs;
                mesh.triangles = triangles;
                return mesh;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return new();
            }
        }

        public static HashSet<GameObject> CreateColliders<T>(this IGrid<T> grid, Transform parent = null) where T : PathNode
        {
            HashSet<GameObject> colliders = new();
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
                    colliders.Add(newColliderObj);
                }
            }
            return colliders;
        }

        public static void CreateOuterWalls<T>(this IGrid<T> grid) where T : TileMapNode
        {
            if (grid.Height > 0)
            {
                for (int i = 0; i < grid.Width; i++)
                {
                    grid[i, 0].UV00 = new(1, 1);
                    grid[i, 0].UV11 = new(1, 1);
                }

                for (int i = 0; i < grid.Width; i++)
                {
                    grid[i, grid.Height - 1].UV00 = new(1, 1);
                    grid[i, grid.Height - 1].UV11 = new(1, 1);
                } 
            }

            if (grid.Width > 0)
            {
                for (int i = 0; i < grid.Height; i++)
                {
                    grid[0, i].UV00 = new(1, 1);
                    grid[0, i].UV11 = new(1, 1);
                }

                for (int i = 0; i < grid.Height; i++)
                {
                    grid[grid.Width - 1, i].UV00 = new(1, 1);
                    grid[grid.Width - 1, i].UV11 = new(1, 1);
                } 
            }
        }
    }
}
