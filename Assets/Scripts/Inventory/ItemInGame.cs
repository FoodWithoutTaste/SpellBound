using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInGame : InteractiableObject
{
    public Item item;
    public int quantity;
    public void Start()
    {
        Instantiate(item.prefab, transform.GetChild(0));
    }
    public override void Interact()
    {
        base.Interact();
        FindObjectOfType<PL_Inventory>().AddItem(item, 1);
        FindAnyObjectByType<GrabItem>().createGrabIcon(item);
        Destroy(this.gameObject);
    }
}
