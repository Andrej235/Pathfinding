namespace Assets.Code.Items.Interfaces
{
    public interface ITargetAbility : IAbility
    {
        float Damage { get; } //TODO: replace with abstraction
        float Range { get; }
    }
}
