using UnityEngine;

public class Spell : ScriptableObject
{
    [Header("Default")]
    protected GameObject spellGameObject;
    public string spellName; // Changed 'name' to 'spellName' to avoid conflicts
    public AnimationClip animClip;
    public GameObject Prefab;
    public float waitTimeBeforeSpawn;
    public float manaCost;
    [Header("Delayed")]
    public bool delayedUncast;
    public float waitTimeAfterUncast;
  

    [Header("Spell Properties")]
   
    public float spellTime;
    protected float spellTimeCurrent;
    public float coolDown;
    public float dmg;
    public float screenShake;
    public virtual void Cast(Vector3 position, Quaternion rotation,Transform parent)
    {
        spellTimeCurrent = spellTime;
        spellGameObject.AddComponent<SpellInstance>();
        spellGameObject.GetComponent<SpellInstance>().delayedUncast = delayedUncast;
        spellGameObject.GetComponent<SpellInstance>().waitTimeAfterUncast = waitTimeAfterUncast;
        spellGameObject.GetComponent<SpellInstance>().spellTime = spellTime;
        spellGameObject.GetComponent<SpellInstance>().spellAnim = spellGameObject.GetComponent<Animator>();
    }
    public void SetDmg(Transform trans, float dmg)
    {
        DealDamage dealDamageComponent1 = trans.GetComponent<DealDamage>();
        if (dealDamageComponent1 != null)
        {
            dealDamageComponent1.Damage = dmg;
        }

        foreach (Transform child in trans)
        {
            DealDamage dealDamageComponent = child.GetComponent<DealDamage>();
            if (dealDamageComponent != null)
            {
                dealDamageComponent.Damage = dmg;
            }
            SetDmg(child, dmg);
        }
    }
    public virtual void UnCast()
    {
       
    }
    public virtual void DelayedUnCast()
    {
       

    }

    public virtual void UpdateSpell()
    {
       
      
    }

}
