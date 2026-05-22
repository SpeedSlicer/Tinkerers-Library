using Unity.Netcode;
using UnityEngine;
public class PlayerActionBehaviour : NetworkBehaviour
{
    protected PlayerInventory playerInventory;
    protected PlayerController playerController;
    public void LinkComponents(PlayerInventory inventory, PlayerController controller)
    {
        if (playerInventory != null || playerController != null)
        {
            Debug.LogWarning("PlayerActionBehaviour: Components already linked. Possible cheater");
            return;
        }
        Debug.Log("Linked Components");
        playerInventory = inventory;
        playerController = controller;
    }
}
