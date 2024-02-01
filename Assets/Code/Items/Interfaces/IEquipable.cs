namespace Assets.Code.Items.Interfaces
{
    public interface IEquipable : IItem
    {
        void Equip();
        void Unequip();
    }
}
