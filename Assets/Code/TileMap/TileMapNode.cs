using UnityEngine;

public class TileMapNode : PathNode
{
    //Add NBT-like Data?
    public TileMapNode(int x, int y, Vector2 uv00, Vector2 uv11, bool isWalkable) : base(x, y, isWalkable)
    {
        UV00 = uv00;
        UV11 = uv11;
    }

    public Vector2 UV00 { get; set; }
    public Vector2 UV11 { get; set; }
}