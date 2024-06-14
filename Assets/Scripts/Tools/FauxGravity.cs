using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FauxGravity : MonoBehaviour
{
    public bool isPlanet = true;
    public float gravity = 10f; // Strength of the gravity
    public Transform gravityAttractor; // Reference to the planet
    public float rotationSpeed = 5f; // Speed of the rotation alignment
    public PlanetGeneration table;
    private Rigidbody rb;
    public Vector3 fauxDirection;
    public bool g = false;
    IEnumerator Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Disable default gravity
        rb.constraints = RigidbodyConstraints.FreezeRotation; // Prevent physics from rotating the player
        
        yield return new WaitForSeconds(10f);
        if (g == true)
        {
            gravity = 50f;
            GetComponent<ChunkRenderManager>().range = 5;
        }
    }

    void FixedUpdate()
    {
        if (isPlanet)
        {
            // Apply custom gravity
            if (table != null)
            {
                Vector3 gravityDirection = (table.center * table.gameObject.transform.localScale.x - transform.position).normalized;
                fauxDirection = gravityDirection;
                rb.AddForce(gravityDirection * gravity);

                // Align player's up direction with gravity direction
                Vector3 localUp = transform.up;
                Quaternion targetRotation = Quaternion.FromToRotation(localUp, -gravityDirection) * transform.rotation;
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime));
            }
            else
            {
                Vector3 gravityDirection = (gravityAttractor.position - transform.position).normalized;
                rb.AddForce(gravityDirection * gravity);

                // Align player's up direction with gravity direction
                Vector3 localUp = transform.up;
                Quaternion targetRotation = Quaternion.FromToRotation(localUp, -gravityDirection) * transform.rotation;
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime));
            }
        }
        else
        {
            fauxDirection = Vector3.down;
                rb.AddForce(fauxDirection * gravity);
        }
       
       
    }
}
