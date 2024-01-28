using System.Collections.Generic;
using UnityEngine;

internal static class Directions
{
    public readonly static List<Vector2Int> cardinalDirectionsList = new()
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left,
    };

    /// <summary>
    /// Generates a random cardinal direction every time it is read
    /// </summary>
    public static Vector2Int RandomCardinalDirection => cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
}