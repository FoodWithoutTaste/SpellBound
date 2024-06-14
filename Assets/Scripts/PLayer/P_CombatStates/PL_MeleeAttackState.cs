using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class PL_MeleeAttackState : PL_AttackState
{

    public PL_MeleeAttackState(PL_CombatSm stateMachine) : base("PL_MeleeAttackState", stateMachine) { }
    private bool doOnce = false;
    private bool doOnce2 = false;
    public override void Enter()
    {
        base.Enter();
       _sm.movementSm.RotateTowardsCamera();
        _sm.movementSm.ChangeStates(_sm.movementSm.pl_IdleState);
        _sm.equipSystem.ChangeHandSlot(_sm.equipSystem.RightHandSlot, _sm.inventoryRefrence.equipedMeleeWeapon);
       
        _sm.anims.SetBool("Combat", true);

        _sm.anims.SetTrigger("MeleeAttack");
        doOnce = false;
        _sm.playerMeleeController.SetActive(false);

        timer = _sm.comboManager.GetCurrentClip().length;
        maxTimer = timer;
        doOnce2 = false;


    }
    AnimationClip FindAnimClipByName(string name, AnimationClip[] ans)
    {
        var anim = new AnimationClip();
        for (int i = 0; i < ans.Length; i++)
        {
            if(ans[i].name == name)
                anim = ans[i];
        }
        return anim;
    }
    public override void LogicUpdate()
    {
        if (Mathf.Abs(timer - maxTimer) > 0.29f && doOnce2 == false)
        {
            doOnce2 = true;
          
        }

        if (timer >= 0 && started)
        {
            timer -= Time.deltaTime;
        } 
       
           
        
        if (timer < 0)
        {
            stateMachine.ChangeStates(((PL_CombatSm)stateMachine).pl_defaultState);
        }
        
            if (doOnce == false)
            {
                doOnce = true;
                _sm.movementSm.AddForwordForce(10f);
                _sm.playerMeleeController.SetActive(true);
            }
            if (timer <= maxTimer / 4f)
            {
                _sm.playerMeleeController.SetActive(false);
            }
        
       




    }
    public override void Exit()
    {
        base.Exit();
        _sm.playerMeleeController.SetActive(false);
        doOnce = false;
        _sm.comboManager.ProgressCombo();
        _sm.anims.SetBool("Combat", false);
        _sm.pl_defaultState.AttackCooldown = 1f;
        GameObject.Find("CombatPlayerFollowMelee").GetComponent<CinemachineVirtualCamera>().Priority = 9;
        doOnce2 = false;
    }

  
}
