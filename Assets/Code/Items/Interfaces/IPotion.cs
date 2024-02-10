using Assets.Code.Entity.Effects.Buffs;

namespace Assets.Code.Items.Interfaces
{
    public interface IPotion : IConsumable
    {
        ITemporaryBuff Buff { get; }
    }
}
