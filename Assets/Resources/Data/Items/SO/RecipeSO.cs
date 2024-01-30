using Assets.Code.Inventory;
using Assets.Code.Items.Interfaces;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "MyRecipe", menuName = "Item/Recipe")]
public class RecipeSO : ScriptableObject
{
    public List<RecipeIngredient> Ingredients => ingredients;
    [SerializeField] private List<RecipeIngredient> ingredients;

    public IItem Result => result;
    [SerializeField] private IItem result;

    [SerializeField] private int a;
}

[CustomEditor(typeof(RecipeSO))]
public class RecipeSOEditor : Editor
{
    private RecipeSO recipe;

    private void Awake() => recipe = (RecipeSO)target;

    public override void OnInspectorGUI()
    {
        for (int i = 0; i < recipe.Ingredients.Count; i++)
        {
            GUILayout.BeginHorizontal();
            recipe.Ingredients[i].MaterialItemSO = EditorGUILayout.ObjectField(recipe.Ingredients[i].MaterialItemSO, typeof(ItemSO), false) as ItemSO;
            if (recipe.Ingredients[i].MaterialItemSO is not IMaterial material)
                recipe.Ingredients[i].MaterialItemSO = null;
            else
                recipe.Ingredients[i].Amount = EditorGUILayout.IntSlider(recipe.Ingredients[i].Amount, 0, material.MaxStack);
            GUILayout.EndHorizontal();
        }
        SaveChanges();
    }
}