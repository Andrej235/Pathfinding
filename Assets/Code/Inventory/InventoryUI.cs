using Assets.Code.Inventory;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static CodeMonkey.Utils.UI_TextComplex;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject UIInventory;
    private StorageItems storage;
    [SerializeField] private GameObject icon;
    private TextMeshProUGUI text;
    private GameObject[] newIcon = new GameObject[20];

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
        text = storage.slots[e.SlotIndex].GetComponentInChildren<TextMeshProUGUI>();
        if(e.Amount > 0)
        {
            text.text = e.Amount.ToString();
        }
        else
        {
            text.text = "";
        }

        if(IsEmpty(e.SlotIndex))
        {
            newIcon[e.SlotIndex] = Instantiate(icon, storage.slots[e.SlotIndex].transform, false);
            newIcon[e.SlotIndex].GetComponent<DragAndDrop>().i = e.SlotIndex;
            newIcon[e.SlotIndex].transform.SetSiblingIndex(0);
        }
    }

    private bool IsEmpty(int i)
    {
        return newIcon[i] == null;
    }
}
