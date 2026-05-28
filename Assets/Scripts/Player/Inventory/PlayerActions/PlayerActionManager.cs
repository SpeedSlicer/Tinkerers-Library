using Unity.Netcode;
using UnityEngine;

public class PlayerActionManager : NetworkBehaviour
{
    private PlayerInventory playerInventory;
    private PlayerMachine playerMachine;
    [SerializeField] PlayerActionBehaviour[] actionBehaviours;
    [SerializeField] GameObject actionBehaviourContainer;
    public void Start()
    {
        playerInventory = GetComponent<PlayerInventory>();
        playerMachine = GetComponent<PlayerMachine>();
        foreach (var behaviour in actionBehaviours)
        {
            behaviour.LinkComponents(playerInventory, playerMachine);
        }
        if (playerInventory == null)
        {
            Debug.LogError("PlayerActionManager: PlayerInventory component not found on the GameObject.");
        }
        if (playerMachine == null)
        {
            Debug.LogError("PlayerActionManager: PlayerMachine component not found on the GameObject.");
        }
    }
}
