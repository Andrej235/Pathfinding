using System;
using System.Collections.Generic;

namespace Assets.Code.Dungeon.DTOs
{
    [Serializable]
    public record DungeonDTO
    {
        public DungeonDTO(string name, string materialName, List<RoomDTO> possibleRooms)
        {
            Name = name;
            MaterialName = materialName;
            PossibleRooms = possibleRooms;
        }

        public string Name;
        public string MaterialName;
        public List<RoomDTO> PossibleRooms;
    }
}
