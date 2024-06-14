using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellInstance : MonoBehaviour
{
    [Header("Delayed")]
    public bool delayedUncast;
    public float waitTimeAfterUncast;
    private bool oneTimeDelay;

    [Header("Spell Properties")]
    public float spellTime;
    public Animator spellAnim;


    public SpellInstance(bool delayedUncast,float waitTaimeAfterUncast, float spellTime,Animator spellAnim)
    {
        this.delayedUncast = delayedUncast;
        this.waitTimeAfterUncast = waitTaimeAfterUncast;
        this.spellTime = spellTime;
        this.spellAnim = spellAnim;
    }
     
    private void Update()
    {
        if (delayedUncast)
        {

            if (spellTime > 0)
            {
                spellTime-= Time.deltaTime;

            }
            else
            {
                if (oneTimeDelay == false)
                {
                    oneTimeDelay = true;
                    DelayedUnCast();
                }
                if (Mathf.Abs(spellTime) < waitTimeAfterUncast)
                {
                    spellTime -= Time.deltaTime;

                }
                else
                {
                    UnCast();
                }
            }
        }
        else
        {
            if (spellTime > 0)
            {
                spellTime -= Time.deltaTime;
                if (spellTime<= 0)
                {
                    // When timer reaches zero, uncast the spell
                    UnCast();
                }
            }
        }
    }

    public void UnCast()
    {
        Debug.Log("xddd");
        Destroy(this.gameObject);

    }
    public  void DelayedUnCast()
    {
    
        if (spellAnim != null)
            spellAnim.SetTrigger("Uncast");

    }
}
