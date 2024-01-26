using System;
using UnityEngine;

namespace Assets.Code.TileMap
{
    [Serializable]
    public record TileMapGridDTO
    {
        public int GridWidth;
        public int GridHeight;
        public float GridCellSize;

        public Vector2[,] GridUV00s;
        public Vector2[,] GridUV11s;
        public bool[,] GridIsWalkables;

        public TileMapGridDTO(int gridWidth, int gridHeight, float gridCellSize, Vector2[,] gridUV00s, Vector2[,] gridUV11s, bool[,] gridIsWalkables)
        {
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            GridCellSize = gridCellSize;
            GridUV00s = gridUV00s;
            GridUV11s = gridUV11s;
            GridIsWalkables = gridIsWalkables;
        }
    }
}
