using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using System.Linq;
public class ItemDataHandler : NetworkBehaviour
{
    [SerializeField] private ItemData predeterminedItem;
    [SerializeField] private int initialQuantity = 1;

    private readonly NetworkVariable<NetworkInventoryItem> itemNetVar = new NetworkVariable<NetworkInventoryItem>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            if (predeterminedItem != null)
            {
                NetworkInventoryItem newItem = new NetworkInventoryItem
                {
                    ItemId = predeterminedItem.ItemId,
                    Quantity = initialQuantity,
                    ItemName = new FixedString32Bytes(predeterminedItem.name)
                };

                itemNetVar.Value = newItem;
            }
            else
            {
                Debug.LogWarning($"No predetermined item assigned on {gameObject.name}!", gameObject);
            }
        }

        itemNetVar.OnValueChanged += OnItemDataChanged;

        RefreshVisuals(itemNetVar.Value);
    }

    public override void OnNetworkDespawn()
    {
        itemNetVar.OnValueChanged -= OnItemDataChanged;
    }

    private void OnItemDataChanged(NetworkInventoryItem oldVal, NetworkInventoryItem newVal)
    {
        RefreshVisuals(newVal);
    }

    private void RefreshVisuals(NetworkInventoryItem currentNetworkItem)
    {
        ItemData visualAssetData = ItemRegistry.Instance.GetItem(currentNetworkItem.ItemId);

        if (visualAssetData != null)
        {
            foreach (var collider in GetComponentsInChildren<Transform>())
            {
                if (collider.gameObject != this.gameObject)
                {
                    Destroy(collider.gameObject);
                }
            }
            Instantiate(visualAssetData.ItemMesh).transform.SetParent(this.transform, false);
        }
    }

    public NetworkInventoryItem GetItemData()
    {
        return itemNetVar.Value;
    }
}
