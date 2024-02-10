using Assets.Code.Entity.Damage;
using Assets.Code.Entity.Effects;
using Assets.Code.Entity.Effects.Buffs;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Entity
{
    public class DamageTesting : MonoBehaviour
    {
        public class DamageBuff : IBuff
        {
            private readonly float amount;

            public DamageBuff(float amount)
            {
                this.amount = amount;
            }

            public Stat Stat => Stat.DamageBonus;

            public float Amount => amount;

            public bool IsAbsolute => false;
        }

        private void Start()
        {
            IDamage damage = DamageBuilder.StartBuiding(10)
                .AddBuffs(new List<IBuff>() { new DamageBuff(.53f) })
                .BuildPhysical();

            Debug.Log(damage.Damage);
        }
    }
}
