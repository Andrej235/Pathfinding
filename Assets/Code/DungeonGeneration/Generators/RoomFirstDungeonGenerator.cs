using Assets.Code.DungeonGeneration.Models;
using Assets.Code.Utility;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomFirstDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField] private int minRoomWidth = 4;
    [SerializeField] private int minRoomHeight = 4;
    [SerializeField] protected int dungeonWidth = 20;
    [SerializeField] protected int dungeonHeight = 20;
    [SerializeField] protected int offset = 1;

    protected override IEnumerable<Vector2Int> RunProceduralGeneration()
    {
        var roomBoundsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new((Vector3Int)startPosition, new(dungeonWidth, dungeonHeight)), minRoomWidth, minRoomHeight);
        HashSet<Vector2Int> dungeonFloorPositions = CreateRooms(roomBoundsList);

        List<Vector2Int> roomCenters = new();
        foreach (var room in roomBoundsList)
            roomCenters.Add(Vector2Int.RoundToInt(room.center));

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        dungeonFloorPositions.UnionWith(corridors);
        dungeonData.Path.UnionWith(corridors);

        tileMapVisualizer.PaintFloorTiles(dungeonFloorPositions);
        return WallGenerator.CreateWalls(dungeonFloorPositions, tileMapVisualizer);
    }

    protected virtual HashSet<Vector2Int> CreateRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new();
        foreach (var roomBounds in roomsList)
        {
            HashSet<Vector2Int> roomFloor = new();
            for (int col = offset; col < roomBounds.size.x - offset; col++)
            {
                for (int row = offset; row < roomBounds.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)roomBounds.min + new Vector2Int(col, row);
                    roomFloor.Add(position);
                }
            }
            dungeonData.Rooms.Add(new(roomFloor, roomBounds.center));
            floor.UnionWith(roomFloor);
        }

        foreach (var room in dungeonData.Rooms)
            PopulateRoomWithProps(room);

        return floor;
    }

    private void PopulateRoomWithProps(Room room)
    {
        PopulateFloorTilesWithProps(room, room.InnerTiles);
    }

    private void PopulateFloorTilesWithProps(Room room, HashSet<Vector2Int> tiles)
    {
        foreach (var tile in tiles)
        {
            if (dungeonData.Path.Contains(tile))
                continue;

            if (RNG.Chance(parameters.chanceToSpawnAProp / 100))
            {
                var prop = parameters.propsChance.GetByChance();

                room.PropPositions.Add(tile);
                room.PropObjects.Add(Instantiate(prop.propPrefab, new Vector3(tile.x, tile.y) + Vector3.one * .5f, Quaternion.identity, transform));

                if (prop.placeAsAGroup)
                {
                    var currentTile = tile;
                    var groupSize = RNG.Get(prop.groupMinCount, prop.groupMaxCount);

                    HashSet<Vector2Int> groupMemberPositions = new() { tile };

                    int notFoundCount = 0;
                    int maxNotFoundCount = prop.groupMinCount / 3;

                    for (int i = 0; i < groupSize; i++)
                    {
                        bool foundTile = false;
                        for (int j = 0; j < 3; j++)
                        {
                            var direction = Directions.RandomCardinalDirection;
                            var neighbourTile = currentTile + direction;

                            if (!room.PropPositions.Contains(neighbourTile) && tiles.Contains(neighbourTile))
                            {
                                currentTile = neighbourTile;
                                room.PropPositions.Add(currentTile);
                                room.PropObjects.Add(Instantiate(prop.propPrefab, new Vector3(currentTile.x, currentTile.y) + Vector3.one * .5f, Quaternion.identity, transform));

                                groupMemberPositions.Add(neighbourTile);
                                foundTile = true;
                                break;
                            }
                        }

                        if (!foundTile)
                        {
                            if (notFoundCount < maxNotFoundCount)
                                currentTile = groupMemberPositions.GetRandomElement();
                            else
                                return;
                        }
                    }
                }
            }
        }
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
