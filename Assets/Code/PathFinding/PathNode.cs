public class PathNode
{
    public readonly int x;
    public readonly int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public bool isWalkable;

    public PathNode cameFromNode;

    public PathNode(int x, int y, bool isWalkable = true)
    {
        this.x = x;
        this.y = y;
        this.isWalkable = isWalkable;
    }

    public override string ToString() => $"{x},{y}";

    internal void CalculateFCost() => fCost = gCost + hCost;
}
