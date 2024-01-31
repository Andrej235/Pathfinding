using Assets.Code.Items.Interfaces;
using Assets.Code.Items.ItemManager;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#nullable enable
[CreateAssetMenu(fileName = "ItemManager", menuName = "ScriptableObjects/ItemManager")]
public class ItemManagerSO : ScriptableObject
{
    public List<ItemSOIdPair> Items = new();
    public List<RecipeSOIdPair> Recipes = new();

    public static ItemManagerSO Instance
    {
        get
        {
            if (instance == null)
                instance = Resources.Load<ItemManagerSO>(@"Data\Items\Managers\ItemManager");

            return instance;
        }
    }
    private static ItemManagerSO instance = null!;

    public int highestItemId;
    public void AddItem(ItemSO newItem)
    {
        if (Items.Any(x => x.Item == newItem))
            return;

        Items.Add(new(++highestItemId, newItem));

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public int highestRecipeId;
    public void AddRecipe(RecipeSO newRecipe)
    {
        if (Recipes.Any(x => x.Recipe == newRecipe))
            return;

        Recipes.Add(new(++highestRecipeId, newRecipe));

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    public IItem? GetItem(int id) => Items.FirstOrDefault(x => x.Id == id)?.Item;
    public int GetItemId(IItem item)
    {
        if (item is null)
            return -1;

        //var idPair = Items.FirstOrDefault(x => x.Item == (Object)item);
        //Test the line above, then the line below
        var idPair = Items.FirstOrDefault(x => (x.Item as IItem).Name == item.Name);
        return idPair != null ? idPair.Id : -1;
    }

    public RecipeSO? GetRecipe(int id) => Recipes.FirstOrDefault(x => x.Id == id).Recipe;
    public int GetRecipeId(RecipeSO recipe)
    {
        var idPair = Recipes.FirstOrDefault(x => x.Recipe == recipe);
        return idPair != null ? idPair.Id : -1;
    }

    public bool editor_ShowItems;
    public bool editor_ShowRecipes;
}

[CustomEditor(typeof(ItemManagerSO))]
public class ItemManagerSOEditor : Editor
{
    private ItemManagerSO manager = null!;

    private ItemSO? newItem;
    private RecipeSO? newRecipe;

    private void Awake() => manager = (ItemManagerSO)target;

    public override void OnInspectorGUI()
    {
        manager.editor_ShowItems = GUILayout.Toggle(manager.editor_ShowItems, "Show items");
        GUILayout.Space(5);
        if (manager.editor_ShowItems)
        {
            for (int i = 0; i < manager.Items.Count; i++)
            {
                if (manager.Items[i] == null)
                    continue;

                GUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(manager.Items[i].Item, typeof(ItemSO), false, GUILayout.Width(150));
                GUILayout.Label($"Id: {manager.Items[i].Id}");

                if (GUILayout.Button("-", GUILayout.Width(33)))
                {
                    manager.Items.Remove(manager.Items[i]);
                    SaveChanges();
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            newItem = (ItemSO)EditorGUILayout.ObjectField(newItem, typeof(ItemSO), false);
            if (GUILayout.Button("+", GUILayout.Width(33)) && newItem != null)
            {
                manager.AddItem(newItem);
                newItem = null;
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(manager.editor_ShowItems && manager.editor_ShowRecipes ? 30 : 10);

        manager.editor_ShowRecipes = GUILayout.Toggle(manager.editor_ShowRecipes, "Show recipes");
        GUILayout.Space(5);
        if (manager.editor_ShowRecipes)
        {
            for (int i = 0; i < manager.Recipes.Count; i++)
            {
                if (manager.Recipes[i] == null)
                    continue;

                GUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(manager.Recipes[i].Recipe, typeof(RecipeSO), false, GUILayout.Width(150));
                GUILayout.Label($"Id: {manager.Recipes[i].Id}");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            newRecipe = (RecipeSO)EditorGUILayout.ObjectField(newRecipe, typeof(RecipeSO), false);
            if (GUILayout.Button("+", GUILayout.Width(33)) && newRecipe != null)
            {
                manager.AddRecipe(newRecipe);
                newRecipe = null;
            }
            GUILayout.EndHorizontal();
        }
    }
}
