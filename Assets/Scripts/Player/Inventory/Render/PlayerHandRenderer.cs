using UnityEngine;

public class PlayerHandRenderer : MonoBehaviour
{
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private GameObject handItemObject;

    public void ReloadHand()
    {
        if (handItemObject.transform.childCount > 0)
        {
            Destroy(handItemObject.transform.GetChild(0).gameObject);
        }

        GameObject newItem = Instantiate(
            playerInventory.GetHandItemData().HandItemMesh,
            handItemObject.transform
        );

        newItem.transform.localPosition = Vector3.zero;
        newItem.transform.localRotation = Quaternion.identity;
        newItem.transform.localScale = Vector3.one;
    }
}