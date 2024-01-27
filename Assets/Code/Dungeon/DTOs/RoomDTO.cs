using Assets.Code.TileMap;
using System;

namespace Assets.Code.Dungeon.DTOs
{
    [Serializable]
    public record RoomDTO
    {
        public RoomDTO(RoomType type, TileMapGridDTO tileMap)
        {
            Type = type;
            TileMap = tileMap;
        }

        public RoomDTO() { }

        [Serializable]
        public enum RoomType
        {
            Enemy = 1,
            Reward = 2,
            Special = 4,
            Shop = 8,
            Boss = 16,
        }

        public RoomType Type;
        public TileMapGridDTO TileMap;
    }
}
