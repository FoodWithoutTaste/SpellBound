using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class PL_SpellAttackState : PL_AttackState
{
    public PL_SpellAttackState(PL_CombatSm stateMachine) : base("PL_PL_SpellAttackState", stateMachine) { }
    private bool doOnce = false;

    public override void Enter()
    {
        base.Enter();
        _sm.anims.SetBool("BeamAttack", false);

        GameObject.Find("CombatPlayerFollowMagic").GetComponent<CinemachineVirtualCamera>().Priority = 12;
        _sm.anims.SetBool("Combat", true);
        _sm.animLayerController.ChangeLayers(1, 1);
        _sm.spellCaster.StartCoroutine("CastSpell");
        doOnce = false;
        timer = 1f;
        maxTimer = timer;

    }
    public override void LogicUpdate()
    {
        if (timer >= 0 && started)
        {
            timer -= Time.deltaTime;
        }
        if (timer < 0)
        {
            stateMachine.ChangeStates(((PL_CombatSm)stateMachine).pl_aimState);
        }

    }
    public override void Exit()
    {
        base.Exit();
        _sm.anims.SetBool("Combat", false);
        _sm.animLayerController.ChangeLayers(1, 0);
        GameObject.Find("CombatPlayerFollowMagic").GetComponent<CinemachineVirtualCamera>().Priority = 9;
    }

   

}
