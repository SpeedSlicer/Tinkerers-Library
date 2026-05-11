using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Tinker/Item")]
public class ItemData : ScriptableObject {
    public string registryName;
    public Sprite icon;         
    public GameObject prefab;  
    public readonly int maxStackSize = 16;
}
