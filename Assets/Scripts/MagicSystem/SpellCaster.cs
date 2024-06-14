using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCaster : MonoBehaviour
{
   
 
    
    [SerializeField] Transform leftHand;
    [SerializeField] Transform rightHand;
    [SerializeField] Transform player;
    public Animator anim;
    public GameObject beam;
    public Spell spell;
    public bool button = false;
    public IEnumerator CastSpell()
    {
        spell = FindObjectOfType<PL_Inventory>()._equipedGrimoire.spell;
        ApplyOverrride();
        anim.SetTrigger("SpellAttack");
        yield return new WaitForSeconds(spell.waitTimeBeforeSpawn);
        CinemachineShake.Instance.ShakeCamera(FindObjectOfType<PL_Inventory>()._equipedGrimoire.spell.screenShake, 0.4f);
        FindObjectOfType<PL_Stats>().mana -= spell.manaCost;
        spell.Cast(rightHand.position, Camera.main.transform.rotation, rightHand);
        
    }
    public IEnumerator CastSpellBeam()
    {
        spell = FindObjectOfType<PL_Inventory>()._equipedGrimoire.spell;
        
        ApplyOverrride();
        anim.SetBool("BeamAttack", true);
        yield return new WaitForSeconds(spell.waitTimeBeforeSpawn);
        Debug.Log("CAsttt");
        spell.Cast(rightHand.position, Camera.main.transform.rotation, rightHand);

    }

    public void Update()
    {
        if(spell != null)
        spell.UpdateSpell();

        if(button== true)
        {
            button = false;
            spell.UpdateSpell();
            StartCoroutine(CastSpell());
        }

    }

    public void ApplyOverrride()
    {
        AnimatorOverrideController aoc = new AnimatorOverrideController(anim.runtimeAnimatorController);
        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();

        for (int i = 0; i < aoc.animationClips.Length; i++)
        {
            if (aoc.animationClips[i].name == "Player_LightAttack1")
            {
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(aoc.animationClips[i], FindObjectOfType<PL_Inventory>()._equipedGrimoire.spell.animClip));
            }
            else
            {
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(aoc.animationClips[i], anim.runtimeAnimatorController.animationClips[i]));
            }
        }

        aoc.ApplyOverrides(anims);
        anim.runtimeAnimatorController = aoc;

    }
}
