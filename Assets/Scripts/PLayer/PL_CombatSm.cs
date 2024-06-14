using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PL_CombatSm : StateMachine
{
    [HideInInspector]
    public PL_AimState pl_aimState;
    [HideInInspector]
    public PL_MeleeBlock pl_meleeBlock;

    [HideInInspector]
    public PL_DefaultState pl_defaultState;

    [HideInInspector]
    public PL_MeleeAttackState pl_MeleeAttackState;
          
    [HideInInspector]
    public PL_SpellAttackState pl_spellAttackState;
    [HideInInspector]
    public PL_SpellAttackState1 pl_spellAttackState1;

    [Header("Refrences")]
    public PL_Inventory inventoryRefrence;
    public SpellCaster spellCaster;
    public PL_MovementSm movementSm;
    public PL_EquipSystem equipSystem;
    [SerializeField] Transform visuals;
    public Animator anims;
    public AnimLayerController animLayerController;
    public GameObject manaBolt;
    public Transform boltSpawn;
    public float bulletSpeed;
    public LayerMask boltLayer;
    public GameObject playerMeleeController;
    public Avatar ava;
    public PL_ComboManager comboManager;
    public PL_Stats stats;
    protected override BaseState GetInitialState()
    {
        return pl_defaultState;
    }

    private void Awake()
    {
        pl_aimState = new PL_AimState("AimState",this);
        pl_defaultState= new PL_DefaultState(this);

        pl_MeleeAttackState = new PL_MeleeAttackState(this);
       
        pl_meleeBlock = new PL_MeleeBlock("meleeBlock", this);
        pl_spellAttackState = new PL_SpellAttackState(this);
        pl_spellAttackState1 = new PL_SpellAttackState1(this);
    }

    // Update is called once per frame
    public override void Update()
    {

        base.Update();
      
        
    }
}
