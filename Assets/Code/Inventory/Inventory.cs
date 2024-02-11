using Assets.Code.Items.Interfaces;

namespace Assets.Code.Inventory
{
#nullable enable
    public class Inventory
    {
        public const int NUMBER_OF_ACCESSORY_SLOTS = 5;
        public const int NUMBER_OF_ABILITY_SLOTS = 5;

        public IWeapon? Weapon { get; set; }
        public IAbility?[] Abilities { get; set; } = new IAbility[NUMBER_OF_ABILITY_SLOTS];
        public Storage Storage { get; set; } = new(20);
        public IAccessory?[] Accessories { get; set; } = new IAccessory[NUMBER_OF_ACCESSORY_SLOTS];
    }
}
