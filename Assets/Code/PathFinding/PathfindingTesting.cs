using CodeMonkey.Utils;
using UnityEngine;

public class PathfindingTesting : MonoBehaviour
{
    [SerializeField] private PathfindingVisual pathfindingVisual;
    [SerializeField] private bool enableDebug;
    [SerializeField] private bool includeText = true;
    private Pathfinding pathfinding;

    private void Start()
    {
        pathfinding = GridMaster.Instance.Pathfinding;
        pathfindingVisual.Pathfinding = pathfinding;

        if (enableDebug)
            new GridDebugVisual<PathNode>(pathfinding.Grid, includeText, transform);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePos = UtilsClass.GetMouseWorldPosition();
            (int x, int y) = pathfinding.Grid.GetXY(mousePos);
            if (pathfinding.Grid[x, y] is null)
                return;

            pathfinding.Grid[x, y].isWalkable = !pathfinding.Grid[x, y].isWalkable;
            pathfinding.Grid.RaiseOnCellValueChangedEvent(x, y);
        }
    }
}
