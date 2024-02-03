using Assets.Code.Grid;
using Assets.Code.PathFinding;
using CodeMonkey.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField] protected TilemapVisualizer tileMapVisualizer;
    [SerializeField] protected Vector2Int startPosition;
    private Grid<PathNode> grid;
    private Pathfinding pathfinding;
    public Pathfinding DungeonPathfinding => pathfinding;

    /// <summary>
    /// Begins the procedural generation
    /// </summary>
    public void GenerateDungeon()
    {
        tileMapVisualizer.Clear();

        grid = DungeonGridGenerator.GeneratePathNodeGrid(RunProceduralGeneration());
        pathfinding = new Pathfinding(grid);
    }

    /// <summary>
    /// Generates a dungeon
    /// </summary>
    /// <returns>An IEnumerable<Vector2Int> where each element represents a position of a wall</returns>
    protected abstract IEnumerable<Vector2Int> RunProceduralGeneration();

    #region Testing
    private List<PathNode> path;
    [SerializeField] private bool includeGridGizmos;

    private void OnDrawGizmos()
    {
        if (grid is null)
            return;

        if (includeGridGizmos)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    if (!grid[x, y].isWalkable)
                        Gizmos.color = Color.white;

                    Gizmos.DrawLine(grid.GetWorldPosition(x, y), grid.GetWorldPosition(x, y + 1));
                    Gizmos.DrawLine(grid.GetWorldPosition(x, y), grid.GetWorldPosition(x + 1, y));

                    Gizmos.color = Color.black;
                }
            }

            Gizmos.DrawLine(grid.GetWorldPosition(0, grid.Height), grid.GetWorldPosition(grid.Width, grid.Height));
            Gizmos.DrawLine(grid.GetWorldPosition(grid.Width, 0), grid.GetWorldPosition(grid.Width, grid.Height));
        }

        if (path is null || !path.Any())
            return;

        Gizmos.color = Color.blue;
        for (int i = 0; i < path.Count - 1; i++)
            Gizmos.DrawLine(
                grid.GetWorldPosition(path[i].x, path[i].y) + Vector2.one * (grid.CellSize / 2),
                grid.GetWorldPosition(path[i + 1].x, path[i + 1].y) + Vector2.one * (grid.CellSize / 2));
        Gizmos.color = Color.black;
    }

    Vector2? pathfindingStartPos = null;
    [SerializeField] uint pathfindingDepth;

    private void Update()
    {
        if (pathfinding != null && Input.GetMouseButtonDown(0))
        {
            if (pathfindingStartPos is null)
            {
                path = null;
                pathfindingStartPos = UtilsClass.GetMouseWorldPosition();
            }
            else
            {
                path = pathfinding.FindPath(pathfindingStartPos ?? Vector2.zero, UtilsClass.GetMouseWorldPosition(), pathfindingDepth);
                pathfindingStartPos = null;
            }
        }
    }
    #endregion
}
