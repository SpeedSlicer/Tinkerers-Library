using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerInventory : NetworkBehaviour
{
    private readonly NetworkList<NetworkInventoryItem> inventory =
        new NetworkList<NetworkInventoryItem>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
    private readonly NetworkVariable<int> handItem = new NetworkVariable<int>(value: 0, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        inventory.OnListChanged += OnInventoryChanged;
    }

    public override void OnNetworkDespawn()
    {
        inventory.OnListChanged -= OnInventoryChanged;
        base.OnNetworkDespawn();
    }

    private void OnInventoryChanged(NetworkListEvent<NetworkInventoryItem> changeEvent)
    {
        Debug.Log($"Inventory updated. Type of change: {changeEvent.Type}");
    }

    [ServerRpc(InvokePermission = RpcInvokePermission.Owner)]
    public void AddItemServerRpc(NetworkInventoryItem nItem)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].ItemId == nItem.ItemId)
            {
                var item = inventory[i];
                item.Quantity += nItem.Quantity;
                inventory[i] = item;

                return;
            }
        }

        inventory.Add(nItem);
    }

    [ServerRpc(InvokePermission = RpcInvokePermission.Owner)]
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
                    inventory.RemoveAt(i);
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
        handItem.Value = targetedSlot;
    }
}
