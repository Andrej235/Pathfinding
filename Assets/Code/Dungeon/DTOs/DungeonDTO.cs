using System;
using System.Collections.Generic;

namespace Assets.Code.Dungeon.DTOs
{
    [Serializable]
    public record DungeonDTO
    {
        public DungeonDTO(string name, int materialId, List<RoomDTO> possibleRooms)
        {
            Name = name;
            MaterialId = materialId;
            PossibleRooms = possibleRooms;
        }

        public string Name;
        public int MaterialId;
        public List<RoomDTO> PossibleRooms;
    }
}
