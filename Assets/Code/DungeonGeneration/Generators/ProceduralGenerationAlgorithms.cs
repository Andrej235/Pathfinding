using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerationAlgorithms
{
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLenght)
    {
        HashSet<Vector2Int> path = new() { startPosition };
        var previousPosition = startPosition;

        for (int i = 0; i < walkLenght; i++)
        {
            var newPosition = previousPosition + Directions.RandomCardinalDirection;
            path.Add(newPosition);
            previousPosition = newPosition;
        }
        return path;
    }

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int corridorLength)
    {
        List<Vector2Int> corridor = new() { startPosition };
        var direction = Directions.RandomCardinalDirection;
        var currentPosition = startPosition;

        for (int i = 0; i < corridorLength; i++)
        {
            currentPosition += direction;
            corridor.Add(currentPosition);
        }
        return corridor;
    }

    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        Queue<BoundsInt> roomsQueue = new();
        List<BoundsInt> roomsList = new();

        roomsQueue.Enqueue(spaceToSplit);
        while (roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();
            if (room.size.x <= minWidth || room.size.y < minHeight)
                continue;

            if (Random.value < .5f)
            {
                if (room.size.y >= minHeight * 2)
                    SplitHorizontally(room, roomsQueue);
                else if (room.size.x >= minWidth * 2)
                    SplitVertically(room, roomsQueue);
                else
                    roomsList.Add(room);
            }
            else
            {
                if (room.size.x >= minWidth * 2)
                    SplitVertically(room, roomsQueue);
                else if (room.size.y >= minHeight * 2)
                    SplitHorizontally(room, roomsQueue);
                else
                    roomsList.Add(room);
            }
        }

        return roomsList;
    }

    private static void SplitVertically(BoundsInt room, Queue<BoundsInt> roomsQueue)
    {
        var xSplit = Random.Range(1, room.size.x);
        BoundsInt room1 = new(room.min, new(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new(new(room.min.x + xSplit, room.min.y, room.min.z), new(room.size.x - xSplit, room.size.y, room.size.z));

        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    static void SplitHorizontally(BoundsInt room, Queue<BoundsInt> roomsQueue)
    {
        var ySplit = Random.Range(1, room.size.y);
        BoundsInt room1 = new(room.min, new(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new(new(room.min.x, room.min.y + ySplit, room.min.z), new(room.size.x, room.size.y - ySplit, room.size.z));

        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }
}
