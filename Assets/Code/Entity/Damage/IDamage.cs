using Assets.Code.Entity.Effects.Debuffs;
using System.Collections.Generic;

namespace Assets.Code.Entity.Damage
{
    public interface IDamage
    {
        float Damage { get; }
        IEnumerable<IDebuff> Debuffs { get; }
    }
}
