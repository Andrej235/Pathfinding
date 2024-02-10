using Assets.Code.Inventory;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public class StorageSlot : MonoBehaviour, IDropHandler
{
    private int i;

    private void Start()
    {
        i = transform.GetSiblingIndex();
    }
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DragAndDrop draggableItem = dropped.GetComponent<DragAndDrop>();
        draggableItem.parentAfterDrag = transform;
        if (i != draggableItem.i)
        {
            InventoryManager.Inventory.Storage.Swap(i, draggableItem.i);
            Destroy(dropped);
        }
    }
}
