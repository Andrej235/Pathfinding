using Assets.Code.Items.Interfaces;

namespace Assets.Code.Inventory
{
#nullable enable
    public class Inventory
    {
        private const int NUMBER_OF_ACCESSORY_SLOTS = 5;

        public IWeapon? Weapon { get; set; }
        public IAbility? RightAbility { get; set; }
        public IAbility? LeftAbility { get; set; }
        public Storage Storage { get; set; } = new(20);
        public IAccessory?[] Accessories { get; set; } = new IAccessory[NUMBER_OF_ACCESSORY_SLOTS];
    }
}
