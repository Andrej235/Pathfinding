namespace Assets.Code.Items.Interfaces
{
    public interface IRangeWeapon : IWeapon
    {
        float Range { get; protected set; }
    }
}
