using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemManager", menuName = "ScriptableObjects/ItemManager")]
public class ItemManagerSO : ScriptableObject
{
    public List<ItemSOIdPair> Items = new();

    private static ItemManagerSO instance;
    public static ItemManagerSO Instance
    {
        get
        {
            if (instance == null)
                instance = Resources.Load<ItemManagerSO>(@"Data\Items\Managers\ItemManager");

            return instance;
        }
    }

    public int id;

    public void AddItem(ItemSO newItem)
    {
        if (Items.Any(x => x.Item == newItem))
            return;

        Items.Add(new(++id, newItem));

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}

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

[CustomEditor(typeof(ItemManagerSO))]
public class ItemManagerSOEditor : Editor
{
    private ItemManagerSO manager;
    private ItemSO newItem;

    private void Awake() => manager = target as ItemManagerSO;

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        newItem = (ItemSO)EditorGUILayout.ObjectField(newItem, typeof(ItemSO), false);
        if (GUILayout.Button("+"))
        {
            manager.AddItem(newItem);
            newItem = null;
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(15);

        for (int i = 0; i < manager.Items.Count; i++)
        {
            if (manager.Items[i] == null)
                continue;

            GUILayout.BeginHorizontal();
            GUILayout.Label(manager.Items[i].Item.Name);
            GUILayout.Label(manager.Items[i].Id.ToString());
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Reset"))
        {
            manager.Items = new();
            manager.id = 0;
        }

        //base.OnInspectorGUI();
    }
}
