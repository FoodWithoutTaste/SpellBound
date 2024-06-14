using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class PL_ComboManager : MonoBehaviour
{
    public PL_MeleeCombo currentMeleeCombo;
    public List<PL_MeleeCombo> meleeCombos;
    public VisualEffect effect;
    public int comboProgress = 0; // Tracks the progress of the current combo
   
    public Animator anim;
    private void Start()
    {
        ChooseCombo();
     
    }
    private void Update()
    {
      
    }
    public void ApplyOverrride()
    {
        AnimatorOverrideController aoc = new AnimatorOverrideController(anim.runtimeAnimatorController);
        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();

        for (int i = 0; i < aoc.animationClips.Length; i++)
        {
            if (aoc.animationClips[i].name == "Player_SlashLeft")
            {
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(aoc.animationClips[i], GetCurrentClip()));
            }
            else
            {
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(aoc.animationClips[i], anim.runtimeAnimatorController.animationClips[i]));
            }
        }

        aoc.ApplyOverrides(anims);
        anim.runtimeAnimatorController = aoc;

    }


    private void ChooseCombo()
    {
        currentMeleeCombo = meleeCombos[Random.Range(0, meleeCombos.Count)];
        comboProgress = 0;
    }

    public void ProgressCombo()
    {
        if(currentMeleeCombo.animCLips.Count-1 != comboProgress)
        {
            comboProgress++;
            ApplyOverrride();
        }else if (comboProgress == currentMeleeCombo.animCLips.Count - 1)
        {
            ChooseCombo();
            ApplyOverrride();
        }

     
    }
    public AnimationClip GetCurrentClip()
    {
        return currentMeleeCombo.animCLips[comboProgress];
        
    }
}
