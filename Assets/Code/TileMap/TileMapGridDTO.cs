using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Code.TileMap
{
    [Serializable]
    public record TileMapGridDTO
    {
        public int GridWidth;
        public int GridHeight;
        public float GridCellSize;

        [SerializeField]public List<Vector2> GridUV00s;
        [SerializeField] public List<Vector2> GridUV11s;
        [SerializeField] public List<bool> GridIsWalkables;

        public TileMapGridDTO(int gridWidth, int gridHeight, float gridCellSize, List<List<Vector2>> gridUV00s, List<List<Vector2>> gridUV11s, List<List<bool>> gridIsWalkables)
        {
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            GridCellSize = gridCellSize;
            GridUV00s = gridUV00s.SelectMany(x => x).ToList();
            GridUV11s = gridUV11s.SelectMany(x => x).ToList();
            GridIsWalkables = gridIsWalkables.SelectMany(x => x).ToList();
        }
    }
}
