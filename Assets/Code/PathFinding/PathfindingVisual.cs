using UnityEngine;

public class PathfindingVisual : MonoBehaviour
{
    private Mesh mesh;
    private Vector2 quadSize;
    private Pathfinding pathfinding;
    public Pathfinding Pathfinding
    {
        get => pathfinding;
        set
        {
            if (pathfinding != null)
                pathfinding.Grid.OnCellValueChanged -= OnCellValueChanged;

            value.Grid.OnCellValueChanged += OnCellValueChanged;
            quadSize = new(value.Grid.CellSize, value.Grid.CellSize);
            pathfinding = value;

            mesh = new();
            GetComponent<MeshFilter>().mesh = mesh;
            CreateHeatMapVisual();
        }
    }

    private void Start()
    {
        if (Pathfinding == null)
            return;

        mesh = new();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateHeatMapVisual();
    }

    private void CreateHeatMapVisual()
    {
        MeshUtils.CreateEmptyMeshArrays(Pathfinding.Grid.Width * Pathfinding.Grid.Height, out Vector3[] vertices, out Vector2[] uvs, out int[] triangles);
        for (int x = 0; x < Pathfinding.Grid.Width; x++)
        {
            for (int y = 0; y < Pathfinding.Grid.Height; y++)
            {
                int index = x * Pathfinding.Grid.Height + y;

                Vector2 cellValueUV = Pathfinding.Grid[x, y].isWalkable ? Vector2.zero : Vector2.one;
                MeshUtils.AddToMeshArrays(vertices, uvs, triangles, index, Pathfinding.Grid.GetWorldPosition(x, y) + quadSize * .5f, 0, quadSize, cellValueUV, cellValueUV);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
    }

    bool updateMesh = false;
    private void OnCellValueChanged(object sender, Grid<PathNode>.OnGridCellValueChangedEventArgs e) => updateMesh = true;

    private void LateUpdate()
    {
        //Gets triggered after a frame ends
        if (updateMesh)
        {
            CreateHeatMapVisual();
            updateMesh = false;
        }
    }
}
