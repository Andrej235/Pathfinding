using Assets.Code.Grid;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Code.PathFinding
{
    public static class DungeonGridGenerator
    {
        public static Grid<PathNode> GeneratePathNodeGrid(this IEnumerable<Vector2Int> wallPositions)
        {
            if (wallPositions is null || !wallPositions.Any())
                return new(0, 0, 0);

            var minX = wallPositions.Min(x => x.x);
            var maxX = wallPositions.Max(x => x.x);
            var minY = wallPositions.Min(x => x.y);
            var maxY = wallPositions.Max(x => x.y);

            var grid = new Grid<PathNode>(Mathf.Abs(maxX) + Mathf.Abs(minX) + 2, Mathf.Abs(maxY) + Mathf.Abs(minY) + 2, 1, originPosition: new(minX, minY), createGridObject: (g, x, y) => new(x, y));
            foreach (var wallPosition in wallPositions)
            {
                var (x, y) = grid.GetXY(wallPosition);
                var cell = grid[x, y];

                if (cell != null)
                    grid[x, y].isWalkable = false;
            }
            return grid;
        }
    }
}
