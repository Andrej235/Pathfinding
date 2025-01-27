using Assets.Code.PathFinding;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.DungeonGeneration.Models
{
    public class Room
    {
        public enum RoomType : uint
        {
            None = 0,
            Start = 1 << 0,
            Enemy = 1 << 1,
            Treassure = 1 << 2,
            Special = 1 << 3,
            Boss = 1 << 4,
            Everything = uint.MaxValue,
        }
        public RoomType Type { get; private set; }

        public Room(HashSet<Vector2Int> floor, Vector2 roomCenter)
        {
            Floor = floor;
            RoomCenter = roomCenter;

            foreach (var floorTile in floor)
            {
                var neighbours = 4;

                if (!floor.Contains(floorTile + Vector2Int.up))
                {
                    TilesNextToTopWall.Add(floorTile);
                    neighbours--;
                }

                if (!floor.Contains(floorTile + Vector2Int.right))
                {
                    TilesNextToRightWall.Add(floorTile);
                    neighbours--;
                }

                if (!floor.Contains(floorTile + Vector2Int.down))
                {
                    TilesNextToBottomWall.Add(floorTile);
                    neighbours--;
                }

                if (!floor.Contains(floorTile + Vector2Int.left))
                {
                    TilesNextToLeftWall.Add(floorTile);
                    neighbours--;
                }

                if (neighbours == 4)
                    InnerTiles.Add(floorTile);
                else if (neighbours <= 2)
                    CornerTiles.Add(floorTile);
            }

            TilesNextToTopWall.ExceptWith(CornerTiles);
            TilesNextToRightWall.ExceptWith(CornerTiles);
            TilesNextToBottomWall.ExceptWith(CornerTiles);
            TilesNextToLeftWall.ExceptWith(CornerTiles);

            Type = RoomType.None;
        }

        public Vector2 RoomCenter { get; }
        public HashSet<Vector2Int> Floor { get; } = new();

        public HashSet<Vector2Int> TilesNextToTopWall { get; } = new();
        public HashSet<Vector2Int> TilesNextToRightWall { get; } = new();
        public HashSet<Vector2Int> TilesNextToBottomWall { get; } = new();
        public HashSet<Vector2Int> TilesNextToLeftWall { get; } = new();
        public HashSet<Vector2Int> CornerTiles { get; } = new();
        public HashSet<Vector2Int> InnerTiles { get; } = new();

        public HashSet<Vector2Int> PropPositions { get; set; } = new();
        public List<GameObject> PropObjects { get; set; } = new();

        public List<GameObject> EnemyObjects { get; set; } = new();

        public HashSet<Vector2Int> TilesAccessibleFromPath { get; set; } = new();

        private DungeonParametersSO.RoomParameters parameters;
        public DungeonParametersSO.RoomParameters Parameters
        {
            get => parameters;
            set
            {
                parameters = value;
                Type = value.Type;
            }
        }

        public HashSet<Vector2Int> GetTiles(PropSO.PropPlacementType propPlacementType) => propPlacementType switch
        {
            PropSO.PropPlacementType.Inner => InnerTiles,
            PropSO.PropPlacementType.NextToTopWall => TilesNextToTopWall,
            PropSO.PropPlacementType.NextToRightWall => TilesNextToRightWall,
            PropSO.PropPlacementType.NextToBottomWall => TilesNextToBottomWall,
            PropSO.PropPlacementType.NextToLeftWall => TilesNextToLeftWall,
            PropSO.PropPlacementType.Corner => CornerTiles,
            _ => new(),
        };

        public void UpdateTilesAccessibleFromPath() => TilesAccessibleFromPath = PathfindingAlgorithms.GetReachableBFS(Vector2Int.RoundToInt(RoomCenter), InnerTiles, PropPositions);
    }
}
