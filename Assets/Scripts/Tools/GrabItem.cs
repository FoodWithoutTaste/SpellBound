using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GrabItem : MonoBehaviour
{
   [SerializeField] GameObject grabitem;
    Item item;
   public void createGrabIcon(Item item)
    {
        this.item = item;
        StartCoroutine("destroyAfterWhile");
    }

    IEnumerator destroyAfterWhile()
    {
        var a = Instantiate(grabitem, transform.position, Quaternion.identity, transform);
        a.transform.GetChild(0).GetComponentInChildren<Image>().sprite = item.itemTexture;
        a.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = item.itemName;
        yield return new WaitForSeconds(1.1f);
        Destroy(a);

    }
}
