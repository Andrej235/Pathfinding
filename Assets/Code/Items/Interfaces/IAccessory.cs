using Assets.Code.Entity.Effects.Buffs;

namespace Assets.Code.Items.Interfaces
{
    public interface IAccessory : IEquipable
    {
        IBuff Buff { get; }
    }
}
