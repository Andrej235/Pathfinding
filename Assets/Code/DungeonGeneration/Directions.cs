using System.Collections.Generic;
using UnityEngine;

public static class Directions
{
    public static List<Vector2Int> cardinalDirectionsList = new()
    {
        new(0,1), //UP
        new(1,0), //RIGHT
        new(0, -1), // DOWN
        new(-1, 0) //LEFT
    };

    public static List<Vector2Int> diagonalDirectionsList = new()
    {
        new(1,1), //UP-RIGHT
        new(1,-1), //RIGHT-DOWN
        new(-1, -1), // DOWN-LEFT
        new(-1, 1) //LEFT-UP
    };

    public static List<Vector2Int> eightDirectionsList = new()
    {
        new(0,1), //UP
        new(1,1), //UP-RIGHT
        new(1,0), //RIGHT
        new(1,-1), //RIGHT-DOWN
        new(0, -1), // DOWN
        new(-1, -1), // DOWN-LEFT
        new(-1, 0), //LEFT
        new(-1, 1) //LEFT-UP

    };

    public static Vector2Int GetRandomCardinalDirection() => cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
    public static Vector2Int RandomCardinalDirection => GetRandomCardinalDirection();
}