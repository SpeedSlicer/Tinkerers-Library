using System.Collections.Generic;
using UnityEngine;

public class ItemRegistry : MonoBehaviour
{
    public static ItemRegistry Instance;

    [SerializeField] private ItemData[] items;

    private Dictionary<int, ItemData> itemLookup;

    private void Awake()
    {
        Instance = this;

        itemLookup = new Dictionary<int, ItemData>();

        foreach (var item in items)
        {
            if (item == null)
                continue;

            if (itemLookup.ContainsKey(item.ItemId))
            {
                Debug.LogError($"Duplicate Item ID detected: {item.ItemId}");
                continue;
            }

            itemLookup.Add(item.ItemId, item);
        }
    }

    public ItemData GetItem(int id)
    {
        if (itemLookup.TryGetValue(id, out var item))
            return item;

        Debug.LogWarning($"Item with ID {id} not found.");
        return null;
    }   
}