using CodeMonkey.Utils;
using UnityEngine;

public class GridDebugVisual<T>
{
    private readonly TextMesh[,] debugTextArray;

    public GridDebugVisual(Grid<T> grid)
    {
        debugTextArray = new TextMesh[grid.Width, grid.Height];

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                debugTextArray[x, y] = UtilsClass.CreateWorldText(grid[x, y].ToString(), null, grid.GetWorldPosition(x, y) + new Vector2(grid.CellSize, grid.CellSize) * .5f, 25, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center);
                Debug.DrawLine(grid.GetWorldPosition(x, y), grid.GetWorldPosition(x, y + 1), Color.white, 1000);
                Debug.DrawLine(grid.GetWorldPosition(x, y), grid.GetWorldPosition(x + 1, y), Color.white, 1000);
            }
        }

        Debug.DrawLine(grid.GetWorldPosition(0, grid.Height), grid.GetWorldPosition(grid.Width, grid.Height), Color.white, 1000);
        Debug.DrawLine(grid.GetWorldPosition(grid.Width, 0), grid.GetWorldPosition(grid.Width, grid.Height), Color.white, 1000);

        grid.OnCellValueChanged += OnCellValueChanged;
    }

    private void OnCellValueChanged(object sender, Grid<T>.OnGridCellValueChangedEventArgs e) => debugTextArray[e.X, e.Y].text = e.NewValue.ToString();
}
