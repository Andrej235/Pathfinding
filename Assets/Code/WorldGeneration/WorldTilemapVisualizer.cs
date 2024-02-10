using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Code.WorldGeneration
{
    public class WorldTilemapVisualizer
    {
        private readonly Tilemap tilemap;

        public WorldTilemapVisualizer(Tilemap tilemap)
        {
            this.tilemap = tilemap;
        }

        public void PaintTiles(IEnumerable<Vector2Int> positions, TileBase tile)
        {
            foreach (var position in positions)
                PaintSingleTile(tilemap, tile, position);
        }

        private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position) => tilemap.SetTile(tilemap.WorldToCell((Vector3Int)position), tile);

        public void Clear() => tilemap.ClearAllTiles();
    }
}
