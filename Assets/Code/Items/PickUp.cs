using Assets.Code.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUp : MonoBehaviour
{
    private StorageItems storage;
    [SerializeField] private GameObject icon;

    private void Start()
    {
        storage = GameObject.FindGameObjectWithTag("Player").GetComponent<StorageItems>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            for (int i = 0; i < storage.slots.Length ; i++)
            {
                if (storage.isFull[i] == false)
                {
                    storage.isFull[i] = true;
                    Instantiate(icon, storage.slots[i].transform, false);
                    break;
                }
            }
            Destroy(gameObject);
        }
    }
}
