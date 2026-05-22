using System;
using Unity.Netcode;
using UnityEngine;
public class PlayerInventory : NetworkBehaviour
{
    NetworkList<NetworkInventoryItem> inventoryItems = new NetworkList<NetworkInventoryItem>();
    NetworkVariable<int> handItem = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    int inventorySize = 2;
    [SerializeField] PlayerHandRenderer playerHandRenderer;
    [SerializeField] GameObject itemSpawnable;
    public override void OnNetworkSpawn()
    {
        handItem.OnValueChanged += HandItemChanged;
        inventoryItems.OnListChanged += InventoryChanged;

        if (IsServer)
        {
            if (inventoryItems.Count == 0)
            {
                for (int i = 0; i < inventorySize; i++)
                {
                    inventoryItems.Add(new NetworkInventoryItem
                    {
                        ItemId = 0,
                        Quantity = 0,
                        ItemName = ""
                    });
                }
            }
        }
    }
    private void InventoryChanged(NetworkListEvent<NetworkInventoryItem> changeEvent)
    {
        playerHandRenderer.ReloadHand();
    }

    private void HandItemChanged(int previousValue, int newValue)
    {
        playerHandRenderer.ReloadHand();
    }



    [ServerRpc(InvokePermission = RpcInvokePermission.Server)]
    public void AddItemToInventoryServerRpc(NetworkInventoryItem itemToAdd)
    {
        AddItemServerOnly(itemToAdd);
    }
    public void AddItemServerOnly(NetworkInventoryItem itemToAdd)
    {
        if (!IsServer) return;
        Debug.Log("Adding item");
        Debug.Log($"Player Inventory: {inventoryItems}");
        ItemData itemData = ItemRegistry.Instance.GetItem(itemToAdd.ItemId);
        if (itemData != null)
        {
            for (int i = 0; i < inventorySize; i++)
            {
                if (itemData.IsStackable)
                {
                    if (inventoryItems[i].ItemId == itemData.ItemId && inventoryItems[i].ItemName == itemToAdd.ItemName)
                    {
                        ItemData toAddTo = ItemRegistry.Instance.GetItem(inventoryItems[i].ItemId);
                        if (inventoryItems[i].Quantity >= toAddTo.MaxStackSize)
                        {
                            continue;
                        }
                        else
                        {
                            if (inventoryItems[i].Quantity + itemToAdd.Quantity <= toAddTo.MaxStackSize)
                            {
                                inventoryItems[i] = new NetworkInventoryItem
                                {
                                    ItemId = toAddTo.ItemId,
                                    Quantity = inventoryItems[i].Quantity + itemToAdd.Quantity,
                                    ItemName = inventoryItems[i].ItemName
                                };
                                return;
                            }
                            else
                            {
                                int needed = itemData.MaxStackSize - inventoryItems[i].Quantity;
                                int leftover = itemToAdd.Quantity - needed;
                                inventoryItems[i] = new NetworkInventoryItem
                                {
                                    ItemId = toAddTo.ItemId,
                                    Quantity = inventoryItems[i].Quantity + needed,
                                    ItemName = inventoryItems[i].ItemName
                                };
                                AddItemServerOnly(new NetworkInventoryItem
                                {
                                    ItemId = itemToAdd.ItemId,
                                    ItemName = itemToAdd.ItemName,
                                    Quantity = leftover
                                });
                                return;
                            }
                        }
                    }
                }
                if (inventoryItems[i].ItemId == 0)
                {
                    if (itemToAdd.Quantity <= itemData.MaxStackSize)
                    {
                        inventoryItems[i] = itemToAdd;
                        return;
                    }
                    else if (itemToAdd.Quantity > itemData.MaxStackSize)
                    {
                        int amount = itemData.MaxStackSize;
                        int leftover = itemToAdd.Quantity - itemData.MaxStackSize;
                        inventoryItems[i] = new NetworkInventoryItem
                        {
                            ItemId = itemToAdd.ItemId,
                            ItemName = itemToAdd.ItemName,
                            Quantity = amount
                        };
                        AddItemServerOnly(new NetworkInventoryItem
                        {
                            ItemId = itemToAdd.ItemId,
                            ItemName = itemToAdd.ItemName,
                            Quantity = leftover
                        });
                        return;
                    }
                }
            }
            var go = Instantiate(itemSpawnable);
            go.GetComponent<ItemDataHandler>().SetItemDataServer(itemToAdd);
            go.transform.position = transform.position;
            go.GetComponent<NetworkObject>().Spawn();
            go.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 2, ForceMode.Impulse);
        }
        else
        {
            return;
        }
        Debug.Log($"Player Inventory: {inventoryItems}");

    }
    public int GetHandSlotNumber()
    {
        return handItem.Value;
    }

    public void SetHandItem(int slot)
    {
        if (!IsOwner) return;
        handItem.Value = slot;
    }

    public ItemData GetHandItemData()
    {
        return ItemRegistry.Instance.GetItem(inventoryItems[handItem.Value].ItemId);
    }


}
