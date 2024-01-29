namespace Assets.Code.Items.Interfaces
{
    public interface IMagicWeapon : IWeapon
    {
        float ManaCost { get; protected set; }
        float Range { get; protected set; }
    }
}
