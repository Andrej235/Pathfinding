using CodeMonkey.Utils;
using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class Editor : MonoBehaviour
{
    public Grid<PathNode> Grid { get; private set; }
    public int width;
    public int height;
    public int cellSize;
    public Vector2 origin;
    public bool showGrid;
    private Mesh mesh;

    private void OnValidate()
    {
        if (mesh == null)
            mesh = new Mesh();
        else
            mesh.Clear();

        Grid = new Grid<PathNode>(width, height, cellSize, origin, (g, x, y) => new(x, y));

        Vector2 quadSize = new(cellSize, cellSize);
        MeshUtils.CreateEmptyMeshArrays(Grid.Width * Grid.Height, out Vector3[] vertices, out Vector2[] uvs, out int[] triangles);
        for (int x = 0; x < Grid.Width; x++)
        {
            for (int y = 0; y < Grid.Height; y++)
            {
                int index = x * Grid.Height + y;

                Vector2 cellValueUV = Grid[x, y].isWalkable ? Vector2.zero : Vector2.one;
                MeshUtils.AddToMeshArrays(vertices, uvs, triangles, index, Grid.GetWorldPosition(x, y) + quadSize * .5f, 0, quadSize, cellValueUV, cellValueUV);
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

        for (int x = 0; x < Grid.Width; x++)
        {
            for (int y = 0; y < Grid.Height; y++)
            {
                Gizmos.DrawLine(Grid.GetWorldPosition(x, y), Grid.GetWorldPosition(x, y + 1));
                Gizmos.DrawLine(Grid.GetWorldPosition(x, y), Grid.GetWorldPosition(x + 1, y));
            }
        }
        Gizmos.DrawLine(Grid.GetWorldPosition(0, Grid.Height), Grid.GetWorldPosition(Grid.Width, Grid.Height));
        Gizmos.DrawLine(Grid.GetWorldPosition(Grid.Width, 0), Grid.GetWorldPosition(Grid.Width, Grid.Height));
    }
}
