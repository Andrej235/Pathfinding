using System;

namespace Assets.Code.Grid
{
    public class OnGridCellValueChangedEventArgs<T> : EventArgs
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
}
