using System;

namespace Assets.Code.Grid
{
    public class OnGridCellValueChangedEventArgs : EventArgs
    {
        public OnGridCellValueChangedEventArgs(int x, int y, object newValue)
        {
            X = x;
            Y = y;
            NewValue = newValue;
        }

        public int X { get; }
        public int Y { get; }
        public object NewValue { get; }
    }
}
