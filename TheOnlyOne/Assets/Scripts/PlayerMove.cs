using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private WeaponHolder weaponChanger;
    private Vector3 weaponChangerOrigin;
    private float headBobTime;
    //private float idleTime;
    public float headBobAmplitude=0.005f;
    public float headBobSpeed;
    public float headBobAimDividerX;
    public float headBobAimDividerY;
    Vector3 targetBobPosition;
    [SerializeField] Transform orientation;
    //input
    float horizontalMove;
    float verticalMove;
    public bool isAiming;
    [Header("Movement")]
    public float moveSpeed;
    public float idleSpeed = 0;
    public float walkSpeed=150;
    public float crouchSpeed = 75;
    public float runSpeed=200;
    public float accel = 10f;

    // public float maxSpeed=100f;

    public bool isSprinting;
    public bool isGrounded;
    public bool isCrouching;
    public bool isJumping;

    //RigidBody
    public float rbDrag=10f; 
    private Rigidbody rb;

    //Scale and direction
    public Vector3 playerScale;
    public Vector3 crouchScale=new Vector3(1,0.5f,1);
    Vector3 moveDirection;
    public Transform cameraRot;
    public Vector3 crouchedCamPos;
    //jump
    public float jumpForce;
    public bool readyToJump=true;
    public float jumpCooldown=0.6f;
    //slide
    public float slideForce = 400;
    //Drag
    public float defaultDrag = 10f;
    public float airDrag = 2f;
    public float slideDrag = 0f;

    //ground check
    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask Ground;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        weaponChanger = FindObjectOfType<WeaponHolder>();
        weaponChangerOrigin = weaponChanger.transform.localPosition;
        playerScale = transform.localScale;
        crouchedCamPos = new Vector3(cameraRot.position.x, cameraRot.position.y-0.7f, cameraRot.position.z);
    }
    private void Update()
    {
        MyInput();
        ControlDrag();
        ControlSpeed();
        ControlHeadBob();
        //jump
        if (isJumping && isGrounded && readyToJump) Jump();

    }
    void FixedUpdate()
    {
        MovePlayer();
       
    }
    public void MyInput()
    {
        //Input direccion
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");
        if (!weaponChanger.isEmpty() && weaponChanger.GetCurrentItem().typeOfItem == GameUtils.TypeOfItem.GUN)
        {
            isAiming = weaponChanger.GetCurrentItem().gameObject.GetComponent<Weapon>().isAming;
        }
        else isAiming = false;
        isJumping = Input.GetButton("Jump");
        isCrouching = Input.GetKey(KeyCode.LeftControl);
        isSprinting = Input.GetKey(KeyCode.LeftShift);
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, Ground);

        //Crouching
        if (Input.GetKeyDown(KeyCode.LeftControl))
            Crouch(true);

        if (Input.GetKeyUp(KeyCode.LeftControl))
            Crouch(false);
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
        CapsuleCollider coll = GetComponentInChildren<CapsuleCollider>();
        if (crouch)
        {
            Vector3.Lerp(cameraRot.position, crouchedCamPos, Time.deltaTime);
            coll.height = 1f;
            //moveSpeed = crouchSpeed;
           // coll.center.Set(0, -0.4f, 0);

        }
        else
        {
            Vector3.Lerp(crouchedCamPos,cameraRot.position, Time.deltaTime);
            coll.height = 2f;
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
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
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
            if (isCrouching)
                multiplier = 0f;
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
            if (isSprinting && isGrounded)
            {
                desiredSpeed = runSpeed;
            }
            else
            if (isCrouching && isGrounded)
            {
                desiredSpeed = crouchSpeed;
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
