using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomFirstRandomPathDungeonGenerator : RoomFirstDungeonGenerator
{
    [SerializeField] protected RandomWalkParametersSO randomWalkRoomGenerationParameters;

    protected override HashSet<Vector2Int> CreateRooms(List<BoundsInt> roomsList)
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

    protected HashSet<Vector2Int> RunRandomWalk(RandomWalkParametersSO parameters, Vector2Int position)
    {
        var currentPosition = position;
        HashSet<Vector2Int> floorPositions = new();
        for (int i = 0; i < parameters.iterations; i++)
        {
            var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, parameters.walkLength);
            floorPositions.UnionWith(path);

            if (parameters.startRandomlyEachIteration)
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
        }
        return floorPositions;
    }
}
