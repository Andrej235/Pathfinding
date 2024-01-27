using Assets.Code.TileMap;

namespace Assets.Code.Dungeon.DTOs
{
    public record RoomDTO
    {
        public enum RoomType
        {
            Enemy = 1,
            Reward = 2,
            Special = 4,
            Shop = 8,
            Boss = 16,
        }

        public RoomType Type { get; }
        public TileMapGridDTO TileMap { get; }
    }
}
