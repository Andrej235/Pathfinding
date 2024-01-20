using CodeMonkey.Utils;
using UnityEngine;

public class PathfindingTesting : MonoBehaviour
{
    [SerializeField] private PathfindingVisual pathfindingVisual;
    private Pathfinding pathfinding;

    private void Start()
    {
        pathfinding = new(10, 10);
        pathfindingVisual.Pathfinding = pathfinding;
        new GridDebugVisual<PathNode>(pathfinding.Grid);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
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
            {
                Debug.DrawLine(new Vector2(path[i].x, path[i].y) * 10f + Vector2.one * 5f, new Vector2(path[i + 1].x, path[i + 1].y) * 10f + Vector2.one * 5f, Color.blue, 1000);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            var mousePos = UtilsClass.GetMouseWorldPosition();
            (int x, int y) = pathfinding.Grid.GetXY(mousePos);
            pathfinding.Grid[x, y].isWalkable = !pathfinding.Grid[x, y].isWalkable;
            pathfinding.Grid.RaiseOnCellValueChangedEvent(x, y);
        }
    }
}
