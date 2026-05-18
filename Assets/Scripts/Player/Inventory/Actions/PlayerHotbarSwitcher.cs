using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHotbarSwitcher : NetworkBehaviour
{
    [SerializeField] private InputActionReference[] hotbarActions;
    [SerializeField] private PlayerInventory playerInventory;
    void Start()
    {
        if (!IsOwner) return;
        playerInventory = GetComponent<PlayerInventory>();
    }
    void OnEnable()
    {
        if (!IsOwner) return;
        foreach (var x in hotbarActions)
        {
            x.action.Enable();
        }
    }
    void OnDisable()
    {
        if (!IsOwner) return;
        foreach (var x in hotbarActions)
        {
            x.action.Enable();
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        for (int i = 0; i < hotbarActions.Count(); i++)
        {
            if (hotbarActions[i].action.WasPressedThisFrame())
            {
                playerInventory.SetItemSlotServerRpc(i);
            }
        }
    }
}
