using Assets.Code.Inventory;
using UnityEngine;

public class TestingInventory : MonoBehaviour
{
    void Start()
    {
        StorageSlot slot = new()
        {
            Id = 2,
            Amount = 15,
        };

        InventoryManager.Inventory.Storage.Add(slot);

        var a = InventoryManager.Inventory.Storage;
        foreach (var item in a)
            Debug.Log(item.Item.Name);
    }

    public void Save()
    {
        InventoryManager.SaveInventoryData();
    }
}
