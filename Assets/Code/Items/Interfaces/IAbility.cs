namespace Assets.Code.Items.Interfaces
{
    public interface IAbility : IEquipable
    {
        float Cooldown { get; }
    }
}
