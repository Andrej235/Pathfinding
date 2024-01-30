namespace Assets.Code.Items.Interfaces
{
    public interface ISelfAbility : IAbility
    {
        IEffect Effect { get; }
    }
}
