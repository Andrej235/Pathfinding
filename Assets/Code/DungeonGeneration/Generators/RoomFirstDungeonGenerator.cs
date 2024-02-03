using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField] private int minRoomWidth = 4;
    [SerializeField] private int minRoomHeight = 4;
    [SerializeField] protected int dungeonWidth = 20;
    [SerializeField] protected int dungeonHeight = 20;
    [SerializeField] int offset = 1;
    [SerializeField] private bool randomWalkRooms = false;

    protected override IEnumerable<Vector2Int> RunProceduralGeneration()
    {
        var roomBoundsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new((Vector3Int)startPosition, new(dungeonWidth, dungeonHeight)), minRoomWidth, minRoomHeight);
        HashSet<Vector2Int> dungeonFloorPositions = !randomWalkRooms ? CreateSimpleRooms(roomBoundsList) : CreateRoomsUsingRandomWalk(roomBoundsList);

        List<Vector2Int> roomCenters = new();
        foreach (var room in roomBoundsList)
            roomCenters.Add(Vector2Int.RoundToInt(room.center));

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        dungeonFloorPositions.UnionWith(corridors);

        tileMapVisualizer.PaintFloorTiles(dungeonFloorPositions);
        return WallGenerator.CreateWalls(dungeonFloorPositions, tileMapVisualizer);
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new();
        foreach (var room in roomsList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }

    private HashSet<Vector2Int> CreateRoomsUsingRandomWalk(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new();
        foreach (var roomBounds in roomsList)
        {
            var roomCenter = Vector2Int.RoundToInt(roomBounds.center);
            var roomFloor = RunRandomWalk(randomWalkRoomGenerationParameters, roomCenter);

            floor.UnionWith(
                roomFloor.Where(x =>
                    x.x >= (roomBounds.min.x + offset)
                    && x.x <= (roomBounds.max.x - offset)
                    && x.y >= (roomBounds.min.y + offset)
                    && x.y <= (roomBounds.max.y - offset)
                    )
                );
        }
        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int start, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new();
        var position = start;
        corridor.Add(position);

        while (position.y != destination.y)
        {
            position += destination.y > position.y ? Vector2Int.up : Vector2Int.down;
            corridor.Add(position);

            //Increases the width of corridors to 3
            corridor.Add(position + Vector2Int.right);
            corridor.Add(position + Vector2Int.left);
        }

        while (position.x != destination.x)
        {
            position += destination.x > position.x ? Vector2Int.right : Vector2Int.left;
            corridor.Add(position);

            //Increases the width of corridors to 3
            corridor.Add(position + Vector2Int.up);
            corridor.Add(position + Vector2Int.down);
        }

        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;

        foreach (var room in roomCenters)
        {
            float currentDistance = Vector2.Distance(room, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = room;
            }
        }
        return closest;
    }
}
