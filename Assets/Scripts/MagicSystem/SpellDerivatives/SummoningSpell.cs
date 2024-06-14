using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
[CreateAssetMenu(fileName = "SummoningSpell", menuName = "Spells/SummoningSpell", order = 1)]
public class SummoningSpell : Spell
{
   
    private static SummoningSpell instance;
    public float distance;
    public LayerMask summonableGround;
    private Animator spellAnim;
  
    private SummoningSpell()
    {
        // Private constructor to prevent instantiation
    }

    public static SummoningSpell Instance
    {
        get
        {
            if (instance == null)
            {
                instance = ScriptableObject.CreateInstance<SummoningSpell>();
            }
            return instance;
        }
    }

    public override void Cast(Vector3 position, Quaternion rotation, Transform parent)
    {
       


        if (InteractSystem.Instance.CastRayCastBool(distance, summonableGround))
        {
            var pos = InteractSystem.Instance.CastRayCast(distance, summonableGround);
            GameObject Summon = MonoBehaviour.Instantiate(Prefab, pos, Quaternion.Euler(0f,Camera.main.transform.rotation.eulerAngles.y + 90f,0));
           
            spellGameObject = Summon;

            SetDmg(Summon.transform, dmg);
            base.Cast(position, Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0),parent);
          
        }
        
       
    }

    public override void UpdateSpell()
    {
        base.UpdateSpell();
     

    }

}
