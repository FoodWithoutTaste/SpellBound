using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
public class EN_MovementSm : StateMachine
{
    //States
    [HideInInspector]
    public EN_IdleState en_IdleState;
    [HideInInspector]
    public EN_FollowState en_FollowState;
    [HideInInspector]
    public EN_AttackState en_AttackState;

    public string name;

    [Header("Properties")]
    public Transform visuals;
    public Animator anims;
    public NavMeshAgent aiAgent;
    public float moveSpeed;
    public bool isInFollowRange;
    public bool isInAttackRange;
    public float followRange;
    public float attackRange;
    public LayerMask playerLayer;
    public Transform target;
    public GameObject attackCol;

    public float timer = 0;
    public float maxTimer;
    public CinemachineShake shaker;
    public List<Transform> targets;
    private void Awake()
    {
        en_IdleState = new EN_IdleState(this);
        en_FollowState = new EN_FollowState(this);
        en_AttackState = new EN_AttackState(this);
        timer = 1f;
      
        maxTimer = timer;
    }
    void chooseEnemy()
    {
        if(targets.Count > 0)
        target = targets[Random.Range(0, targets.Count)].transform;
 
    }
    protected override BaseState GetInitialState()
    {
        return en_IdleState;
    }
    public override void Update()
    {
        base.Update();
        if (timer >= 0)
        {
            timer -= Time.deltaTime;
        }
        Collider[] cols = Physics.OverlapSphere(transform.position, followRange, playerLayer);
        if(cols.Length > 0)
        {
            target = cols[Random.Range(0, cols.Length)].transform;
            targets.Clear();
            foreach(var t in cols)
            {
                targets.Add(t.transform);
            }
            if (target == null)
            {
                chooseEnemy();
            }
        }
        else
        {
            target = null;
        }
        
     
        isInFollowRange = Physics.CheckSphere(transform.position, followRange, playerLayer);
        isInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
        if (isInFollowRange){
            if (target != null)
                if(target.GetComponent<Stats>() != null)
                target.GetComponent<Stats>().inCombat = true;
        }
    }
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}
