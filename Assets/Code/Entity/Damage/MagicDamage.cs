using Assets.Code.Entity.Effects;
using Assets.Code.Entity.Effects.Debuffs;
using System.Collections.Generic;

namespace Assets.Code.Entity.Damage
{
    public class MagicDamage : IDamage
    {
        public float Damage => throw new System.NotImplementedException();

        public IEnumerable<IDebuff> Debuffs => throw new System.NotImplementedException();
    }
}
