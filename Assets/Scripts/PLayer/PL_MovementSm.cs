using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PL_MovementSm : StateMachine
{
    public FauxGravity gravityFaux;
    public bool useFaux;
    //States
    [HideInInspector]
    public PL_IdleState pl_IdleState;
    [HideInInspector]
    public PL_MoveState pl_MoveState;
    [HideInInspector]
    public PL_JumpingState pl_JumpingState;
    [HideInInspector]
    public PL_RunState pl_runState;
    [HideInInspector]
    public PL_FallState pl_fallState;
    [HideInInspector]
    public PL_GlidingState pl_glidingState;

    public PL_CombatSm combatSm;

    [Header("Properties")]
    public bool isGrounded;
    public LayerMask groundLayer;
    public float moveSpeed = 15.3f;
    public float runSpeed = 15.3f;
    private float movementMultiplier = 1000f;
    public float MoveSpeed{get { return moveSpeed * movementMultiplier; }set { moveSpeed = value; }}
    public float RunSpeed { get { return runSpeed * movementMultiplier; } set { runSpeed = value; } }

    public float jumpSpeed = 60;
    public float groundDistance = 3;

    [Header("Refrences")]
    public Rigidbody rb;
    public Collider coll;
    public Transform aim;
    public Transform orientation;

    [SerializeField] Transform visuals;
    public Animator anims;
    public Transform[] groundedRaycasts;
 
    

    [Header("Settings")]
    float groundDrag = 6f;
    float airDrag = 1.4f;
    [HideInInspector] public float jumpCoolDown = 0;
    [HideInInspector] public Vector3 slopeMoveDirection;
    [HideInInspector] public RaycastHit slopeHit;
     public float gravity = 70f;
    [HideInInspector] public float airMultiplier = 0.2f;
    [HideInInspector] public Vector3 movementInput;
    [HideInInspector] public float _horizontalInput;
    [HideInInspector] public float _verticalInput;
    [HideInInspector] public float turnSmoothTime = 0.1f;
    [HideInInspector] float turnSmoothVelocity;
    public Material mat;


    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        pl_IdleState = new PL_IdleState(this);
        pl_MoveState = new PL_MoveState(this);
        pl_JumpingState = new PL_JumpingState(this);
        pl_runState = new PL_RunState(this);
        pl_fallState = new PL_FallState(this);
        pl_glidingState = new PL_GlidingState(this);
        currentState = pl_IdleState;
      
    }
    public IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        GetComponent<SoundManager>().PlayMusic("Music1");

        GetComponent<Stats>().inCombat = false;
        if (mat != null)
        
            transform.position = new Vector3(FindObjectOfType<PlanetGeneration>().sizeOverAll * 9600, FindObjectOfType<PlanetGeneration>().sizeOverAll * 17082, FindObjectOfType<PlanetGeneration>().sizeOverAll * 9600);
    }
    protected override BaseState GetInitialState()
    {
        return pl_IdleState;
    }
   
    public override void Update()
    {
        base.Update();
        if(mat != null)
        {
            if (mat.GetVector("_playerPos") != null)
            {
                mat.SetVector("_playerPos", transform.position);
            }
            if (mat.GetVector("_Center") != null)
            {
                mat.SetVector("_Center", FindObjectOfType<PlanetGeneration>().center);

            }
        }
       





        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
        orientation.localRotation = Quaternion.Euler(0,aim.localRotation.eulerAngles.y,0);
        movementInput = orientation.forward * _verticalInput + orientation.right * _horizontalInput;

        isGrounded = IsGrounded();
        anims.SetBool("IsGrounded", isGrounded);
        
        anims.SetFloat("X", _horizontalInput);
        anims.SetFloat("Y", _verticalInput);
        rb.drag = isGrounded == true ? groundDrag : airDrag;
        slopeMoveDirection = Vector3.ProjectOnPlane(movementInput, slopeHit.normal);

        if (!isGrounded)
        {
            if (useFaux)
            {
            }
            else
            {
                rb.AddForce((transform.up * -1) * gravity, ForceMode.Acceleration);
            }
          
        }
       


        if (anims.GetBool("Combat"))
        {
            RotateTowardsCamera();
        }
        else
        {
            if (movementInput.magnitude > 0)
            {
                if(combatSm.currentState.GetType() == typeof(PL_MeleeAttackState))
                {

                }
                else
                {

                    Quaternion targetRotation1 = Quaternion.LookRotation(movementInput.normalized, transform.up);
                    visuals.rotation = Quaternion.Euler(visuals.rotation.eulerAngles.x, targetRotation1.eulerAngles.y, visuals.rotation.eulerAngles.z);
                    visuals.localRotation = Quaternion.Euler(0, visuals.localRotation.eulerAngles.y, 0);
                }

            }
        }
        
        
        if (jumpCoolDown > 0.1f)
        {
            jumpCoolDown -= Time.deltaTime;
        }
    }
    private bool IsGrounded()
    {
        bool isGrounded = false;

       // Iterate through each starting position for raycasts
        for (int i = 0; i < groundedRaycasts.Length; i++)
        {
            // Set up the raycast parameters
            Vector3 raycastOrigin = groundedRaycasts[i].position;
            Vector3 raycastDirection = transform.up* -1; // Raycast direction is down
            float raycastDistance = groundDistance; // Adjust the distance based on your needs

            // Perform the raycast with layer filtering
            RaycastHit hit;
            if (Physics.Raycast(raycastOrigin, raycastDirection, out hit, raycastDistance, groundLayer))
            {
                // Raycast hit an object on the specified ground layer
                isGrounded = true;
                break; // Break out of the loop since one hit is enough to consider the object grounded
            } 
        }
        return isGrounded;
    }
    private bool IsGroundedToApplyGrav()
    {
        bool isGrounded = false;

        // Iterate through each starting position for raycasts
        for (int i = 0; i < groundedRaycasts.Length; i++)
        {
            // Set up the raycast parameters
            Vector3 raycastOrigin = groundedRaycasts[i].position;
            Vector3 raycastDirection = transform.up * -1; // Raycast direction is down
            float raycastDistance = groundDistance /2; // Adjust the distance based on your needs

            // Perform the raycast with layer filtering
            RaycastHit hit;
            if (Physics.Raycast(raycastOrigin, raycastDirection, out hit, raycastDistance, groundLayer))
            {
                // Raycast hit an object on the specified ground layer
                isGrounded = true;
                break; // Break out of the loop since one hit is enough to consider the object grounded
            }
        }
        return isGrounded;
    }




    private void OnDrawGizmosSelected()
    {
       
        for (int i = 0; i < groundedRaycasts.Length; i++)
            {
                // Set up the raycast parameters
                Vector3 raycastOrigin = groundedRaycasts[i].position;
                Vector3 raycastDirection = transform.up * -1; // Raycast direction is down
                float raycastDistance = groundDistance; // Adjust the distance based on your needs
                 Gizmos.DrawLine(raycastOrigin ,raycastOrigin + raycastDirection * raycastDistance);
           
            }
    }


    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, transform.up * -1, out slopeHit, ((CapsuleCollider)coll).height / 2 + 3f))
        {
            if (slopeHit.normal != transform.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    public void AddForwordForce(float force)
    {
        rb.AddForce(Camera.main.transform.forward * force, ForceMode.Impulse);
    }
 
    void RotateVisuals(Vector3 direction)
    {
        // Calculate the rotation
        Quaternion targetRotation1 = Quaternion.LookRotation(movementInput);
        float angle = Mathf.SmoothDampAngle(visuals.eulerAngles.y, targetRotation1.eulerAngles.y, ref turnSmoothVelocity, turnSmoothTime);
        visuals.localRotation = Quaternion.Euler(0, angle, 0);


    }
    public void RotateTowardsCamera()
    {
        // Get the direction the camera is facing
        Vector3 cameraForward = Camera.main.transform.forward;

        // Calculate the rotation angle around the y-axis
        Quaternion targetRotation = Quaternion.LookRotation(cameraForward);

        // Apply rotation only on the y-axis to the player's visuals
        Quaternion yRotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);
        visuals.localRotation = yRotation;
    }
}
