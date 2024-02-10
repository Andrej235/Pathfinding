using Assets.Code.Entity.Effects.Buffs;

namespace Assets.Code.Items.Interfaces
{
    public interface ISelfAbility : IAbility
    {
        IBuff Effect { get; }
    }
}
