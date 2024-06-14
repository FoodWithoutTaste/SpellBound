using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "item/Item", order = 1)]

public class Item : ScriptableObject
{
    [Header("Item")]
    public string itemName;
    [TextArea(15, 20)]
    public string description;
    public Sprite itemTexture;
    public GameObject prefab;
 
}

