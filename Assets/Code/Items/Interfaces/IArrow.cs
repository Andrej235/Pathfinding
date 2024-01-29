namespace Assets.Code.Items.Interfaces
{
#nullable enable
    public interface IArrow : IItem
    {
        float Damage { get; protected set; }
        IEffect? Effect { get; protected set; }
    }
}
