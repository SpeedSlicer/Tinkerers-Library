using Unity.Netcode;
using UnityEngine;
public class PlayerActionBehaviour : NetworkBehaviour
{
    protected PlayerInventory playerInventory;
    protected PlayerMachine playerController;
    public void LinkComponents(PlayerInventory inventory, PlayerMachine controller)
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
