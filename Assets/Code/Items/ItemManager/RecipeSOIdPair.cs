using System;
using UnityEngine;

namespace Assets.Code.Items.ItemManager
{
    [Serializable]
    public class RecipeSOIdPair
    {
        public RecipeSOIdPair(int id, RecipeSO recipe)
        {
            this.id = id;
            this.recipe = recipe;
        }

        [SerializeField] private int id;
        [SerializeField] private RecipeSO recipe;

        public int Id => id;
        public RecipeSO Recipe => recipe;
    }
}