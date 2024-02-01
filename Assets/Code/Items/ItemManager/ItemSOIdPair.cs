using System;
using UnityEngine;

namespace Assets.Code.Items.ItemManager
{
    [Serializable]
    public class ItemSOIdPair
    {
        public ItemSOIdPair(int id, ItemSO item)
        {
            this.id = id;
            this.item = item;
        }

        [SerializeField] private int id;
        [SerializeField] private ItemSO item;

        public int Id => id;
        public ItemSO Item => item;
    }
}