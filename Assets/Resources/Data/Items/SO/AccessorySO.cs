using Assets.Code.Entity.Effects;
using Assets.Code.Entity.Effects.Buffs;
using Assets.Code.Items.Interfaces;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MyAccessory", menuName = "Item/Accessory")]
public class AccessorySO : EquipableSO, IAccessory
{
    [SerializeField] private AccessoryBuff buff;
    public IBuff Buff => buff;

    public override void Equip()
    {
        throw new NotImplementedException();
    }

    public override void Unequip()
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class AccessoryBuff : IBuff
{
    [SerializeField] private Stat stat;
    [SerializeField] private float amount;
    [SerializeField] private bool isAbsolute;

    public AccessoryBuff(Stat stat, float amount, bool isAbsolute)
    {
        this.stat = stat;
        this.amount = amount;
        this.isAbsolute = isAbsolute;
    }

    public Stat Stat => stat;
    public float Amount => amount;
    public bool IsAbsolute => isAbsolute;
}