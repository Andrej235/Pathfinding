using UnityEngine;

public class HeatMapGrid : Grid<int>
{
    public const int HEAT_MAP_MAX_VALUE = 100;
    public const int HEAT_MAP_MIN_VALUE = 0;

    public HeatMapGrid(int width, int height, int cellSize) : base(width, height, cellSize) { }

    public HeatMapGrid(int width, int height, int cellSize, Vector2 originPosition) : base(width, height, cellSize, originPosition) { }

    public override int this[int x, int y]
    {
        get => base[x, y];
        set => base[x, y] = Mathf.Clamp(value, HEAT_MAP_MIN_VALUE, HEAT_MAP_MAX_VALUE);
    }

    private void AddValue(int x, int y, int value) => this[x, y] += value;

    public void AddValue(Vector2 worldPosition, int value, int range)
    {
        (int originX, int originY) = GetXY(worldPosition);

        for (int x = 0; x < range; x++)
        {
            for (int y = 0; y < range - x; y++)
            {
                AddValue(originX + x, originY + y, value);

                if (x != 0)
                    AddValue(originX - x, originY + y, value);

                if(y != 0)
                {
                    AddValue(originX + x, originY - y, value);

                    if (x != 0)
                        AddValue(originX - x, originY - y, value);
                }
            }
        }
    }
}
