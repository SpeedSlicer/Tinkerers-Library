using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using System.Linq;
public class PlayerInventory : NetworkBehaviour
{
    NetworkList<NetworkInventoryItem> inventoryItems = new NetworkList<NetworkInventoryItem>();
    void Start()
    {
        if (!IsOwner) return;
        for (int i = 0; i < 9; i++)
        {
            inventoryItems.Add(new NetworkInventoryItem { ItemId = 0, Quantity = 0, ItemName = "" });
        }
    }

    [ServerRpc(InvokePermission = RpcInvokePermission.Server)]
    public void AddItemToInventoryServerRpc(NetworkInventoryItem itemToAdd)
    {
        ItemData itemData = ItemRegistry.Instance.GetItem(itemToAdd.ItemId);
        if (itemData != null)
        {
            if (itemData.IsStackable)
            {
                int existingIndex = -1;
                for (int i = 0; i < inventoryItems.Count; i++)
                {
                    if (inventoryItems[i].ItemId == itemToAdd.ItemId)
                    {
                        if (inventoryItems[i].Quantity + itemToAdd.Quantity > ItemRegistry.Instance.GetItem(itemToAdd.ItemId).MaxStackSize)
                        {
                            int overflow = (inventoryItems[i].Quantity + itemToAdd.Quantity) - ItemRegistry.Instance.GetItem(itemToAdd.ItemId).MaxStackSize;
                            inventoryItems[i] = new NetworkInventoryItem
                            {
                                ItemId = itemToAdd.ItemId,
                                Quantity = ItemRegistry.Instance.GetItem(itemToAdd.ItemId).MaxStackSize,
                                ItemName = itemToAdd.ItemName
                            };
                            AddItemToInventoryServerRpc(new NetworkInventoryItem
                            {
                                ItemId = itemToAdd.ItemId,
                                Quantity = overflow,
                                ItemName = itemToAdd.ItemName
                            });
                            return;
                        }
                        existingIndex = i;
                        break;
                    }
                }
                if (existingIndex != -1)
                {
                    NetworkInventoryItem existingItem = inventoryItems[existingIndex];
                    existingItem.Quantity += itemToAdd.Quantity;
                    inventoryItems[existingIndex] = existingItem;
                    return;
                }
            }

            inventoryItems.Add(itemToAdd);
        }
    }
 
}
