namespace Assets.Code.Items.Interfaces
{
    public interface IPotion : IConsumable
    {
        IEffect Effect { get; protected set; }
    }
}
