using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class PL_SpellAttackState1 : PL_AttackState
{
    public PL_SpellAttackState1(PL_CombatSm stateMachine) : base("PL_PL_SpellAttackState1", stateMachine) { }
    private bool doOnce = false;

    public override void Enter()
    {
        base.Enter();
       
        Debug.Log("Nigger");
        GameObject.Find("CombatPlayerFollowMagic").GetComponent<CinemachineVirtualCamera>().Priority = 12;
        _sm.anims.SetBool("Combat", true);
        _sm.animLayerController.ChangeLayers(1, 1);
        _sm.spellCaster.StartCoroutine("CastSpellBeam");
        doOnce = false;
        timer = 10000f;

    }
    public override void LogicUpdate()
    {
        CinemachineShake.Instance.ShakeCamera(1.5f, 0.4f);
        if (Input.GetMouseButton(0))
        {


          


        }
        else
        {
            stateMachine.ChangeStates(_sm.pl_aimState);
        }

    }
    public override void Exit()
    {
        base.Exit();
        _sm.anims.SetBool("Combat", false);
        _sm.animLayerController.ChangeLayers(1, 0);
        GameObject.Find("CombatPlayerFollowMagic").GetComponent<CinemachineVirtualCamera>().Priority = 9;
       _sm.anims.SetBool("BeamAttack", false);
        _sm.anims.SetTrigger("BeamEnd");
        if(_sm.spellCaster.beam != null)
        _sm.spellCaster.beam.GetComponent<SpellInstance>().UnCast();
    }

   

}
