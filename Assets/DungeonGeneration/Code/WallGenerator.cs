using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator
{
    public static HashSet<Vector2Int> CreateWalls(HashSet<Vector2Int> floorPositions, TileMapVisualizer tileMapVisualizer)
    {
        var wallPositions = FindWallsInDirections(floorPositions, Directions.directionsList);

        foreach (var position in wallPositions)
            tileMapVisualizer.PainSingleBasicWall(position);

        return wallPositions;
    }

    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions, List<Vector2Int> directions)
    {
        HashSet<Vector2Int> wallPositions = new();
        foreach (var position in floorPositions)
        {
            foreach (var direction in directions)
            {
                var neighbourPosition = position + direction;
                if (!floorPositions.Contains(neighbourPosition))
                    wallPositions.Add(neighbourPosition);
            }
        }
        return wallPositions;
    }
}
