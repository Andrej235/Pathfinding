using UnityEngine;

public class GridMaster : MonoBehaviour
{
    public static GridMaster Instance { get; private set; }
    private void Awake()
    {
        Instance = this;

        Grid = new(100, 100, 7, new(-100, -50), (g, x, y) => new(x, y));
        Pathfinding = new(Grid);
    }

    public Pathfinding Pathfinding { get; private set; }
    public Grid<PathNode> Grid { get; private set; }
}
