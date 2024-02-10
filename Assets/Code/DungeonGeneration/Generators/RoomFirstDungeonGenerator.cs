using Assets.Code.DungeonGeneration.Models;
using Assets.Code.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

#nullable enable
public class RoomFirstDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField] private int minRoomWidth = 4;
    [SerializeField] private int minRoomHeight = 4;
    [SerializeField] protected int dungeonWidth = 20;
    [SerializeField] protected int dungeonHeight = 20;
    [SerializeField] protected int offset = 1;

    protected override IEnumerable<Vector2Int> RunProceduralGeneration()
    {
        if (tileMapVisualizer is null)
            return new List<Vector2Int>();

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

        return floor;
    }

    #region Props
    protected override void PlaceProps(Room room)
    {
        PopulateFloorTilesWithProps(room, PropSO.PropPlacementType.Corner);
        PopulateFloorTilesWithProps(room, PropSO.PropPlacementType.NextToTopWall);
        PopulateFloorTilesWithProps(room, PropSO.PropPlacementType.NextToRightWall);
        PopulateFloorTilesWithProps(room, PropSO.PropPlacementType.NextToBottomWall);
        PopulateFloorTilesWithProps(room, PropSO.PropPlacementType.NextToLeftWall);
        PopulateFloorTilesWithProps(room, PropSO.PropPlacementType.Inner);
    }

    private void PopulateFloorTilesWithProps(Room room, PropSO.PropPlacementType propPlacementType)
    {
        if (parameters == null)
            return;

        var possibleRoomParameters = parameters.roomParameters.Where(x => x.Type == room.Type);

        var possibleProps = parameters.propsChance
            .Union(possibleRoomParameters.Any() ? possibleRoomParameters.GetByChance().SpecificRoomPropsChance : new())
            .Where(x => x.Value.placementType.HasFlag(propPlacementType));

        if (possibleProps.Count() <= 0)
            return;

        foreach (var tile in room.GetTiles(propPlacementType).Except(dungeonData.Path))
        {
            if (room.PropPositions.Contains(tile))
                continue;

            if (RNG.Chance(parameters.chanceToSpawnAProp / 100f))
                PlaceProp(room, tile, possibleProps.GetByChance());
        }
    }

    private void PlaceProp(Room room, Vector2Int originTile, PropSO prop)
    {
        if (prop.placeAsAGroup)
            PlacePropAsGroup(room, originTile, prop);
        else
            PlaceSingleProp(room, originTile, prop);
    }

    private bool PlacePropAsGroup(Room room, Vector2Int originTile, PropSO prop)
    {
        if (!PlaceSingleProp(room, originTile, prop))
            return false;

        var currentoriginTile = originTile;
        var groupSize = RNG.Get(prop.groupMinCount - 1, prop.groupMaxCount); //min - 1 because the original one is spawned regardless, max stays the same as it is exclusive (not included in rng)

        HashSet<Vector2Int> groupMemberPositions = new() { originTile };

        int notFoundCount = 0;
        int maxNotFoundCount = groupSize / 3;

        for (int i = 0; i < groupSize; i++)
        {
            bool foundoriginTile = false;
            for (int j = 0; j < RNG.Get(1, 4); j++)
            {
                var direction = Directions.RandomCardinalDirection * Math.Min(prop.PropSize.x, prop.PropSize.y);
                var neighbouroriginTile = currentoriginTile + direction;

                var success = PlaceSingleProp(room, neighbouroriginTile, prop);
                if (success)
                {
                    foundoriginTile = true;
                    currentoriginTile = neighbouroriginTile;
                    groupMemberPositions.Add(neighbouroriginTile);
                    break;
                }
            }

            if (!foundoriginTile)
            {
                if (notFoundCount < maxNotFoundCount)
                    currentoriginTile = groupMemberPositions.GetRandomElement();
                else
                    break;
            }
        }
        return true;
    }

    private bool PlaceSingleProp(Room room, Vector2Int originTile, PropSO prop)
    {
        if (!TryPlaceProp(room, originTile, prop))
            return false;

        if (prop.PropSize == Vector2Int.one)
        {
            room.PropPositions.Add(originTile);
            room.PropObjects.Add(Instantiate(prop.propPrefab, new Vector3(originTile.x + .5f, originTile.y + .5f), Quaternion.identity, transform));
            return true;
        }
        Vector3 offset;

        switch (prop.origin)
        {
            case PropSO.PropOrigin.TopLeft:
                offset = new Vector3(prop.PropSize.x * .5f, 0);

                for (int x = 0; x < prop.PropSize.x; x++)
                    for (int y = 0; y < prop.PropSize.y; y++)
                        room.PropPositions.Add(originTile + new Vector2Int(x, -y));
                break;

            case PropSO.PropOrigin.TopRight:
                offset = new Vector3(0, 0);

                for (int x = 0; x < prop.PropSize.x; x++)
                    for (int y = 0; y < prop.PropSize.y; y++)
                        room.PropPositions.Add(originTile + new Vector2Int(-x, -y));
                break;

            case PropSO.PropOrigin.BottomLeft:
                offset = new Vector3(prop.PropSize.x * .5f, prop.PropSize.y * .5f);

                for (int x = 0; x < prop.PropSize.x; x++)
                    for (int y = 0; y < prop.PropSize.y; y++)
                        room.PropPositions.Add(originTile + new Vector2Int(x, y));
                break;

            case PropSO.PropOrigin.BottomRight:
                offset = new Vector3(0, prop.PropSize.y * .5f);

                for (int x = 0; x < prop.PropSize.x; x++)
                    for (int y = 0; y < prop.PropSize.y; y++)
                        room.PropPositions.Add(originTile + new Vector2Int(-x, y));
                break;

            default:
                throw new NotSupportedException();
        }

        room.PropObjects.Add(Instantiate(prop.propPrefab, (Vector3Int)originTile + offset, Quaternion.identity, transform));
        return true;
    }

    private bool TryPlaceProp(Room room, Vector2Int tileToPlaceOn, PropSO prop)
    {
        if (room.PropPositions.Contains(tileToPlaceOn) || dungeonData.Path.Contains(tileToPlaceOn))
            return false;

        PropSO.PropPlacementType placementType = prop.placementType;

        //Go through each PropSO.PropPlacementType and check if the tileToPlaceOn is a part of a collection which given prop can be placed on
        return ((placementType.HasFlag(PropSO.PropPlacementType.Inner) && room.InnerTiles.Contains(tileToPlaceOn))
            || (placementType.HasFlag(PropSO.PropPlacementType.NextToTopWall) && room.TilesNextToTopWall.Contains(tileToPlaceOn))
            || (placementType.HasFlag(PropSO.PropPlacementType.NextToRightWall) && room.TilesNextToRightWall.Contains(tileToPlaceOn))
            || (placementType.HasFlag(PropSO.PropPlacementType.NextToBottomWall) && room.TilesNextToBottomWall.Contains(tileToPlaceOn))
            || (placementType.HasFlag(PropSO.PropPlacementType.NextToLeftWall) && room.TilesNextToLeftWall.Contains(tileToPlaceOn))
            || (placementType.HasFlag(PropSO.PropPlacementType.Corner) && room.CornerTiles.Contains(tileToPlaceOn)))
            && TryPlacePropBasedOnItsSize(room, tileToPlaceOn, prop);
    }

    private bool TryPlacePropBasedOnItsSize(Room room, Vector2Int originTile, PropSO prop)
    {
        if (prop.PropSize == Vector2Int.one)
            return true;

        var availableTiles = room.Floor.Except(room.PropPositions).Except(dungeonData.Path);

        switch (prop.origin)
        {
            case PropSO.PropOrigin.TopLeft:
                for (int x = 0; x < prop.PropSize.x; x++)
                    for (int y = 0; y < prop.PropSize.y; y++)
                        if (!availableTiles.Contains(originTile + new Vector2Int(x, -y)))
                            return false;
                break;

            case PropSO.PropOrigin.TopRight:
                for (int x = 0; x < prop.PropSize.x; x++)
                    for (int y = 0; y < prop.PropSize.y; y++)
                        if (!availableTiles.Contains(originTile + new Vector2Int(-x, -y)))
                            return false;
                break;

            case PropSO.PropOrigin.BottomLeft:
                for (int x = 0; x < prop.PropSize.x; x++)
                    for (int y = 0; y < prop.PropSize.y; y++)
                        if (!availableTiles.Contains(originTile + new Vector2Int(x, y)))
                            return false;
                break;

            case PropSO.PropOrigin.BottomRight:
                for (int x = 0; x < prop.PropSize.x; x++)
                    for (int y = 0; y < prop.PropSize.y; y++)
                        if (!availableTiles.Contains(originTile + new Vector2Int(-x, y)))
                            return false;
                break;

            default:
                break;
        }

        return true;
    }
    #endregion

    #region Enemies
    protected override void SpawnEnemies(Room room)
    {
        if (parameters == null)
            return;

        var roomParameters = parameters.roomParameters.FirstOrDefault(x => x.Type == room.Type);
        if (roomParameters is null)
            return;

        var possibleEnemies = parameters.enemiesChance.Where(x => room.Type.HasFlag(x.RoomType)).Union(roomParameters.SpecificRoomEnemies);
        if (!possibleEnemies.Any())
            return;

        int enemiesPerRoom = RNG.Get(roomParameters.MinimumNumberOfEnemies, roomParameters.MaximumNumberOfEnemies);
        var enemyPositions = room.TilesAccessibleFromPath.Shuffle().Take(enemiesPerRoom);

        foreach (var enemyPosition in enemyPositions)
        {
            var enemyToSpawn = possibleEnemies.GetByChance();
            room.EnemyObjects.Add(Instantiate(enemyToSpawn, new Vector3(enemyPosition.x, enemyPosition.y) + Vector3.one * .5f, Quaternion.identity, transform));
        }
    }
    #endregion

    #region Corridor generation
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
    #endregion
}
