namespace Assets.Code.Items.Interfaces
{
    public interface IConsumable : IItem
    {
        float Cooldown { get; protected set; }
    }
}
