using Assets.Code.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CodeMonkey.Utils.UI_TextComplex;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject UIInventory;
    private StorageItems storage;
    [SerializeField] private GameObject icon;

    private void Start()
    {
        InventoryManager.Inventory.Storage.OnSlotChanged += OnSlotChanged;
        storage = GameObject.FindGameObjectWithTag("Player").GetComponent<StorageItems>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            UIInventory.SetActive(!UIInventory.activeSelf);
        }
    }

    private void OnSlotChanged(object sender, Storage.OnSlotChangedEventArgs e)
    {
        Debug.Log(e.Amount);
        Debug.Log(e.Item.Name);
        Instantiate(icon, storage.slots[e.SlotIndex].transform, false);
    }
}
