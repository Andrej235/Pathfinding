namespace Assets.Code.Utility
{
    public static class ArrayExtensions
    {
        public static int GetIndexOf<T>(this T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(value))
                    return i;
            }

            return -1;
        }
    }
}
