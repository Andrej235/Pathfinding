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
        public Storage Storage { get; set; } = new(20);
        public List<IAccessory> Accessories { get; set; } = new();
    }
}
