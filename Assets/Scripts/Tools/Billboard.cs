using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;
    // Update is called once per frame
    private void Start()
    {
        // Get the main camera
        mainCamera = Camera.main;
    }
    void Update()
    {
        if (mainCamera != null)
        {

            // Calculate the direction from the object to the camera
            Vector3 lookDir = mainCamera.transform.position - transform.position;

            // Make the object look toward the camera only on the Y axis (keep its original rotation on other axes)
            transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
        }
       
    }  
}
