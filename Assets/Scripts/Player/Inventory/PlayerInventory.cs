using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : NetworkBehaviour
{
    private const int INVENTORY_SIZE = 32;

    private readonly NetworkList<NetworkInventoryItem> inventory =
        new NetworkList<NetworkInventoryItem>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
    
    private readonly NetworkVariable<int> handItem = 
        new NetworkVariable<int>(value: 0, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
    
    public UnityEvent runTimeInventoryUpdateEvent;

    [SerializeField] private PlayerItemRenderer playerItemRenderer;

    public void Start()
    {
    }

    public override void OnNetworkSpawn()
    {
        inventory.OnListChanged += OnInventoryChanged;
        handItem.OnValueChanged += OnHandValueChanged;

        if (IsServer && inventory.Count == 0)
        {
            for (int i = 0; i < INVENTORY_SIZE; i++)
            {
                inventory.Add(new NetworkInventoryItem { ItemId = 0, Quantity = 0 });
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        inventory.OnListChanged -= OnInventoryChanged;
        handItem.OnValueChanged -= OnHandValueChanged;
        base.OnNetworkDespawn();
    }
    
    private void OnHandValueChanged(int previousValue, int newValue)
    {
        Debug.Log($"Hand item changed from slot {previousValue} to slot {newValue}");
        ReloadHandItem();
    }

    private void ReloadHandItem()
    {
        if (handItem.Value >= 0 && handItem.Value < inventory.Count && inventory[handItem.Value].ItemId != 0)
        {
            playerItemRenderer.UpdateHandItem(inventory[handItem.Value].ItemId);
        }
        else
        {
             playerItemRenderer.UpdateHandItem(0);
        }
    }

    private void OnInventoryChanged(NetworkListEvent<NetworkInventoryItem> changeEvent)
    {
        Debug.Log($"Inventory updated. Type of change: {changeEvent.Type}");
        runTimeInventoryUpdateEvent?.Invoke();
        ReloadHandItem();
    }

    [ServerRpc(InvokePermission = RpcInvokePermission.Owner)]
    public void SwapItemsServerRpc(int fromSlot, int toSlot)
    {
        if (fromSlot < 0 || fromSlot >= inventory.Count || toSlot < 0 || toSlot >= inventory.Count) 
            return;

        NetworkInventoryItem temp = inventory[fromSlot];
        inventory[fromSlot] = inventory[toSlot];
        inventory[toSlot] = temp;
    }

    public void AddItemServerOnly(NetworkInventoryItem nItem)
    {
        if (!IsServer) return; 
        ProcessAddItem(nItem);
    }

    [ServerRpc(InvokePermission = RpcInvokePermission.Server)]
    public void AddItemServerRpc(NetworkInventoryItem nItem)
    {
        ProcessAddItem(nItem);
    }

    private void ProcessAddItem(NetworkInventoryItem nItem)
    {
        int firstEmptyIndex = -1;

        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].ItemId == nItem.ItemId)
            {
                var item = inventory[i];
                item.Quantity += nItem.Quantity;
                inventory[i] = item;
                return;
            }

            if (firstEmptyIndex == -1 && inventory[i].ItemId == 0)
            {
                firstEmptyIndex = i;
            }
        }

        if (firstEmptyIndex != -1)
        {
            inventory[firstEmptyIndex] = nItem;
        }
    }

    [ServerRpc(InvokePermission = RpcInvokePermission.Server)]
    public void RemoveItemServerRpc(int itemId, int amount)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].ItemId == itemId)
            {
                var item = inventory[i];
                item.Quantity -= amount;

                if (item.Quantity <= 0)
                {
                    inventory[i] = new NetworkInventoryItem { ItemId = 0, Quantity = 0 };
                }
                else
                {
                    inventory[i] = item;
                }
                return;
            }
        }
    }

    [ServerRpc(InvokePermission = RpcInvokePermission.Owner)]
    public void SetItemSlotServerRpc(int targetedSlot)
    {
        if (targetedSlot >= 0 && targetedSlot < inventory.Count)
        {
            handItem.Value = targetedSlot;
        }
    }

    public ItemData GetItemInHand()
    {
        if (handItem.Value < 0 || handItem.Value >= inventory.Count) return null;
        int itemId = inventory[handItem.Value].ItemId;
        return ItemRegistry.Instance.GetItem(itemId);
    }

    public int GetItemIDInHand()
    {
        if (handItem.Value < 0 || handItem.Value >= inventory.Count) return 0;
        return inventory[handItem.Value].ItemId;
    }
}
