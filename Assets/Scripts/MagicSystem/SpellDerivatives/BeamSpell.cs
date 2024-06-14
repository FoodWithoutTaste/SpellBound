using UnityEngine;
using System.Collections;
using System;
[CreateAssetMenu(fileName = "ProjectileSpell", menuName = "Spells/BeamSpell ", order = 1)]
public class BeamSpell : Spell
{
  
    private static BeamSpell instance;

    private BeamSpell()
    {
        // Private constructor to prevent instantiation
    }

    public static BeamSpell Instance
    {
        get
        {
            if (instance == null)
            {
                instance = ScriptableObject.CreateInstance<BeamSpell>();
            }
            return instance;
        }
    }

    public override void Cast(Vector3 position, Quaternion rotation,Transform parent)
    {
      

        GameObject bullet = Instantiate(Prefab, position, rotation,parent);
        MonoBehaviour.FindObjectOfType<SpellCaster>().beam = bullet;
        spellGameObject = bullet;
        base.Cast(position, rotation,parent);
        bullet.GetComponent<DealDamage>().Damage = dmg;

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
