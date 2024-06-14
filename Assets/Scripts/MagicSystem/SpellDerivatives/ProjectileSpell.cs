using UnityEngine;
using System.Collections;
using System;
[CreateAssetMenu(fileName = "ProjectileSpell", menuName = "Spells/ProjectileSpell", order = 1)]
public class ProjectileSpell : Spell
{
    public float projectileSpeed;
    private static ProjectileSpell instance;

    private ProjectileSpell()
    {
        // Private constructor to prevent instantiation
    }

    public static ProjectileSpell Instance
    {
        get
        {
            if (instance == null)
            {
                instance = ScriptableObject.CreateInstance<ProjectileSpell>();
            }
            return instance;
        }
    }

    public override void Cast(Vector3 position, Quaternion rotation, Transform parent)
    {
      

        GameObject bullet = Instantiate(Prefab, position, rotation);
        spellGameObject = bullet;
        base.Cast(position, rotation, parent);
        bullet.GetComponent<DealDamage>().Damage = dmg;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity))
        {
            Vector3 bulletDirection = hit.point - position;
            bulletDirection.Normalize();
            bullet.GetComponent<Rigidbody>().velocity = bulletDirection * projectileSpeed;
        }
        else
        {
            Vector3 bulletDirection = (Camera.main.transform.position + Camera.main.transform.forward * 1000) - position;
            bulletDirection.Normalize();
            bullet.GetComponent<Rigidbody>().velocity = bulletDirection * projectileSpeed;
        }
    }

    public override void UpdateSpell()
    {
        base.UpdateSpell();
     
       
    }
  

    public override void UnCast()
    {
        base.UnCast();
        Destroy(spellGameObject);

    }
}
