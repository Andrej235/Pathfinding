using System;
using UnityEngine;

public class Grid<T>
{
    public class OnGridCellValueChangedEventArgs : EventArgs
    {
        public OnGridCellValueChangedEventArgs(int x, int y, T newValue)
        {
            X = x;
            Y = y;
            NewValue = newValue;
        }

        public int X { get; }
        public int Y { get; }
        public T NewValue { get; }
    }
    public event EventHandler<OnGridCellValueChangedEventArgs> OnCellValueChanged;

    public int Width { get; }
    public int Height { get; }
    public float CellSize { get; }

    private readonly T[,] gridArray;
    private readonly Vector2 originPosition;

    public Grid(int width, int height, float cellSize) : this(width, height, cellSize, Vector2.zero, (_, _, _) => default) { }

    public Grid(int width, int height, float cellSize, Vector2 originPosition) : this(width, height, cellSize, originPosition, (_, _, _) => default) { }

    public Grid(int width, int height, float cellSize, Func<Grid<T>, int, int, T> createGridObject) : this(width, height, cellSize, Vector2.zero, createGridObject) { }

    public Grid(int width, int height, float cellSize, Vector2 originPosition, Func<Grid<T>, int, int, T> createGridObject)
    {
        Width = width;
        Height = height;
        CellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new T[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                gridArray[x, y] = createGridObject(this, x, y);
    }

    public Vector2 GetWorldPosition(int x, int y) => new Vector2(x, y) * CellSize + originPosition;
    public (int x, int y) GetXY(Vector2 worldPosition) => new(Mathf.FloorToInt((worldPosition - originPosition).x / CellSize), Mathf.FloorToInt((worldPosition - originPosition).y / CellSize));

    public void SetValue(Vector2 worldPosition, T value)
    {
        (int x, int y) = GetXY(worldPosition);
        this[x, y] = value;
    }

    public T GetValue(Vector2 worldPosition)
    {
        (int x, int y) = GetXY(worldPosition);
        return this[x, y];
    }

    public virtual T this[int x, int y]
    {
        get
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return default;

            return gridArray[x, y];
        }

        set
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return;

            gridArray[x, y] = value;
            OnCellValueChanged?.Invoke(this, new(x, y, value));
        }
    }

    public void RaiseOnCellValueChangedEvent(int x, int y) => OnCellValueChanged?.Invoke(this, new(x, y, this[x, y]));
}
