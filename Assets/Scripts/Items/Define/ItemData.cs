using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Tinker/Item")]
public class ItemData : ScriptableObject {
    [SerializeField] string registryName;
    [SerializeField] Sprite icon;         
    [SerializeField] GameObject itemMesh;  
    [SerializeField] int maxStackSize = 16;
}
