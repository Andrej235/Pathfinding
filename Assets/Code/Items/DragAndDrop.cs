using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{

    //Drag
    public Image image;
    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public int i;
    [HideInInspector] public bool doNotDestroy;
    private Transform originalParent;

    //Drop
    private bool IODropMenu = false;
    [SerializeField] private GameObject dropMenu;

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject dropArea = eventData.pointerCurrentRaycast.gameObject;

        if (dropArea.CompareTag("Slot"))
        {
            transform.SetParent(dropArea.transform);
        }
        else
        {
            transform.SetParent(originalParent);
        }

        GetComponent<RectTransform>().localPosition = new Vector3(-50, -50, 0);
        transform.SetSiblingIndex(0);
        image.raycastTarget = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            IODropMenu = !IODropMenu;
            dropMenu.SetActive(IODropMenu);
        }
    }
}
