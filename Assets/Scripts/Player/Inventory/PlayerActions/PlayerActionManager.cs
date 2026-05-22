using Unity.Netcode;
using UnityEngine;

public class PlayerActionManager : NetworkBehaviour
{
    private PlayerInventory playerInventory;
    private PlayerController playerController;
    [SerializeField] PlayerActionBehaviour[] actionBehaviours;
    [SerializeField] GameObject actionBehaviourContainer;
    public void Start()
    {
        playerInventory = GetComponent<PlayerInventory>();
        playerController = GetComponent<PlayerController>();
        foreach (var behaviour in actionBehaviours)
        {
            behaviour.LinkComponents(playerInventory, playerController);
        }
        if (playerInventory == null)
        {
            Debug.LogError("PlayerActionManager: PlayerInventory component not found on the GameObject.");
        }
        if (playerController == null)
        {
            Debug.LogError("PlayerActionManager: PlayerController component not found on the GameObject.");
        }
    }
}
