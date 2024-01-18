using UnityEngine;

public class HeatMapVisual : MonoBehaviour
{
    private Mesh mesh;
    private Vector2 quadSize;

    private HeatMapGrid grid;
    public HeatMapGrid Grid
    {
        get => grid;
        set
        {
            if (grid != null)
                grid.OnCellValueChanged -= OnCellValueChanged;

            value.OnCellValueChanged += OnCellValueChanged;
            quadSize = new(value.CellSize, value.CellSize);
            grid = value;
        }
    }

    private void Start()
    {
        mesh = new();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateHeatMapVisual();
    }

    private void CreateHeatMapVisual()
    {
        MeshUtils.CreateEmptyMeshArrays(Grid.Width * Grid.Height, out Vector3[] vertices, out Vector2[] uvs, out int[] triangles);
        for (int x = 0; x < Grid.Width; x++)
        {
            for (int y = 0; y < Grid.Height; y++)
            {
                int index = x * Grid.Height + y;

                int cellValue = Grid[x, y];
                float cellValueNormalized = (float)cellValue / HeatMapGrid.HEAT_MAP_MAX_VALUE;
                Vector2 cellValueUV = new(cellValueNormalized, 0);

                MeshUtils.AddToMeshArrays(vertices, uvs, triangles, index, Grid.GetWorldPosition(x, y) + quadSize * .5f, 0, quadSize, cellValueUV, cellValueUV);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
    }

    private void OnCellValueChanged(object sender, Grid<int>.OnGridCellValueChangedEventArgs e)
    {
        Vector2 cellValueUV = new((float)e.NewValue / HeatMapGrid.HEAT_MAP_MAX_VALUE, 0);
        MeshUtils.AddToMesh(mesh, Grid.GetWorldPosition(e.X, e.Y) + quadSize * .5f, 0, quadSize, cellValueUV, cellValueUV);
    }
}
