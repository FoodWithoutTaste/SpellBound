
using System.Collections.Generic;
using UnityEngine;

public class HPType : MonoBehaviour
{
    public List<DmgTypeHolder> weaknesses;
    public List<DmgTypeHolder> resistances;

    public void TakeDamage(float damage, DmgType damageTypeOfIncomingAttack)
    {
      
        float modifiedDamage = ApplyWeaknessesAndResistances(damage, damageTypeOfIncomingAttack);
        GetComponent<Stats>().hp -= modifiedDamage;
    }

    private float ApplyWeaknessesAndResistances(float damage, DmgType damageTypeOfIncomingAttack)
    {
        float modifiedDamage = damage;

      
        foreach (DmgTypeHolder weakness in weaknesses)
        {
            if (weakness.dmgType == null) continue; 

           
            if (weakness.dmgType == damageTypeOfIncomingAttack)
            {
                modifiedDamage *= (1.0f - weakness.value); 
            }
        }

       
        foreach (DmgTypeHolder resistance in resistances)
        {
            if (resistance.dmgType == null) continue;

          
            if (resistance.dmgType == damageTypeOfIncomingAttack)
            {
                modifiedDamage *= (1.0f + resistance.value); 
            }
        }

        return modifiedDamage;
    }
}
