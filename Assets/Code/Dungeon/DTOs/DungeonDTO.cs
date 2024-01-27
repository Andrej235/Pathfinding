using System.Collections.Generic;

namespace Assets.Code.Dungeon.DTOs
{
    public record DungeonDTO
    {
        public DungeonDTO(string name, int materialId, List<RoomDTO> possibleRooms)
        {
            Name = name;
            MaterialId = materialId;
            PossibleRooms = possibleRooms;
        }

        public string Name { get; set; }
        public int MaterialId { get; set; }
        public List<RoomDTO> PossibleRooms { get; }
    }
}
