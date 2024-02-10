using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WallGenerator
{
    public static IEnumerable<Vector2Int> CreateWalls(HashSet<Vector2Int> floorPositions, DungeonTilemapVisualizer tilemapVisualizer)
    {
        //Positions which represent holes in the floorPositions
        HashSet<Vector2Int> missingFloorPositions = new();

        var basicWallPositions = FindWallsInDirections(floorPositions, Directions.cardinalDirectionsList, ref missingFloorPositions);
        var cornerWallPositions = FindWallsInDirections(floorPositions, Directions.diagonalDirectionsList, ref missingFloorPositions);

        CreateBasicWall(tilemapVisualizer, basicWallPositions, floorPositions);
        CreateCornerWalls(tilemapVisualizer, cornerWallPositions, floorPositions);

        tilemapVisualizer.PaintFloorTiles(missingFloorPositions);
        return basicWallPositions.Union(cornerWallPositions);
    }

    private static void CreateBasicWall(DungeonTilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> basicWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in basicWallPositions)
        {
            string neighboursBinaryType = "";
            foreach (var direction in Directions.cardinalDirectionsList)
                neighboursBinaryType += floorPositions.Contains(position + direction) ? "1" : "0";

            tilemapVisualizer.PaintSingleBasicWall(position, neighboursBinaryType);
        }
    }

    private static void CreateCornerWalls(DungeonTilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> cornerWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in cornerWallPositions)
        {
            string neighboursBinaryType = "";
            foreach (var direction in Directions.eightDirectionsList)
                neighboursBinaryType += floorPositions.Contains(position + direction) ? "1" : "0";

            tilemapVisualizer.PaintSingleCornerWall(position, neighboursBinaryType);
        }
    }

    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionList, ref HashSet<Vector2Int> missingFloorPositions)
    {
        HashSet<Vector2Int> wallPositions = new();

        foreach (var position in floorPositions)
        {
            foreach (var direction in directionList)
            {
                var neighbourPosition = position + direction;
                if (!floorPositions.Contains(neighbourPosition) && !missingFloorPositions.Contains(neighbourPosition))
                {
                    var isWall = false;
                    foreach (var innerWallDirection in Directions.cardinalDirectionsList)
                    {
                        Vector2Int current = neighbourPosition + innerWallDirection;
                        if (!floorPositions.Contains(current) && !missingFloorPositions.Contains(current))
                        {
                            //If the current tile position isn't a floor, that means that neighbourPosition is a wall
                            isWall = true;
                            break;
                        }
                    }

                    if (isWall)
                        wallPositions.Add(neighbourPosition);
                    else
                        missingFloorPositions.Add(neighbourPosition);
                }
            }
        }
        return wallPositions;
    }
}
