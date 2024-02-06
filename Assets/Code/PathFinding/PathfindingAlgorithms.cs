using Assets.Code.Grid;
using Priority_Queue;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.PathFinding
{
#nullable enable
    public static class PathfindingAlgorithms
    {
        #region A*
        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        /// <summary>
        /// Initializes neighbour nodes for all nodes inside of a <see cref="Grid{T}"></see>/>
        /// </summary>
        /// <typeparam name="T">A type which is or derives from type <see cref="PathNode"/></typeparam>
        public static void InitializeNeighbours<T>(this IGrid<T> grid) where T : PathNode
        {
            List<PathNode> GetNeighbourList(PathNode node)
            {
                List<PathNode> neighbours = new();

                if (node.x - 1 >= 0) //Left
                {
                    neighbours.Add(grid[node.x - 1, node.y]);

                    if (node.y - 1 >= 0) //Down
                        neighbours.Add(grid[node.x - 1, node.y - 1]);

                    if (node.y + 1 < grid.Height) //Up
                        neighbours.Add(grid[node.x - 1, node.y + 1]);

                }
                if (node.x + 1 < grid.Width) //Right
                {
                    neighbours.Add(grid[node.x + 1, node.y]);

                    if (node.y - 1 >= 0) //Down
                        neighbours.Add(grid[node.x + 1, node.y - 1]);

                    if (node.y + 1 < grid.Height) //Up
                        neighbours.Add(grid[node.x + 1, node.y + 1]);
                }

                if (node.y - 1 >= 0) //Down
                    neighbours.Add(grid[node.x, node.y - 1]);

                if (node.y + 1 < grid.Height) //Up
                    neighbours.Add(grid[node.x, node.y + 1]);

                return neighbours;
            }

            for (int x = 0; x < grid.Width; x++)
                for (int y = 0; y < grid.Height; y++)
                    grid[x, y].neighbours = GetNeighbourList(grid[x, y]);
        }

        /// <summary>
        /// Runs AStar pathfinding algorithm on a given grid
        /// Each path node MUST already have precalculated neighbour nodes
        /// </summary>
        /// <typeparam name="T">A type which is or derives from type <see cref="PathNode"/></typeparam>
        /// <param name="grid">Grid on which the algorithm will be performed</param>
        /// <param name="startX">Start X grid coordinate</param>
        /// <param name="startY">Start Y grid coordinate</param>
        /// <param name="endX">End X grid coordinate</param>
        /// <param name="endY">End Y grid coordinate</param>
        /// <param name="depth">Number of nodes the algorithm will look at, </param>
        /// <returns>
        /// A list of pathnodes which together make up the most efficient path if the algorithm was successfull
        /// If the algorithm fails (there is no path between start and end position) returns null
        /// </returns>
        public static List<PathNode>? RunAStar<T>(this IGrid<T> grid, int startX, int startY, int endX, int endY, uint depth = uint.MaxValue) where T : PathNode
        {
            SimplePriorityQueue<PathNode> OpenListQueue = new();
            HashSet<PathNode> closedList;

            PathNode? startNode = grid[startX, startY];
            PathNode? endNode = grid[endX, endY];
            uint cycle = 0;

            if (startNode is null || endNode is null)
                return null;

            if (!startNode.isWalkable || !endNode.isWalkable)
                return null;

            OpenListQueue.Clear();
            closedList = new();

            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    PathNode pathNode = grid[x, y];
                    pathNode.gCost = int.MaxValue;
                    pathNode.cameFromNode = null;
                }
            }

            startNode.gCost = 0;
            startNode.hCost = CalculateDistanceCost(startNode, endNode);
            OpenListQueue.Enqueue(startNode, startNode.FCost);

            while (OpenListQueue.Count > 0)
            {
                if (cycle > depth)
                    return null;

                PathNode currentNode = OpenListQueue.Dequeue();
                if (currentNode == endNode)
                    return CalculatePath(endNode);
                else
                    cycle++;

                closedList.Add(currentNode);

                foreach (var neighbourNode in currentNode.neighbours)
                {
                    if (closedList.Contains(neighbourNode))
                        continue;

                    if (!neighbourNode.isWalkable)
                    {
                        closedList.Add(neighbourNode);
                        continue;
                    }

                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode); //CalculateDistanceCost returns 10 or 14
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.cameFromNode = currentNode;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);

                        if (OpenListQueue.Contains(neighbourNode))
                            OpenListQueue.UpdatePriority(neighbourNode, neighbourNode.FCost);
                        else
                            OpenListQueue.Enqueue(neighbourNode, neighbourNode.FCost);
                    }
                }
            }

            //Couldn't find a path
            return null;
        }

        private static List<PathNode> CalculatePath(PathNode node)
        {
            List<PathNode> path = new() { node };
            while (node.cameFromNode != null)
            {
                path.Add(node.cameFromNode);
                node = node.cameFromNode;
            }
            path.Reverse();
            return path;
        }

        private static int CalculateDistanceCost(PathNode a, PathNode b)
        {
            float xDistance = MathF.Abs(a.x - b.x);
            float yDistance = MathF.Abs(a.y - b.y);
            float remaining = MathF.Abs(xDistance - yDistance);
            return (int)(MOVE_DIAGONAL_COST * MathF.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining);
        }
        #endregion

        #region Breadth-first search
        public static HashSet<Vector2Int> GetReachableBFS(Vector2Int start, HashSet<Vector2Int> available, HashSet<Vector2Int> obstacles)
        {
            Queue<Vector2Int> toExplore = new();
            toExplore.Enqueue(start);

            HashSet<Vector2Int> visited = new() { start };
            HashSet<Vector2Int> reachable = new() { start };

            while (toExplore.Count > 0)
            {
                var current = toExplore.Dequeue();

                foreach (var direction in Directions.cardinalDirectionsList)
                {
                    var neighbour = current + direction;

                    if (visited.Contains(neighbour))
                        continue;

                    visited.Add(neighbour);

                    if (!available.Contains(neighbour) || obstacles.Contains(neighbour))
                        continue;

                    toExplore.Enqueue(neighbour);
                    reachable.Add(neighbour);
                }
            }

            return reachable;
        }
        #endregion
    }
}
