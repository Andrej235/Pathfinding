using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Code.Grid;
using Assets.Code.PathFinding;

#nullable enable
public class Pathfinding
{
    public IGrid<PathNode> Grid { get; }

    public Pathfinding(IGrid<PathNode> grid)
    {
        grid.InitializeNeighbours();
        Grid = grid;
    }

    ///<inheritdoc cref="PathfindingAlgorithms.RunAStar{T}(IGrid{T}, int, int, int, int, uint)"/>
    public List<PathNode>? FindPath(int startX, int startY, int endX, int endY, uint depth = uint.MinValue) => Grid.RunAStar(startX, startY, endX, endY, depth);

    public List<PathNode>? FindPath(Vector2 worldStartPos, Vector2 worldEndPos, uint depth = uint.MaxValue)
    {
        (int startX, int startY) = Grid.GetXY(worldStartPos);
        (int endX, int endY) = Grid.GetXY(worldEndPos);

        return FindPath(startX, startY, endX, endY, depth);
    }
}
