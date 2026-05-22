using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerItemPickupAction : PlayerActionBehaviour
{
    [SerializeField] private InputActionReference pickupActionReference;
    private void OnEnable()
    {
        if (!IsOwner) return;
        pickupActionReference.action.Enable();
        pickupActionReference.action.performed += AttemptPickup;
    }
    private void OnDisable()
    {
        if (!IsOwner) return;
        pickupActionReference.action.Disable();
        pickupActionReference.action.performed -= AttemptPickup;
    }

    public void AttemptPickup(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Ray ray = playerController.GetMainCamera().ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 3f))
            {
                if (hit.collider.GetComponent<ItemDataHandler>() != null)
                {
                    ItemDataHandler pickup = hit.collider.GetComponent<ItemDataHandler>();
                    if (pickup != null)
                    {
                        ValidatePickupServerRpc();
                    }
                }
            }
        }
    }
    [ServerRpc(InvokePermission = RpcInvokePermission.Owner)]
    private void ValidatePickupServerRpc()
    {
        if (Physics.Raycast(playerController.GetMainCamera().ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 3f) &&
            hit.collider.GetComponent<ItemDataHandler>() == null)
        {
            Debug.LogWarning("ValidatePickupServerRpc: itemToPickup is null. Possible cheater.");
            return;
        }
        ItemDataHandler itemHandler = hit.collider.GetComponent<ItemDataHandler>();
        playerInventory.AddItemToInventoryServerRpc(itemHandler.GetItemData());
    }
}
