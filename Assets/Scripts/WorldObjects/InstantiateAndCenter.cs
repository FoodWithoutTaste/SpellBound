using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class InstantiateAndCenter : MonoBehaviour
{
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;

    public Button equipBut;
    public void InstantiateInspectItem(Item item,InventoryUiSlot slotUI)
    {
        equipBut.onClick.RemoveAllListeners();
       
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        // Destroy any existing child objects
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Instantiate the new object
        GameObject instantiatedObject = Instantiate(item.prefab, transform.position, item.prefab.transform.rotation, transform);
        text1.text = item.itemName;
        text2.text = item.description;
       
        

        equipBut.onClick.AddListener(slotUI.EquipTheItem);
      

        // Change the layer of all child objects (including the grandparent)
        ChangeLayerRecursively(transform, 10);
    }

    void ChangeLayerRecursively(Transform parentTransform, int layer)
    {
        parentTransform.gameObject.layer = layer; // Change the layer of the current object

        // Change the layer of all child objects recursively
        foreach (Transform child in parentTransform)
        {
            ChangeLayerRecursively(child, layer);
        }
    }
}