using Unity.Netcode;
using UnityEngine;

public class PlayerItemRenderer : MonoBehaviour
{
    [SerializeField] GameObject parentHandTransform;
    PlayerInventory playerInventory;
    private void Awake()
    {
        playerInventory = GetComponentInParent<PlayerInventory>();
    }

    public void UpdateHandItem(int itemId)
    {
        ItemData itemData = ItemRegistry.Instance.GetItem(itemId);
        if (itemData != null)
        {
            foreach (var child in parentHandTransform.GetComponentsInChildren<Transform>())
            {
                if (child.gameObject != parentHandTransform)
                {
                    Destroy(child.gameObject);
                }
            }
            Instantiate(itemData.HandItemMesh, parentHandTransform.transform);
        }
    }
}
