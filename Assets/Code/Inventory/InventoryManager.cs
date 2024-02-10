using Assets.Code.Items.Interfaces;
using Assets.Code.Items.ItemExtensions;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Code.Inventory
{
#nullable enable
    public static class InventoryManager
    {
        private static readonly string dataFolderPath = Application.persistentDataPath;
        private static readonly string inventoryDataFilePath = dataFolderPath + @"\inventory\inventory.json";

        private static Inventory? inventory;
        public static Inventory Inventory
        {
            get
            {
                inventory ??= LoadInventoryData();
                return inventory;
            }

            set => inventory = value;
        }

        public static void SaveInventoryData()
        {
            InventoryDTO inventoryDTO = new()
            {
                Weapon = Inventory.Weapon.GetId(),
                LeftAbility = Inventory.LeftAbility.GetId(),
                RightAbility = Inventory.RightAbility.GetId(),
                Accessories = Inventory.Accessories.Select(x => x.GetId()).ToList(),
                Storage = Inventory.Storage.Select(x => new StorageSlotDTO()
                {
                    Amount = x.Amount,
                    ItemId = x.Id,
                }).ToList()
            };

            var inventoryJson = JsonUtility.ToJson(inventoryDTO);
            File.WriteAllText(inventoryDataFilePath, inventoryJson);
        }

        private static Inventory LoadInventoryData()
        {
            if (!File.Exists(inventoryDataFilePath))
            {
                if (!Directory.Exists(dataFolderPath + @"\inventory"))
                    Directory.CreateDirectory(dataFolderPath + @"\inventory");

                File.Create(inventoryDataFilePath);
                return new();
            }

            var inventoryJson = File.ReadAllText(inventoryDataFilePath);
            if (string.IsNullOrWhiteSpace(inventoryJson))
                return new();

            var inventoryDTO = JsonUtility.FromJson<InventoryDTO>(inventoryJson);

            var storage = inventoryDTO.Storage.Aggregate(new Storage(20), (storage, slotDTO) =>
            {
                var item = ItemManagerSO.Instance.GetItem(slotDTO.ItemId);
                if (item is null)
                    return storage;

                storage.Add(item, slotDTO.Amount);
                return storage;
            });

            var accessories = inventoryDTO.Accessories?.Select(ItemManagerSO.Instance.GetItem).Cast<IAccessory>().ToList();

            return inventoryDTO is null ? new() : new()
            {
                Weapon = ItemManagerSO.Instance.GetItem(inventoryDTO.Weapon) as IWeapon,
                LeftAbility = ItemManagerSO.Instance.GetItem(inventoryDTO.LeftAbility) as IAbility,
                RightAbility = ItemManagerSO.Instance.GetItem(inventoryDTO.RightAbility) as IAbility,
                Accessories = accessories ?? new(),
                Storage = storage ?? new(20)
            };
        }

        public static bool EquipAccessory(IAccessory accessory)
        {
            return true;
        }

        public static bool EquipWeapon(IWeapon weapon)
        {
            return true;
        }

        public static bool EquipAbility(IAbility ability)
        {
            return true;
        }

        public static IAccessory? UnequipAccessory(int index = 0)
        {
            if (index < 0 || index >= Inventory.Accessories.Length)
                return null;

            IAccessory? accessory;
            (accessory, Inventory.Accessories[index]) = (Inventory.Accessories[index], null);
            return accessory;
        }

        public static IWeapon? UnequipWeapon()
        {
            IWeapon? weapon;
            (weapon, Inventory.Weapon) = (Inventory.Weapon, null);
            return weapon;
        }

        public static IAbility? UnequipAbility(int index = 0)
        {
            IAbility? ability = null;
            if (index == 0)
                (ability, Inventory.LeftAbility) = (Inventory.LeftAbility, null);
            else if (index == 1)
                (ability, Inventory.RightAbility) = (Inventory.RightAbility, null);

            return ability;
        }
    }
}
