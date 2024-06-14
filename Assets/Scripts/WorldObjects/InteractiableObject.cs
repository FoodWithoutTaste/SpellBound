using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiableObject : MonoBehaviour
{
    

    void FixedUpdate()
    {
        if(transform.gameObject.layer == 9)
        {
            ChangeLayerRecursively(this.gameObject.transform, 0);
        }
    }

    private void ChangeLayerRecursively(Transform target, int newLayer)
    {
        // Change the layer of the current object
        target.gameObject.layer = newLayer;

        // Recursively change the layer of all children
        foreach (Transform child in target)
        {
            ChangeLayerRecursively(child, newLayer);
        }
    }

    public virtual void Interact()
    {

    }
}
