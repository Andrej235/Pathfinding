using Assets.Code.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject UIInventory;

    private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            UIInventory.SetActive(!UIInventory.activeSelf);

        }
    }
}
