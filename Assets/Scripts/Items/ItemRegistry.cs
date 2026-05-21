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
    // billy battista joke
        foreach (var item in items) // Iterate through the items array and add each item to the registry, while checking for null entries and duplicate IDs.
        {
            if (item == null) // Check for null entries in the items array. If a null entry is found, log a warning and skip it.
                continue; // Skip null entries in the items array.

            if (itemLookup.ContainsKey(item.ItemId)) // Check for duplicate item IDs in the registry. If a duplicate is found, log an error and skip adding it to the registry.
            {
                Debug.LogError($"Duplicate Item ID detected: {item.ItemId}");// This should never happen, but if it does, we log an error and skip adding the duplicate item to the registry.
                continue; // Skip adding the duplicate item to the registry.
            }

            itemLookup.Add(item.ItemId, item); // Add the item to the registry if it's not a duplicate.
        }
    }

    public ItemData GetItem(int id) // Retrieves an item from the registry based on its ID. If the item is not found, it logs a warning and returns null.
    {
        if (itemLookup.TryGetValue(id, out var item)) //   Try to get the item from the registry using the provided ID. If the item is found, it is returned; otherwise, a warning is logged and null is returned.
            return item;

        Debug.LogWarning($"Item with ID {id} not found."); // Log a warning if the item with the specified ID is not found in the registry.
        return null; // Return null if the item is not found in the registry.
    }   
}