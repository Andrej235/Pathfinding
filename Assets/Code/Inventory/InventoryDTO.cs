using System;
using System.Collections.Generic;

namespace Assets.Code.Inventory
{
    [Serializable]
    public class InventoryDTO
    {
        public int Weapon;
        public int RightAbility;
        public int LeftAbility;
        public List<StorageSlotDTO> Storage;
        public List<int> Accessories;
    }
}