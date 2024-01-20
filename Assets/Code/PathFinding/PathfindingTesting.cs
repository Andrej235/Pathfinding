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
        /*        if (Input.GetMouseButtonDown(0))
                {
                    var mousePos = UtilsClass.GetMouseWorldPosition();
                    (int x, int y) = pathfinding.Grid.GetXY(mousePos);
                    var path = pathfinding.FindPath(0, 0, x, y);

                    if (path == null)
                    {
                        Debug.Log("No path was found");
                        return;
                    }

                    for (int i = 0; i < path.Count - 1; i++)
                        Debug.DrawLine(
                            GridMaster.Instance.Grid.GetWorldPosition(path[i].x, path[i].y) + Vector2.one * (GridMaster.Instance.Grid.CellSize / 2),
                            GridMaster.Instance.Grid.GetWorldPosition(path[i + 1].x, path[i + 1].y) + Vector2.one * (GridMaster.Instance.Grid.CellSize / 2),
                            Color.white,
                            100);
                }*/

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
