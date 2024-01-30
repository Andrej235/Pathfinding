using Assets.Code.Items.Interfaces;
using System;
using UnityEngine;

namespace Assets.Code.Inventory
{
    [Serializable]
    public class RecipeIngredient
    {
        public IMaterial Material => material as IMaterial;
        public ItemSO MaterialItemSO
        {
            get => material;
            set => material = value;
        }
        [SerializeField] private ItemSO material;

        public int Amount
        {
            get => amount;
            set => amount = value > 0 ? value : 0;
        }
        [SerializeField] private int amount;
    }
}