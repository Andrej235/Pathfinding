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
                inventory ??= new();//LoadInventoryData();
                return inventory;
            }

            set => inventory = value;
        }

        /*        public static void SaveInventoryData()
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
                            ItemId = x.Item.GetId(),
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

                    var storage = inventoryDTO.Storage?.Select(x => new StorageSlot()
                    {
                        Id = x.ItemId,
                        Amount = x.Amount,
                    }).ToList();
                    var accessories = inventoryDTO.Accessories?.Select(ItemManagerSO.Instance.GetItem).Cast<IAccessory>().ToList();

                    return inventoryDTO is null ? new() : new()
                    {
                        Weapon = ItemManagerSO.Instance.GetItem(inventoryDTO.Weapon) as IWeapon,
                        LeftAbility = ItemManagerSO.Instance.GetItem(inventoryDTO.LeftAbility) as IAbility,
                        RightAbility = ItemManagerSO.Instance.GetItem(inventoryDTO.RightAbility) as IAbility,
                        Accessories = accessories ?? new(),
                        Storage = storage ?? new()
                    };
                }*/
    }
}
