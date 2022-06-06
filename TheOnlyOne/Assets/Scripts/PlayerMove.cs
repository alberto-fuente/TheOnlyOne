using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private ItemHolder weaponChanger;
    private Vector3 weaponChangerOrigin;
    [SerializeField] private Camera cam;
    [SerializeField] private float normalFov;
    [SerializeField] private float slideFov;

    public  Vector3 idleItemHolderRotation;
    public Vector3 sprintItemHolderRotation;

    private float headBobTime;
    //private float idleTime;
    public float headBobAmplitude=0.005f;
    float headBobSpeed;
    float headBobAimDividerX;
    float headBobAimDividerY;
    Vector3 targetBobPosition;
    [SerializeField] Transform orientation;
    public Animator armsAnimator;
    public Animator wholeAnimator;
    //input
    float horizontalMove;
    float verticalMove;
    public bool isAiming;
    [Header("Movement")]
    public float moveSpeed;
    public float idleSpeed = 0;
    public float walkSpeed=150;
    public float crouchSpeed = 10;
    public float runSpeed=200;
    public float accel = 10f;
    public float defaultJumpForce = 30;
    public float bounceJumpForce = 130;

    // public float maxSpeed=100f;

    public bool isSprinting;
    public bool isGrounded;
    public bool isCrouching;
    public bool isJumping;
    //public bool isSliding;
    public bool isOnPad;

    //RigidBody
    public float rbDrag=10f; 
    private Rigidbody rb;
    public Transform mesh;
    //Scale and direction
    public Vector3 standScale = new Vector3(1, 2f, 1);
    public Vector3 crouchScale=new Vector3(1,0.7f,1);
    Vector3 moveDirection;
    public Transform cameraRot;
    public Vector3 crouchedCamPos;
    //jump
    public float jumpForce;
    public bool readyToJump=true;
    public float jumpCooldown=0.6f;
    //slide
    public float slideForce = 100;
    //Drag
    public float defaultDrag = 10f;
    public float airDrag = 2f;
    public float slideDrag = 0.1f;

    //ground check
    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask Ground;
    public LayerMask JumpPad;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        weaponChanger = FindObjectOfType<ItemHolder>();
        weaponChangerOrigin = weaponChanger.transform.localPosition;
        idleItemHolderRotation = Vector3.zero;
        sprintItemHolderRotation = new Vector3(0, -60f, 0);
        crouchedCamPos = new Vector3(cameraRot.position.x, cameraRot.position.x-crouchScale.y, cameraRot.position.z);
    }
    private void Update()
    {
        MyInput();
        ControlDrag();
        ControlSpeed();
        ControlHeadBob();
        //jump
        if (isJumping && (isGrounded||isOnPad) && readyToJump) Jump();
        

    }
        void FixedUpdate()
    {
        MovePlayer();
        Crouch(isCrouching);
       /* if (isSprinting && !isAiming)
        {
            weaponChanger.transform.localEulerAngles = Vector3.Lerp(sprintItemHolderRotation, idleItemHolderRotation, Time.deltaTime*20);
        }
        else
        {
            weaponChanger.transform.localEulerAngles = Vector3.Lerp(idleItemHolderRotation, sprintItemHolderRotation, Time.deltaTime*20);
        }
       */
    }
    public void MyInput()
    {
        //Input direccion
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");
        if (weaponChanger.GetCurrentItem()!=null && weaponChanger.GetCurrentItem().typeOfItem == GameUtils.TypeOfItem.GUN)
        {
            isAiming = weaponChanger.GetCurrentItem().gameObject.GetComponent<Weapon>().isAming;
        }
        else isAiming = false;
        isJumping = Input.GetButton("Jump");
        isCrouching = Input.GetKey(KeyCode.LeftControl);
        isSprinting = Input.GetKey(KeyCode.LeftShift);
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, Ground);
        isOnPad = Physics.CheckSphere(groundCheck.position, groundDistance, JumpPad);



    }

    void ControlHeadBob()
    {
        //la velocidad del headBob se calcula en función de la velocidad del jugador
        headBobSpeed = Mathf.Max(moveSpeed / 30, 1);
        headBobAimDividerX = isAiming ? 10 : 1;
        headBobAimDividerY = isAiming ? 5 : 1;

       if(isGrounded || isCrouching)
        {
            headBobTime += Time.deltaTime * headBobSpeed;
            weaponChanger.transform.localPosition = Vector3.Lerp(weaponChanger.transform.localPosition, HeadBob(headBobTime, headBobAmplitude / headBobAimDividerX * headBobSpeed, headBobAmplitude / headBobAimDividerY * headBobSpeed), Time.deltaTime * 5);
        }
        
    }
    Vector3 HeadBob(float timePoint, float xIntensity, float yIntensity)
    {
        targetBobPosition = weaponChangerOrigin + new Vector3(Mathf.Cos(timePoint) * xIntensity, Mathf.Sin(timePoint * 2) * yIntensity, -headBobSpeed/90);
        return targetBobPosition;
    }
    private void Crouch(bool crouch)
    {
        if (crouch)
        {
            Vector3.Lerp(cameraRot.position, crouchedCamPos, Time.deltaTime);
            mesh.localScale = crouchScale;
            /*if (moveSpeed > walkSpeed/2 && !isSliding){
                isSliding = true;
                rb.AddForce(transform.forward * slideForce, ForceMode.VelocityChange);
              //  camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, slideFov, 20 * Time.deltaTime);
            }
            if (moveSpeed == 0) isSliding = false;*/
            
            //moveSpeed = crouchSpeed;
           // coll.center.Set(0, -0.4f, 0);

        }
        else
        {
            //isSliding = false;
           // camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, normalFov, 20 * Time.deltaTime);
            Vector3.Lerp(crouchedCamPos,cameraRot.position, Time.deltaTime);
            mesh.localScale = standScale;
            //coll.center.Set(0,0,0);
        }
        
    }

    void MovePlayer()
    {  
        //Añadir gravedad extra
        rb.AddForce(-transform.up * Time.deltaTime * 20,ForceMode.VelocityChange);

        //direccion donde caminar
        moveDirection = orientation.forward * verticalMove + orientation.right * horizontalMove;//(horizontalMove,0,verticalMove)
        moveDirection.Normalize();

        //aplicar fuerza sobre el rigidBody del jugador para que se mueva
        rb.AddForce(moveDirection * moveSpeed * GetMultiplier() * Time.deltaTime, ForceMode.Impulse);      
        
    }
    public void Jump()
    {
        armsAnimator.Play("Jump");
        wholeAnimator.Play("Jump");
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        jumpForce = isOnPad ? bounceJumpForce : defaultJumpForce;
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        readyToJump = false;
        Invoke("ResetJump", jumpCooldown);
    }
    public void ResetJump()
    {
        readyToJump = true;
    }
    //Multiplicador
    public float GetMultiplier()
    {
        float multiplier;
        if (isGrounded)
        {
            //if (isSliding)
            //    multiplier = 0f;
            if(isCrouching)
                multiplier = 0.7f;
            else
                multiplier = 1f;
        }
        else multiplier=0.2f;

        return multiplier;
    }
    void ControlDrag()
    {
        if (isGrounded) {
            if (isCrouching)
                rb.drag = slideDrag;
            else
            rb.drag = defaultDrag;
        } 
        else rb.drag = airDrag;
    }
    void ControlSpeed()
    {
        int aimDivider = isAiming?3:1;
        float desiredSpeed;
        if (horizontalMove != 0 || verticalMove != 0)
        {
            if (isCrouching && isGrounded)
            {
                desiredSpeed = crouchSpeed;
            }
            else
             if (isSprinting && isGrounded)
            {
                desiredSpeed = runSpeed;
            }
            else
            {
                desiredSpeed = walkSpeed;
            }
        }
        else desiredSpeed = idleSpeed;

        moveSpeed = Mathf.Lerp(moveSpeed, desiredSpeed / aimDivider, accel * Time.deltaTime);
    }
  
}
