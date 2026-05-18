using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerItemLift : NetworkBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float pickupDistance = 3f;
    [SerializeField] private LayerMask pickupLayer;
    [SerializeField] private InputActionReference pickupAction;

    private NetworkObject currentTargetItem;

    private PlayerInventory playerInventory;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        pickupAction.action.Enable();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        pickupAction.action.Disable();
    }
    void Start()
    {
        if (!IsOwner) return;
        playerInventory = this.GetComponent<PlayerInventory>();
    }

    void Update()
    {
        if (!IsOwner) return;

        CheckForPickupItem();

        if (pickupAction.action.WasPressedThisFrame() && currentTargetItem != null)
        {
            RequestPickupServerRpc(currentTargetItem.NetworkObjectId);
        }
    }

    private void CheckForPickupItem()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance, pickupLayer))
        {
            if (hit.collider.TryGetComponent(out NetworkObject networkObject))
            {
                if (currentTargetItem != networkObject)
                {
                    currentTargetItem = networkObject;
                }
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
    private void RequestPickupServerRpc(ulong networkObjectId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject targetObject))
        {
            float distance = Vector3.Distance(transform.position, targetObject.transform.position);

            if (distance <= pickupDistance + 1f)
            {
                var x = targetObject.GetComponent<ItemDataHandler>();
                if (x != null)
                {
                    playerInventory.AddItemServerRpc(x.GetItemData());

                }
                targetObject.Despawn();
            }
        }
    }
}
