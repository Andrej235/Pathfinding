namespace Assets.Code.Utility
{
    public interface IChance<T>
    {
        public T Value { get; set; }
        public float Chance { get; set; }
    }
}