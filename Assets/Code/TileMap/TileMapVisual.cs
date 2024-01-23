using System;
using UnityEngine;

#nullable enable
[ExecuteInEditMode]
public class TileMapVisual : MonoBehaviour
{
    public Grid<PathNode>? Grid { get; private set; }

    public int Width
    {
        get => width;
        set
        {
            if (value < 0)
                return;

            width = value;
        }
    }
    private int width;

    public int Height
    {
        get => height;
        set
        {
            if (value < 0)
                return;

            height = value;
        }
    }
    private int height;

    public float CellSize
    {
        get => cellSize;
        set
        {
            if (value < 0)
                return;

            cellSize = value;
        }
    }
    private float cellSize;

    public Vector2 Origin { get; set; }
    public bool ShowGrid { get; set; }
    private Mesh? mesh;

    private void OnValidate() => RegenerateMesh();

    private void OnDrawGizmos()
    {
        if (!ShowGrid || Grid is null)
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

    public void RegenerateMesh()
    {
        if (mesh == null)
            return;

        mesh.Clear();
        //TODO: somehow save the values of the PathNodes (isWalkable) as they get reset every time the mesh is regenerated
        Grid = new Grid<PathNode>(Width, Height, CellSize, Origin, (g, x, y) => new(x, y));
        GenerateMeshVisual();
    }

    public void GenerateMesh()
    {
        mesh = new();
        GetComponent<MeshFilter>().mesh = mesh;
        Grid = new Grid<PathNode>(Width, Height, CellSize, Origin, (g, x, y) => new(x, y));
        GenerateMeshVisual();
    }

    private void GenerateMeshVisual()
    {
        if (mesh == null || Grid is null)
            return;

        Vector2 quadSize = new(CellSize, CellSize);
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
}