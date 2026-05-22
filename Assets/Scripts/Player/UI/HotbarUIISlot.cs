using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
public class HotbarUIISlot : NetworkBehaviour
{
    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] GameObject activeFrame;
    [SerializeField] Image itemIcon;
    [SerializeField] int id = -1;
    [SerializeField] int itemID = 0;

    public void SetHotbarID(int id)
    {
        this.id = id;
    }
    private void SetFrameActive(bool set)
    {
        activeFrame.SetActive(set);
    }
    public void ReloadSprite()
    {
        itemID = playerInventory.GetItemInSlot(id).ItemId;
        itemIcon.sprite = ItemRegistry.Instance.GetItem(itemID).Icon;
        itemIcon.type = Image.Type.Simple;
        itemIcon.preserveAspect = true;
        SetFrameActive(playerInventory.GetHandSlot() == id);
    }
}
