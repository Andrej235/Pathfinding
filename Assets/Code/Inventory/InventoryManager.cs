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
                Abilities = Inventory.Abilities.Select(x => x.GetId()).ToList(),
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

            var loadedAccessories = inventoryDTO.Accessories?.Select(ItemManagerSO.Instance.GetItem).Cast<IAccessory>();
            var loadedAccessoriesCount = loadedAccessories.Count();
            IAccessory[] accessories = new IAccessory[Inventory.NUMBER_OF_ACCESSORY_SLOTS];

            if (loadedAccessories != null)
            {
                if (loadedAccessoriesCount == Inventory.NUMBER_OF_ACCESSORY_SLOTS)
                {
                    accessories = loadedAccessories.ToArray();
                }
                else if (loadedAccessoriesCount > Inventory.NUMBER_OF_ACCESSORY_SLOTS)
                {
                    accessories = loadedAccessories.Take(Inventory.NUMBER_OF_ACCESSORY_SLOTS).ToArray();
                }
                else
                {
                    for (int i = 0; i < loadedAccessoriesCount; i++)
                        accessories[i] = loadedAccessories.ElementAt(i);
                }
            }

            var loadedAbilities = inventoryDTO.Abilities?.Select(ItemManagerSO.Instance.GetItem).Cast<IAbility>();
            var loadedAbilitiesCount = loadedAbilities.Count();
            IAbility[] abilities = new IAbility[Inventory.NUMBER_OF_ABILITY_SLOTS];

            if (loadedAbilities != null)
            {
                if (loadedAbilitiesCount == Inventory.NUMBER_OF_ABILITY_SLOTS)
                {
                    abilities = loadedAbilities.ToArray();
                }
                else if (loadedAbilitiesCount > Inventory.NUMBER_OF_ABILITY_SLOTS)
                {
                    abilities = loadedAbilities.Take(Inventory.NUMBER_OF_ABILITY_SLOTS).ToArray();
                }
                else
                {
                    for (int i = 0; i < loadedAbilitiesCount; i++)
                        abilities[i] = loadedAbilities.ElementAt(i);
                }
            }

            return inventoryDTO is null ? new() : new()
            {
                Weapon = ItemManagerSO.Instance.GetItem(inventoryDTO.Weapon) as IWeapon,
                Abilities = abilities,
                Accessories = accessories,
                Storage = storage ?? new(20)
            };
        }

        public static bool EquipAccessory(IAccessory accessoryToEquip)
        {
            for (int i = 0; i < Inventory.NUMBER_OF_ACCESSORY_SLOTS; i++)
            {
                if (Inventory.Accessories[i] is null)
                {
                    Inventory.Accessories[i] = accessoryToEquip;
                    accessoryToEquip.Equip();
                    return true;
                }
            }

            return false;
        }

        public static bool EquipAccessory(IAccessory accessoryToEquip, int index)
        {
            if (index < 0 || index >= Inventory.NUMBER_OF_ACCESSORY_SLOTS)
                return false;

            if (Inventory.Accessories[index] is null)
            {
                Inventory.Accessories[index] = accessoryToEquip;
                accessoryToEquip.Equip();
                return true;
            }

            return false;
        }

        public static bool EquipWeapon(IWeapon weapon)
        {
            if (Inventory.Weapon is null)
            {
                Inventory.Weapon = weapon;
                weapon.Equip();
                return true;
            }

            return false;
        }

        public static bool EquipAbility(IAbility abilityToEquip)
        {
            for (int i = 0; i < Inventory.NUMBER_OF_ABILITY_SLOTS; i++)
            {
                if (Inventory.Abilities[i] is null)
                {
                    Inventory.Abilities[i] = abilityToEquip;
                    abilityToEquip.Equip();
                    return true;
                }
            }

            return false;
        }
        public static bool EquipAbility(IAbility abilityToEquip, int index)
        {
            if (index < 0 || index >= Inventory.NUMBER_OF_ABILITY_SLOTS)
                return false;

            if (Inventory.Abilities[index] is null)
            {
                Inventory.Abilities[index] = abilityToEquip;
                abilityToEquip.Equip();
                return true;
            }

            return false;
        }

        public static IAccessory? UnequipAccessory(int index = 0)
        {
            if (index < 0 || index >= Inventory.NUMBER_OF_ACCESSORY_SLOTS)
                return null;

            IAccessory? accessory;
            (accessory, Inventory.Accessories[index]) = (Inventory.Accessories[index], null);

            accessory?.Unequip();
            return accessory;
        }

        public static IWeapon? UnequipWeapon()
        {
            IWeapon? weapon;
            (weapon, Inventory.Weapon) = (Inventory.Weapon, null);

            weapon?.Unequip();
            return weapon;
        }

        public static IAbility? UnequipAbility(int index = 0)
        {
            if (index < 0 || index >= Inventory.NUMBER_OF_ABILITY_SLOTS)
                return null;

            IAbility? ability;
            (ability, Inventory.Abilities[index]) = (Inventory.Abilities[index], null);

            ability?.Unequip();
            return ability;
        }
    }
}
