namespace Assets.Code.Items.Interfaces
{
    public interface IWeapon : IEquipable
    {
        float Damage { get; protected set; } //TODO: Replace with an abstraction
        float AtackSpeed { get; protected set; }
        void Atack();
    }
}
