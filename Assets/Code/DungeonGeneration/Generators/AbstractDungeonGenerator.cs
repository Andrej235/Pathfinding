using Assets.Code.DungeonGeneration.Models;
using Assets.Code.Grid;
using Assets.Code.PathFinding;
using Assets.Code.Utility;
using CodeMonkey.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;

    protected TilemapVisualizer tileMapVisualizer;
    [SerializeField] protected DungeonParametersSO parameters;
    [SerializeField] protected Vector2Int startPosition;
    private Grid<PathNode> grid;
    protected Pathfinding pathfinding;

    protected Room startRoom;
    protected Room bossRoom;

    public Pathfinding DungeonPathfinding => pathfinding;
    protected Dungeon dungeonData;

    /// <summary>
    /// Begins the procedural generation
    /// </summary>
    public void GenerateDungeon()
    {
        tileMapVisualizer = new(parameters, floorTilemap, wallTilemap);
        tileMapVisualizer.Clear();

        dungeonData ??= new();
        for (int i = transform.childCount; i > 0; --i)
            DestroyImmediate(transform.GetChild(0).gameObject);

        dungeonData.Reset();

        grid = DungeonGridGenerator.GeneratePathNodeGrid(RunProceduralGeneration());
        pathfinding = new Pathfinding(grid);

        AsignRoomTypes();
        PopulateRooms();

        foreach (var room in dungeonData.Rooms)
            room.UpdateTilesAccessibleFromPath();
    }

    private void AsignRoomTypes()
    {
        startRoom = dungeonData.Rooms.GetRandomElement();
        startRoom.type = Room.RoomType.Start;

        var rooms = dungeonData.Rooms.OrderBy(x => pathfinding.FindPath(startRoom.RoomCenter, x.RoomCenter).Count);
        foreach (var room in rooms)
            room.type = parameters.roomTypesChance.GetByChance();
        //room.type = (Room.RoomType)Enum.GetValues(typeof(Room.RoomType)).GetRandomElement();

        //The furthest room is supposed to be the boss room
        bossRoom = rooms.Last();
        bossRoom.type = Room.RoomType.Boss;
    }

    protected abstract void PopulateRooms();

    /// <summary>
    /// Generates a dungeon
    /// </summary>
    /// <returns>An IEnumerable<Vector2Int> where each element represents a position of a wall</returns>
    protected abstract IEnumerable<Vector2Int> RunProceduralGeneration();

    #region Gizmos / Testing
    private List<PathNode> path;
    [SerializeField] private bool includeGridGizmos;
    [SerializeField] private bool includeRoomGizmos;
    [SerializeField] private bool includeAccessibleFromPathGizmos;
    [SerializeField] private bool includePathGizmos;
    [SerializeField] private bool includeRoomTypeGizmos;

    private void OnDrawGizmos()
    {
        if (grid == null || dungeonData is null)
            return;

        if (includeRoomTypeGizmos)
        {
            foreach (var room in dungeonData.Rooms)
            {
                Gizmos.color = room.type switch
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

            Gizmos.color = Color.cyan;
            DrawCubes(startRoom.Floor);

            Gizmos.color = Color.red;
            DrawCubes(bossRoom.Floor);

            return;
        }

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

        if (path != null && path.Any())
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < path.Count - 1; i++)
                Gizmos.DrawLine(
                    grid.GetWorldPosition(path[i].x, path[i].y) + Vector2.one * (grid.CellSize / 2),
                    grid.GetWorldPosition(path[i + 1].x, path[i + 1].y) + Vector2.one * (grid.CellSize / 2));
            Gizmos.color = Color.black;
        }

        if (includeRoomGizmos)
        {
            foreach (var room in dungeonData.Rooms)
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
            foreach (var room in dungeonData.Rooms)
                DrawCubes(room.TilesAccessibleFromPath);
        }

        if (includePathGizmos)
        {
            Gizmos.color = Color.black;
            foreach (var tile in dungeonData.Path)
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
