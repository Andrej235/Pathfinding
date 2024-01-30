using Assets.Code.Items.Interfaces;
using System.Collections.Generic;

namespace Assets.Code.Inventory
{
    public class Inventory
    {
        public IWeapon Weapon { get; protected set; }
        public IAbility RightAbility { get; protected set; }
        public IAbility LeftAbility { get; protected set; }
        public List<StorageSlot> Storage { get; protected set; }
        public List<IAccessory> Accessories { get; protected set; }
    }
}
