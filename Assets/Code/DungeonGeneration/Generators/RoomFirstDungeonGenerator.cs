using Assets.Code.DungeonGeneration.Models;
using Assets.Code.Utility;
using System.Collections.Generic;
using System.Linq;
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

        //HAS to be called AFTER corridors have been added to dungeon data
        foreach (var room in dungeonData.Rooms)
            PopulateRoomWithProps(room);

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

        return floor;
    }

    private void PopulateRoomWithProps(Room room)
    {
        PopulateFloorTilesWithProps(room, PropSO.PropPlacementType.Center);
        PopulateFloorTilesWithProps(room, PropSO.PropPlacementType.NextToTopWall);
        PopulateFloorTilesWithProps(room, PropSO.PropPlacementType.NextToRightWall);
        PopulateFloorTilesWithProps(room, PropSO.PropPlacementType.NextToBottomWall);
        PopulateFloorTilesWithProps(room, PropSO.PropPlacementType.NextToLeftWall);
        PopulateFloorTilesWithProps(room, PropSO.PropPlacementType.Corner);
    }

    private void PopulateFloorTilesWithProps(Room room, PropSO.PropPlacementType propPlacementType)
    {
        var possibleProps = parameters.propsChance.Where(x => x.Value.placementType.HasFlag(propPlacementType));
        if (possibleProps.Count() <= 0)
            return;

        foreach (var tile in room.GetTiles(propPlacementType).Except(dungeonData.Path))
        {
            if (dungeonData.Path.Contains(tile))
                continue;

            if (RNG.Chance(parameters.chanceToSpawnAProp / 100f))
            {
                var prop = possibleProps.GetByChance();

                room.PropPositions.Add(tile);
                room.PropObjects.Add(Instantiate(prop.propPrefab, new Vector3(tile.x, tile.y) + Vector3.one * .5f, Quaternion.identity, transform));

                if (prop.placeAsAGroup)
                {
                    var currentTile = tile;
                    var groupSize = RNG.Get(prop.groupMinCount - 1, prop.groupMaxCount); //min - 1 because the original one is spawned regardless, max stays the same as it is exclusive (not included in rng)

                    HashSet<Vector2Int> groupMemberPositions = new() { tile };

                    int notFoundCount = 0;
                    int maxNotFoundCount = groupSize / 3;

                    for (int i = 0; i < groupSize; i++)
                    {
                        bool foundTile = false;
                        for (int j = 0; j < RNG.Get(1, 4); j++)
                        {
                            var direction = Directions.RandomCardinalDirection;
                            var neighbourTile = currentTile + direction;

                            if (TryPlaceProp(room, neighbourTile, prop.placementType))
                            {
                                currentTile = neighbourTile;
                                groupMemberPositions.Add(neighbourTile);

                                room.PropPositions.Add(currentTile);
                                room.PropObjects.Add(Instantiate(prop.propPrefab, new Vector3(currentTile.x, currentTile.y) + Vector3.one * .5f, Quaternion.identity, transform));

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

    private bool TryPlaceProp(Room room, Vector2Int tileToPlaceOn, PropSO.PropPlacementType placementType)
    {
        if (room.PropPositions.Contains(tileToPlaceOn) || dungeonData.Path.Contains(tileToPlaceOn))
            return false;

        //TODO: Add support for bigger props
        //Go through each propplacementtype and check if the tileToPlaceOn is a part of a collection prop can be placed on

        if (placementType.HasFlag(PropSO.PropPlacementType.Center))
        {
            if (room.InnerTiles.Contains(tileToPlaceOn))
                return true;
        }

        if (placementType.HasFlag(PropSO.PropPlacementType.NextToTopWall))
        {
            if (room.TilesNextToTopWall.Contains(tileToPlaceOn))
                return true;
        }

        if (placementType.HasFlag(PropSO.PropPlacementType.NextToRightWall))
        {
            if (room.TilesNextToRightWall.Contains(tileToPlaceOn))
                return true;
        }

        if (placementType.HasFlag(PropSO.PropPlacementType.NextToBottomWall))
        {
            if (room.TilesNextToBottomWall.Contains(tileToPlaceOn))
                return true;
        }

        if (placementType.HasFlag(PropSO.PropPlacementType.NextToLeftWall))
        {
            if (room.TilesNextToLeftWall.Contains(tileToPlaceOn))
                return true;
        }

        if (placementType.HasFlag(PropSO.PropPlacementType.Corner))
        {
            if (room.CornerTiles.Contains(tileToPlaceOn))
                return true;
        }

        return false;
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
