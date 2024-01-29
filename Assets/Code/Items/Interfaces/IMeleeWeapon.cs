namespace Assets.Code.Items.Interfaces
{
    public interface IMeleeWeapon : IWeapon
    {
        float HitWidth { get; protected set; }
    }
}
