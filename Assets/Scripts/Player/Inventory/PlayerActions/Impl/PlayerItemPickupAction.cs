using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerItemPickupAction : PlayerActionBehaviour
{
    [SerializeField] private InputActionReference pickupActionReference;
    [SerializeField] private LayerMask pickupLayer;
    [SerializeField] private NetworkObject currentTargetItem;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        pickupActionReference.action.Enable();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        pickupActionReference.action.Disable();
    }
    void Update()
    {
        if (!IsOwner) return;
        DetectItem();
        if (pickupActionReference.action.WasPerformedThisFrame())
        {
            if (currentTargetItem != null)
            {
                Debug.Log("Sent request to pickup item (Client)");

                ValidatePickupServerRpc(this.playerController.gameObject.GetComponent<NetworkObject>().NetworkObjectId, currentTargetItem.NetworkObjectId);
            }
        }
    }

    public void DetectItem()
    {
        Ray ray = new Ray(playerController.GetMainCamera().transform.position, playerController.GetMainCamera().transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 3f, pickupLayer))
        {
            if (hit.collider.TryGetComponent<ItemDataHandler>(out var itemHandler))
            {
                currentTargetItem = itemHandler.GetComponent<NetworkObject>();
                Debug.Log("Found new item");
                return;
            }
        }
        ClearTarget();
    }
    private void ClearTarget()
    {
        if (currentTargetItem != null)
        {
            currentTargetItem = null;
        }
    }
    [ServerRpc]
    private void ValidatePickupServerRpc(ulong playerNetworkObject, ulong itemNetworkObjectId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(itemNetworkObjectId, out NetworkObject netObject))
        {
            if (netObject.TryGetComponent<ItemDataHandler>(out ItemDataHandler entity))
            {
                Debug.Log("Sent request to pickup item");

                playerInventory.AddItemServerOnly(entity.GetItemData());
                entity.GetComponent<NetworkObject>().Despawn(true);
            }
        }
    }
}
