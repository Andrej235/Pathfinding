using Assets.Code.Items.Interfaces;
using System.Collections.Generic;

namespace Assets.Code.Inventory
{
#nullable enable
    public class Inventory
    {
        public IWeapon? Weapon { get; set; }
        public IAbility? RightAbility { get; set; }
        public IAbility? LeftAbility { get; set; }
        public List<StorageSlot> Storage { get; set; } = new();
        public List<IAccessory> Accessories { get; set; } = new();
    }
}
