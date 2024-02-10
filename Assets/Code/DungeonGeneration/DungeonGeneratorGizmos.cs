using Assets.Code.DungeonGeneration.Models;
using CodeMonkey.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable
public class DungeonGeneratorGizmos : MonoBehaviour
{
    [SerializeField] private AbstractDungeonGenerator? generator;

    private List<PathNode>? path;
    [SerializeField] private bool includeGridGizmos;
    [SerializeField] private bool includeRoomGizmos;
    [SerializeField] private bool includeAccessibleFromPathGizmos;
    [SerializeField] private bool includePathGizmos;
    [SerializeField] private bool includeRoomTypeGizmos;

    private void OnDrawGizmos()
    {
        if (generator == null || generator.DungeonGrid == null || generator.DungeonData is null)
            return;

        if (includeRoomTypeGizmos)
        {
            foreach (var room in generator.DungeonData.Rooms)
            {
                Gizmos.color = room.Type switch
                {
                    Room.RoomType.None => Color.black,
                    Room.RoomType.Start => Color.cyan,
                    Room.RoomType.Enemy => Color.yellow,
                    Room.RoomType.Treassure => Color.magenta,
                    Room.RoomType.Boss => Color.red,
                    _ => Color.white,
                };

                DrawCubes(room.Floor);
            }

            return;
        }

        if (includeGridGizmos)
        {
            for (int x = 0; x < generator.DungeonGrid.Width; x++)
            {
                for (int y = 0; y < generator.DungeonGrid.Height; y++)
                {
                    if (!generator.DungeonGrid[x, y].isWalkable)
                        Gizmos.color = Color.white;

                    Gizmos.DrawLine(generator.DungeonGrid.GetWorldPosition(x, y), generator.DungeonGrid.GetWorldPosition(x, y + 1));
                    Gizmos.DrawLine(generator.DungeonGrid.GetWorldPosition(x, y), generator.DungeonGrid.GetWorldPosition(x + 1, y));
                    Gizmos.color = Color.black;
                }
            }
            Gizmos.DrawLine(generator.DungeonGrid.GetWorldPosition(0, generator.DungeonGrid.Height), generator.DungeonGrid.GetWorldPosition(generator.DungeonGrid.Width, generator.DungeonGrid.Height));
            Gizmos.DrawLine(generator.DungeonGrid.GetWorldPosition(generator.DungeonGrid.Width, 0), generator.DungeonGrid.GetWorldPosition(generator.DungeonGrid.Width, generator.DungeonGrid.Height));
        }
        if (path != null && path.Any())
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < path.Count - 1; i++)
                Gizmos.DrawLine(
                generator.DungeonGrid.GetWorldPosition(path[i].x, path[i].y) + Vector2.one * (generator.DungeonGrid.CellSize / 2),
                    generator.DungeonGrid.GetWorldPosition(path[i + 1].x, path[i + 1].y) + Vector2.one * (generator.DungeonGrid.CellSize / 2));
            Gizmos.color = Color.black;
        }

        if (includeRoomGizmos)
        {
            foreach (var room in generator.DungeonData.Rooms)
            {
                Gizmos.color = Color.black;
                DrawCubes(room.InnerTiles);

                Gizmos.color = Color.gray;
                DrawCubes(room.CornerTiles);

                Gizmos.color = Color.red;
                DrawCubes(room.TilesNextToTopWall);

                Gizmos.color = Color.green;
                DrawCubes(room.TilesNextToRightWall);

                Gizmos.color = Color.blue;
                DrawCubes(room.TilesNextToBottomWall);

                Gizmos.color = Color.cyan;
                DrawCubes(room.TilesNextToLeftWall);
            }
        }

        if (includeAccessibleFromPathGizmos)
        {
            Gizmos.color = Color.white;
            foreach (var room in generator.DungeonData.Rooms)
                DrawCubes(room.TilesAccessibleFromPath);
        }

        if (includePathGizmos)
        {
            Gizmos.color = Color.black;
            foreach (var tile in generator.DungeonData.Path)
                DrawCube(tile);
        }

        static void DrawCube(Vector2Int tile) => Gizmos.DrawCube(tile + Vector2.one * .5f, new(1, 1));
        static void DrawCubes(IEnumerable<Vector2Int> tiles)
        {
            foreach (var tile in tiles)
                Gizmos.DrawCube(tile + (Vector2.one * .5f), new(1, 1));
        }
    }

    Vector2? pathfindingStartPos = null;
    [SerializeField] uint pathfindingDepth;

    private void Update()
    {
        if (generator == null)
            return;

        if (generator.DungeonPathfinding != null && Input.GetMouseButtonDown(0))
        {
            if (pathfindingStartPos is null)
            {
                path = null;
                pathfindingStartPos = UtilsClass.GetMouseWorldPosition();
            }
            else
            {
                path = generator.DungeonPathfinding.FindPath(pathfindingStartPos ?? Vector2.zero, UtilsClass.GetMouseWorldPosition(), pathfindingDepth);
                pathfindingStartPos = null;
            }
        }
    }
}
