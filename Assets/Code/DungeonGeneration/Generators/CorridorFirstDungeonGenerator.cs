using Assets.Code.Grid;
using Assets.Code.PathFinding;
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

    protected override IEnumerable<Vector2Int> RunProceduralGeneration()
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
        return WallGenerator.CreateWalls(floorPositions, tileMapVisualizer);
    }

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
                var newRoomFloor = RunRandomWalk(randomWalkRoomGenerationParameters, position);
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
            foreach (var direction in Directions.cardinalDirectionsList)
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
            var roomFloor = RunRandomWalk(randomWalkRoomGenerationParameters, roomPosition);
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
