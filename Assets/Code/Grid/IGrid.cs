using System;
using UnityEngine;

namespace Assets.Code.Grid
{
    public interface IGrid<out T>
    {
        public int Width { get; }
        public int Height { get; }
        public float CellSize { get; }
        T[,] GridArray { get; }

        public event EventHandler<OnGridCellValueChangedEventArgs> OnCellValueChanged;
        public void RaiseOnCellValueChangedEvent(int x, int y);

        T this[int x, int y] { get; }
        public T GetValue(Vector2 worldPosition);

        public (int x, int y) GetXY(Vector2 worldPosition);
        public Vector2 GetWorldPosition(int x, int y);
    }
}
