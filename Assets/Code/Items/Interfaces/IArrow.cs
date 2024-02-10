using Assets.Code.Entity.Effects.Debuffs;

namespace Assets.Code.Items.Interfaces
{
#nullable enable
    public interface IArrow : IItem
    {
        float Damage { get; }
        IDebuff? Debuff { get; }
    }
}
