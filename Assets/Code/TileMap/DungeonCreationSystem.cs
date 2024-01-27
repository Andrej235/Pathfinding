using Assets.Code.Dungeon.DTOs;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Code.TileMap
{
    public class DungeonCreationSystem : MonoBehaviour
    {
        public static string DungeonsJSONFolderPath => Path.Combine(Directory.GetCurrentDirectory(), @"Assets\JSONFiles\");

        public DungeonDTO dto;

        public TileMapDrawingSystem TileMapDrawingSystem { get; private set; }
        public void InitializeTileMapDrawingSystem()
        {
            bool hasTileMapDrawingSystem = TryGetComponent(out TileMapDrawingSystem tileMapDrawingSystem);
            TileMapDrawingSystem = hasTileMapDrawingSystem ? tileMapDrawingSystem : gameObject.AddComponent<TileMapDrawingSystem>();
        }

        public static IEnumerable<string> GetDungeonNames() => Directory.GetFiles(DungeonsJSONFolderPath, "DungeonPreset_*.json").Select(x => x.Replace(DungeonsJSONFolderPath + "DungeonPreset_", "").Replace(".json", ""));

        public static string ConvertNameToFileName(string name) => $"DungeonPreset_{name}.json";
    }
}