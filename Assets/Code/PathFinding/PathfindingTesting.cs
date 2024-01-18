using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.Diagnostics;

public class PathfindingTesting : MonoBehaviour
{
    private Pathfinding pathfinding;

    private void Start()
    {
        pathfinding = new(10, 10);
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
                Debug.Log("No path was found");

            for (int i = 0; i < path.Count - 1; i++)
            {
                Debug.DrawLine(new Vector2(path[i].x, path[i].y) * 10f + Vector2.one * 5f, new Vector2(path[i + 1].x, path[i + 1].y) * 10f + Vector2.one * 5f, Color.green, 1000);
            }
        }
    }
}
