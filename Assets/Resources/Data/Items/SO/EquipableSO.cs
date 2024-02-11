using Assets.Code.Items.Interfaces;

public abstract class EquipableSO : ItemSO, IEquipable
{
    public abstract void Equip();
    public abstract void Unequip();
}
