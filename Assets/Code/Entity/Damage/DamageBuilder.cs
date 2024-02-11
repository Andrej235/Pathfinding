using Assets.Code.Entity.Effects;
using Assets.Code.Entity.Effects.Buffs;
using Assets.Code.Entity.Effects.Debuffs;
using Assets.Code.Utility;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Code.Entity.Damage
{
    public class DamageBuilder
    {
        public static DamageBuilder StartBuiding(float damage) => new(damage);

        private float damage;
        private IEnumerable<IBuff> buffs;
        private IEnumerable<IChance<IDebuff>> debuffs;
        private DamageBuilder(float damage)
        {
            this.damage = damage;
            buffs = new List<IBuff>();
            debuffs = new List<IChance<IDebuff>>();
        }

        public DamageBuilder AddBuffs(IEnumerable<IBuff> buffs)
        {
            this.buffs = this.buffs.Union(buffs);
            return this;
        }

        public DamageBuilder AddDebuffs(IEnumerable<IChance<IDebuff>> debuffs)
        {
            this.debuffs = this.debuffs.Union(debuffs);
            return this;
        }

        public PhysicalDamage BuildPhysical()
        {
            var damageBuffs = buffs.Where(x => x.Stat == Stat.DamageBonus || x.Stat == Stat.PhysicalDamageBonus);
            damage = damageBuffs.Aggregate(damage, (damage, buff) =>
            {
                if (buff.IsAbsolute)
                    damage += buff.Amount;
                else
                    damage *= 1 + buff.Amount;

                return damage;
            });

            return new(damage, debuffs.GetManyByChance());
        }
    }
}
