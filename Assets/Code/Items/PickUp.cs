using Assets.Code.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUp : MonoBehaviour
{
    [SerializeField] private ItemSO itemSO;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InventoryManager.Inventory.Storage.Add(itemSO, 1);
            Destroy(gameObject);
        }
    }
}
