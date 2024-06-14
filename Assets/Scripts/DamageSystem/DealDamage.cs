// DealDamage.cs
using UnityEngine;

public class DealDamage : MonoBehaviour
{
    public float Damage;
    public DmgType dmgType; // Reference to the damage type
    public bool destroyOnCollision;
    public LayerMask faction;
    public bool projectile = false;
    public GameObject impact;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != faction && destroyOnCollision == false)
        {
            if(projectile == true)
            {
                Instantiate(impact, other.ClosestPoint(transform.position), Quaternion.identity);
            }

            Destroy(this.gameObject);
        }
    }
}
