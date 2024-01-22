using CodeMonkey.Utils;
using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class Editor : MonoBehaviour
{
    private Grid<PathNode> grid;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int cellSize;
    [SerializeField] private Vector2 origin;
    [SerializeField] private bool showGrid;
    private Mesh mesh;

    private void OnValidate()
    {
        if (mesh == null)
            mesh = new Mesh();
        else
            mesh.Clear();

        grid = new Grid<PathNode>(width, height, cellSize, origin, (g, x, y) => new(x, y));

        Vector2 quadSize = new(cellSize, cellSize);
        MeshUtils.CreateEmptyMeshArrays(grid.Width * grid.Height, out Vector3[] vertices, out Vector2[] uvs, out int[] triangles);
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                int index = x * grid.Height + y;

                Vector2 cellValueUV = grid[x, y].isWalkable ? Vector2.zero : Vector2.one;
                MeshUtils.AddToMeshArrays(vertices, uvs, triangles, index, grid.GetWorldPosition(x, y) + quadSize * .5f, 0, quadSize, cellValueUV, cellValueUV);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
    }

    private void Update()
    {
        GetComponent<MeshFilter>().mesh = mesh;

        Debug.Log("A");

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("B");
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGrid)
            return;

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                Gizmos.DrawLine(grid.GetWorldPosition(x, y), grid.GetWorldPosition(x, y + 1));
                Gizmos.DrawLine(grid.GetWorldPosition(x, y), grid.GetWorldPosition(x + 1, y));
            }
        }
        Gizmos.DrawLine(grid.GetWorldPosition(0, grid.Height), grid.GetWorldPosition(grid.Width, grid.Height));
        Gizmos.DrawLine(grid.GetWorldPosition(grid.Width, 0), grid.GetWorldPosition(grid.Width, grid.Height));
    }
}
