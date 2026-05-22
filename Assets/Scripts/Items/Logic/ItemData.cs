using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Tinker/Item")]
public class ItemData : ScriptableObject
{
    [Header("Identification")]
    [SerializeField] private int itemId;
    [SerializeField] private string registryName;

    [Header("Visuals")]
    [SerializeField] private Sprite icon;
    [SerializeField] private GameObject groundItemMesh;
    [SerializeField] private GameObject handItemMesh;

    [SerializeField] private bool isStackable = true;   
    [SerializeField] private int maxStackSize = 99;
    [Header("Gameplay")]
    [SerializeField] private ItemTypes itemType = ItemTypes.Hand;
    public int ItemId => itemId;
    public string RegistryName => registryName;
    public Sprite Icon => icon;
    public GameObject GroundItemMesh => groundItemMesh;
    public GameObject HandItemMesh => handItemMesh;
    public ItemTypes ItemType => itemType;
    public bool IsStackable => isStackable;
    public int MaxStackSize => maxStackSize;

}