using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSystem : MonoBehaviour
{
    Camera main;
    [SerializeField] float distance;
    [SerializeField] GameObject E;



    private static InteractSystem instance;

    // Singleton instance property
    public static InteractSystem Instance
    {
        get
        {
            if (instance == null)
            {
                // If instance is null, find the existing InteractSystem component in the scene
                instance = FindObjectOfType<InteractSystem>();

                // If it doesn't exist, create a new GameObject with InteractSystem attached
                if (instance == null)
                {
                    GameObject obj = new GameObject("InteractSystem");
                    instance = obj.AddComponent<InteractSystem>();
                }
            }
            return instance;
        }
    }

    // Private constructor to prevent instantiation outside this class
    private InteractSystem() { }

    private void Start()
    {
        main = Camera.main;
        instance = FindObjectOfType<InteractSystem>();
    }
    // Update is called once per frame
    void Update()
    {
        // Cast a ray from the camera's position forward
        Ray ray = main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the ray hits something
        if (Physics.Raycast(ray, out hit, distance))
        {

           
            // Check if the hit object has the "Interact" tag
            if (hit.collider.CompareTag("Interactiable"))
            {
                E.SetActive(true);
                // Change the layer of the hit object and its children to layer 10
                ChangeLayerRecursively(hit.collider.gameObject.transform, 9);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    hit.collider.gameObject.GetComponent<InteractiableObject>().Interact();
                }

            }
        }
        else
        {
            E.SetActive(false);
        }
    }

    public Vector3 CastRayCast(float distance,LayerMask layer)
    {
        // Cast a ray from the camera's position forward
        Ray ray = main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the ray hits something
        if (Physics.Raycast(ray, out hit, distance,layer))
        {
            return hit.point;
        }
        else
        {
            return Vector3.zero;

        }
    }
    public bool CastRayCastBool(float distance, LayerMask layer)
    {
        // Cast a ray from the camera's position forward
        Ray ray = main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the ray hits something
        if (Physics.Raycast(ray, out hit, distance, layer))
        {
            return true;
        }
        else
        {
            return false;

        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(main.transform.position, main.transform.forward * distance + main.transform.position);
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
}
