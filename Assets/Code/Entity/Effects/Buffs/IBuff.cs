namespace Assets.Code.Entity.Effects.Buffs
{
    public interface IBuff
    {
        Stat Stat { get; }
        float Amount { get; }
        bool IsAbsolute { get; }
    }

    public interface ITemporaryBuff : IBuff
    {
        float Duration { get; }
    }
}
