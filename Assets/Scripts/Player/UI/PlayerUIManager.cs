using UnityEngine;
using System.Linq;
using Unity.Mathematics;
using Unity.Netcode;
using System.Collections.Generic;
public class PlayerUIManager : NetworkBehaviour
{
    [SerializeField] GameObject playerUICanvas;
    [SerializeField] List<HotbarUIISlot> hotbarUIISlots = new List<HotbarUIISlot>();
    void Start()
    {
        if (!IsOwner)
        {
            playerUICanvas.SetActive(false);
            return;
        }
        foreach (var slot in hotbarUIISlots)
        {
            slot.SetHotbarID(hotbarUIISlots.IndexOf(slot));
        }
        ReloadHotbar();
    }
    void Update()
    {
    }
    public void ReloadHotbar()
    {
        foreach (var slot in hotbarUIISlots)
        {
            slot.ReloadSprite();
        }
    }
}
