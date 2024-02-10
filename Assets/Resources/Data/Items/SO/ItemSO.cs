using Assets.Code.Items.Interfaces;
using UnityEngine;

public class ItemSO : ScriptableObject, IItem
{
    [SerializeField] private string itemName;
    [SerializeField] private int maxStack;
    [SerializeField] private Sprite icon;

    public string Name => itemName;
    public int MaxStack => maxStack;
    public Sprite Icon => icon;
}
