using CodeMonkey.Utils;
using UnityEngine;

public class TestingGrid : MonoBehaviour
{
    [SerializeField] private HeatMapVisual heatMapVisual;
    private HeatMapGrid grid;

    void Awake()
    {
        grid = new(150, 60, 3, new(-170, -80));
        //new GridDebugVisual<int>(grid);

        heatMapVisual.Grid = grid;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePos = UtilsClass.GetMouseWorldPosition();
            grid.AddValue(mousePos, 100, 5, 20);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log(grid.GetValue(UtilsClass.GetMouseWorldPosition()));
        }
    }
}
