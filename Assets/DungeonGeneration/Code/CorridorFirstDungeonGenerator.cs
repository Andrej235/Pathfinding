using Assets.Code.Grid;
using CodeMonkey.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField] private int corridorLength = 14;
    [SerializeField] private int corridorCount = 5;
    [SerializeField][Range(.1f, 1f)] private float roomPercent = .8f;

    protected override void RunProceduralGeneration() => CorridorFirstGeneration();

    private void CorridorFirstGeneration()
    {
        HashSet<Vector2Int> floorPositions = new();
        HashSet<Vector2Int> potentialRoomPositions = new();

        List<List<Vector2Int>> corridors = CreateCorridors(floorPositions, potentialRoomPositions);
        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);

        List<Vector2Int> deadEnds = GetDeadEnds(floorPositions);
        CreateRoomsAtDeadEnds(deadEnds, roomPositions);
        floorPositions.UnionWith(roomPositions);

        corridors.Aggregate(floorPositions, (floorPositions, corridor) =>
        {
            floorPositions.UnionWith(IncreaseCorridorSizeToThree(corridor));
            return floorPositions;
        });

        tileMapVisualizer.PaintFloorTiles(floorPositions);
        var wallPositions = WallGenerator.CreateWalls(floorPositions, tileMapVisualizer);

        //Grid
        var minX = wallPositions.Min(x => x.x);
        var maxX = wallPositions.Max(x => x.x);
        var minY = wallPositions.Min(x => x.y);
        var maxY = wallPositions.Max(x => x.y);

        grid = new Grid<PathNode>(Mathf.Abs(maxX) + Mathf.Abs(minX), Mathf.Abs(maxY) + Mathf.Abs(minY), 1, originPosition: new(minX, minY), createGridObject: (g, x, y) => new(x, y));
        foreach (var wallPosition in wallPositions)
        {
            var (x, y) = grid.GetXY(wallPosition);
            var cell = grid[x, y];

            if (cell != null)
                grid[x, y].isWalkable = false;
        }

        pathfinding = new Pathfinding(grid);
    }

    #region Testing
    private Grid<PathNode> grid;
    private Pathfinding pathfinding;
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

        for (int i = 0; i < path.Count - 1; i++)
            Gizmos.DrawLine(
                grid.GetWorldPosition(path[i].x, path[i].y) + Vector2.one * (grid.CellSize / 2),
                grid.GetWorldPosition(path[i + 1].x, path[i + 1].y) + Vector2.one * (grid.CellSize / 2));
    }

    private void Update()
    {
        if (pathfinding != null && Input.GetMouseButtonDown(0))
        {
            var (toX, toY) = grid.GetXY(UtilsClass.GetMouseWorldPosition());
            path = pathfinding.FindPath(0, 0, toX, toY);
        }
    }
    #endregion

    protected List<Vector2Int> IncreaseCorridorSizeToTwo(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new();
        Vector2Int previousDirection = Vector2Int.zero;

        for (int i = 1; i < corridor.Count; i++)
        {
            Vector2Int directionFromCell = corridor[i] - corridor[i - 1];
            if (previousDirection != Vector2Int.zero && directionFromCell != previousDirection)
            {
                //Handle corner
                for (int x = -1; x < 2; x++)
                    for (int y = -1; y < 2; y++)
                        newCorridor.Add(corridor[i - 1] + new Vector2Int(x, y));

                previousDirection = directionFromCell;
            }
            else
            {
                //Add a single cell in the direction + 90 degrees
                Vector2Int newCorridorTileOffset = GetDirection90From(directionFromCell);
                newCorridor.Add(corridor[i - 1]);                         //Add normal corridor floor cell
                newCorridor.Add(corridor[i - 1] + newCorridorTileOffset); //Add an extra corridor floor cell
            }
        }
        return newCorridor;
    }

    protected List<Vector2Int> IncreaseCorridorSizeToThree(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new();
        for (int i = 1; i < corridor.Count; i++)
        {
            for (int x = -1; x < 2; x++)
                for (int y = -1; y < 2; y++)
                    newCorridor.Add(corridor[i - 1] + new Vector2Int(x, y));
        }
        return newCorridor;
    }

    private Vector2Int GetDirection90From(Vector2Int directionFromCell)
    {
        if (directionFromCell == Vector2Int.up)
            return Vector2Int.right;

        if (directionFromCell == Vector2Int.right)
            return Vector2Int.down;

        if (directionFromCell == Vector2Int.down)
            return Vector2Int.left;

        if (directionFromCell == Vector2Int.left)
            return Vector2Int.up;

        return Vector2Int.zero;
    }

    private void CreateRoomsAtDeadEnds(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloorPositions)
    {
        foreach (var position in deadEnds)
        {
            if (!roomFloorPositions.Contains(position))
            {
                var newRoomFloor = RunRandomWalk(roomGenerationParameters, position);
                roomFloorPositions.UnionWith(newRoomFloor);
            }
        }
    }

    private List<Vector2Int> GetDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new();
        foreach (var position in floorPositions)
        {
            int neighboursCount = 0;
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                if (floorPositions.Contains(position + direction))
                    neighboursCount++;
            }

            if (neighboursCount == 1)
                deadEnds.Add(position);
        }
        return deadEnds;
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new();
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);
        List<Vector2Int> roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();

        foreach (var roomPosition in roomsToCreate)
        {
            var roomFloor = RunRandomWalk(roomGenerationParameters, roomPosition);
            roomPositions.UnionWith(roomFloor);
        }
        return roomPositions;
    }

    private List<List<Vector2Int>> CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPosition = startPosition;
        potentialRoomPositions.Add(currentPosition);
        List<List<Vector2Int>> corridors = new();

        for (int i = 0; i < corridorCount; i++)
        {
            var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
            corridors.Add(corridor);

            //Picks the final position of the corridor to as the start of the next one and sets it as a potential room position
            currentPosition = corridor[^1];
            potentialRoomPositions.Add(currentPosition);
            floorPositions.UnionWith(corridor);
        }
        return corridors;
    }
}
