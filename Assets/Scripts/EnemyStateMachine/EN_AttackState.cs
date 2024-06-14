using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EN_AttackState :BaseState
{
    protected float timer = 0;
    protected float maxTimer;
    protected bool started = false;
    public EN_MovementSm _sm;
    private bool doOnce = false;
    int a;
    public EN_AttackState(EN_MovementSm stateMachine) : base("EN_AttackState", stateMachine) {
        _sm = (EN_MovementSm)stateMachine;

    }

    public override void Enter()
    {
        base.Enter();
        timer = 1.2f;
        doOnce = false;
        maxTimer = timer;
        started = true;
        if(_sm.name == "Golem")
        {
            a = Random.Range(0, 2);
            if(a == 0)
            {
                _sm.attackCol.SetActive(false);
                _sm.anims.SetTrigger("MeleeAttack1");
            }
            else
            {
                _sm.attackCol.SetActive(false);
                _sm.anims.SetTrigger("Golem_LaserAttack1");
            }
        }
        else
        {
            _sm.attackCol.SetActive(false);
            _sm.anims.SetTrigger("MeleeAttack1");
        }

      
    }


    public override void LogicUpdate()
    {
      
        


        base.LogicUpdate();
        if (timer >= 0 && started)
        {
            timer -= Time.deltaTime;
        }
        if (timer < 0)
        {
            stateMachine.ChangeStates(_sm.en_IdleState);
        }
        if (doOnce == false &&  timer <= maxTimer / 2f )
        {
            doOnce = true;
            if(_sm.name == "Golem"){
                if(a== 0)
                {
                    
                    _sm.attackCol.SetActive(true);
                    CinemachineShake.Instance.ShakeCamera(2, 0.75f);
                }
             
            }
            else
            {
                _sm.attackCol.SetActive(true);
                if (_sm.target != null)
                {
                    Quaternion newquat = Quaternion.LookRotation(_sm.target.position - _sm.transform.position);
                    newquat = Quaternion.Euler(0, newquat.eulerAngles.y, 0); 

                    _sm.visuals.transform.rotation = newquat;
                }
            }
          
        }
        if (timer <= maxTimer / 3f)
        {
            _sm.attackCol.SetActive(false);
        }


    }
    public override void Exit()
    {
        base.Exit();
        _sm.timer = _sm.maxTimer * 2;
        started = false;
        timer = 0;

    }
}
