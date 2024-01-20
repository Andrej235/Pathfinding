using CodeMonkey.Utils;
using UnityEngine;

public class GridDebugVisual<T>
{
    private readonly TextMesh[,] debugTextArray;

    public GridDebugVisual(Grid<T> grid, bool includeText = true, Transform textParent = null)
    {
        if (includeText)
            debugTextArray = new TextMesh[grid.Width, grid.Height];

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                if (includeText)
                    debugTextArray[x, y] = UtilsClass.CreateWorldText(grid[x, y].ToString(), textParent, grid.GetWorldPosition(x, y) + new Vector2(grid.CellSize, grid.CellSize) * .5f, 25, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center);

                Debug.DrawLine(grid.GetWorldPosition(x, y), grid.GetWorldPosition(x, y + 1), Color.gray, 1000);
                Debug.DrawLine(grid.GetWorldPosition(x, y), grid.GetWorldPosition(x + 1, y), Color.gray, 1000);
            }
        }

        Debug.DrawLine(grid.GetWorldPosition(0, grid.Height), grid.GetWorldPosition(grid.Width, grid.Height), Color.gray, 1000);
        Debug.DrawLine(grid.GetWorldPosition(grid.Width, 0), grid.GetWorldPosition(grid.Width, grid.Height), Color.gray, 1000);

        if (includeText)
            grid.OnCellValueChanged += OnCellValueChanged;
    }

    private void OnCellValueChanged(object sender, Grid<T>.OnGridCellValueChangedEventArgs e) => debugTextArray[e.X, e.Y].text = e.NewValue.ToString();
}
