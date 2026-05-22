using Unity.Netcode;
using UnityEngine;

public class PlayerActionManager : NetworkBehaviour
{
    private PlayerInventory playerInventory;
    private PlayerController playerController;
    PlayerActionBehaviour[] actionBehaviours;
    [SerializeField] GameObject actionBehaviourContainer;
    public void Start()
    {
        if (!IsOwner) { actionBehaviourContainer.SetActive(false); return; }
        playerInventory = GetComponent<PlayerInventory>();
        playerController = GetComponent<PlayerController>();
        actionBehaviours = actionBehaviourContainer.GetComponents<PlayerActionBehaviour>();
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
