using Assets.Code.Grid;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.TileMap
{
    public static class TileMapLoadingService
    {
        /// <summary>
        /// Creates a mesh which represents the given grid by drawing quads in positions of each grid cell based on their UV00 and UV11 values
        /// </summary>
        /// <param name="grid">Grid which will be used for generating a mesh using it's UV00 and UV11 coordinates</param>
        /// <param name="mesh">
        /// If null, creates a new one
        /// <br />If not null, changes the existing / given mesh
        /// </param>
        /// <returns>
        /// If mesh is null returns a new mesh containing the appropriate
        /// If mesh is not null returns the edited mesh containing the appropriate
        /// </returns>
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

        /// <summary>
        /// Create a mesh which represents the colliders of the given grid.
        /// <br />Both UV00 and UV11 will be set to Vector2(1, 0) if isWalkable and Vector2(0, 0) if not
        /// </summary>
        /// <param name="grid">Grid which will be used for generating a mesh using it's isWalkable property (Vector2(1, 0) if isWalkable and Vector2(0, 0) if not)</param>
        /// <param name="excludeWalkable">If true, only generates the mesh containing quads (cells) which are walkable</param>
        /// <returns>Newly generated mesh</returns>
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

        /// <summary>
        /// Creates colliders on appropriate cell positions of a given grid (non-walkable cells)
        /// </summary>
        /// <param name="grid">Grid which will be used for generating a mesh using it's isWalkable property</param>
        /// <param name="parent">Newly created objects will have this parent asigned to them, null means no parent</param>
        /// <returns>HashSet containing all the newly created gameobjects (collider holders)</returns>
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

        /// <summary>
        /// Creates outer walls / boundaries of the grid and asigns the colliders to them
        /// </summary>
        public static void CreateOuterWalls<T>(this IGrid<T> grid) where T : TileMapNode
        {
            if (grid.Height > 0)
            {
                //Goes through the lower edge of the grid
                for (int i = 0; i < grid.Width; i++)
                {
                    grid[i, 0].UV00 = new(1, 1);
                    grid[i, 0].UV11 = new(1, 1);
                    grid[i, 0].isWalkable = false;
                }

                //Goes through the upper edge of the grid
                for (int i = 0; i < grid.Width; i++)
                {
                    grid[i, grid.Height - 1].UV00 = new(1, 1);
                    grid[i, grid.Height - 1].UV11 = new(1, 1);
                    grid[i, grid.Height - 1].isWalkable = false;
                }
            }

            if (grid.Width > 0)
            {
                //Goes through the left edge of the grid
                for (int i = 0; i < grid.Height; i++)
                {
                    grid[0, i].UV00 = new(1, 1);
                    grid[0, i].UV11 = new(1, 1);
                    grid[0, i].isWalkable = false;
                }

                //Goes through the right edge of the grid
                for (int i = 0; i < grid.Height; i++)
                {
                    grid[grid.Width - 1, i].UV00 = new(1, 1);
                    grid[grid.Width - 1, i].UV11 = new(1, 1);
                    grid[grid.Width - 1, i].isWalkable = false;
                }
            }
        }
    }
}
