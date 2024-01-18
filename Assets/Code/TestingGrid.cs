using CodeMonkey.Utils;
using UnityEngine;

public class TestingGrid : MonoBehaviour
{
    [SerializeField] private HeatMapVisual heatMapVisual;
    private HeatMapGrid grid;

    void Awake()
    {
        grid = new(40, 20, 5, new(-100, -45));
        new GridDebugVisual<int>(grid);

        heatMapVisual.Grid = grid;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePos = UtilsClass.GetMouseWorldPosition();
            grid.AddValue(mousePos, 5, 7);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log(grid.GetValue(UtilsClass.GetMouseWorldPosition()));
        }
    }
}
