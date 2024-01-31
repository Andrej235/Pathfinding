using Assets.Code.Inventory;
using UnityEngine;

public class TestingInventory : MonoBehaviour
{
    void Start()
    {
        InventoryManager.Inventory.Storage.Add(new()
        {
            Item = ItemManagerSO.Instance.GetItem(1),
            Amount = 15,
        });

        //var a = InventoryManager.Inventory.Storage;
        //Debug.Log(a[0]?.Item.Name);
    }

    public void Save()
    {
        InventoryManager.SaveInventoryData();
    }
}
