using System.Collections.Generic;
using UnityEngine;
using Assets.Code.Grid;

public class GridMaster : MonoBehaviour
{
    public static GridMaster Instance { get; private set; }
    private void Awake()
    {
        Instance = this;

        Grid = new(50, 25, 0.7f, new(-20, -10), (g, x, y) => new(x, y));
        Pathfinding = new(Grid);
    }

    public Pathfinding Pathfinding { get; private set; }
    public Grid<PathNode> Grid { get; private set; }



    private readonly List<GameObject> Colliders = new();
    public void CreateColliders()
    {
        Vector2 quadSize = new(Grid.CellSize, Grid.CellSize);
        for (int x = 0; x < Pathfinding.Grid.Width; x++)
        {
            for (int y = 0; y < Pathfinding.Grid.Height; y++)
            {
                if (Pathfinding.Grid[x, y].isWalkable)
                    continue;

                var newColliderObj = new GameObject
                {
                    name = $"squareCollider",
                };
                newColliderObj.transform.position = Pathfinding.Grid.GetWorldPosition(x, y) + quadSize * .5f;
                newColliderObj.transform.parent = transform;
                var newCollider = newColliderObj.AddComponent<BoxCollider2D>();
                newCollider.size = quadSize;

                Colliders.Add(newColliderObj);
            }
        }
    }
}
