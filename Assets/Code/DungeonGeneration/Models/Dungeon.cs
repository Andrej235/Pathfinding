using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.DungeonGeneration.Models
{
    public class Dungeon
    {
        public List<Room> Rooms { get; private set; } = new();
        public HashSet<Vector2Int> Path { get; private set; } = new();

        internal void Reset()
        {
            Rooms = new();
            Path = new();
        }
    }
}
