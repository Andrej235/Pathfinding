using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public Image image;
    [HideInInspector] public Transform parentAfterDrag;
    public int i;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Began draging");
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Draging");
        transform.position = Input.mousePosition;
    }
}
