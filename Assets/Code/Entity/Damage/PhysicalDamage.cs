using Assets.Code.Entity.Effects;
using Assets.Code.Entity.Effects.Debuffs;
using System.Collections.Generic;

namespace Assets.Code.Entity.Damage
{
    public class PhysicalDamage : IDamage
    {
        private readonly float damage;
        public float Damage => damage;

        private readonly IEnumerable<IDebuff> debuffs;
        public IEnumerable<IDebuff> Debuffs => debuffs;

        public PhysicalDamage(float damage, IEnumerable<IDebuff> debuffs)
        {
            this.damage = damage;
            this.debuffs = debuffs;
        }
    }
}
