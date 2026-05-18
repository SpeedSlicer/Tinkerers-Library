using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Tinker/Item")]
public class ItemData : ScriptableObject
{
    [Header("Identification")]
    [SerializeField] private int itemId;
    [SerializeField] private string registryName;

    [Header("Visuals")]
    [SerializeField] private Sprite icon;
    [SerializeField] private GameObject itemMesh;

    [Header("Gameplay")]
    [SerializeField] private int maxStackSize = 99;

    public int ItemId => itemId;
    public string RegistryName => registryName;
    public Sprite Icon => icon;
    public GameObject ItemMesh => itemMesh;
    public int MaxStackSize => maxStackSize;
}