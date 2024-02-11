using System;
using System.Collections.Generic;

namespace Assets.Code.Inventory
{
    [Serializable]
    public class InventoryDTO
    {
        public int Weapon;
        public List<StorageSlotDTO> Storage;
        public List<int> Accessories;
        public List<int> Abilities;
    }
}