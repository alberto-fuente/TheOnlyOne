using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("References")]
    private PlayerInventory playerInventory;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform orientation;
    public Animator armsAnimator;

    [Header("Inventory Points")]
    private Vector3 inventoryOrigin;
    public Vector3 idleInventoryRotation;
    public Vector3 sprintInventoryRotation;

    [Header("HeadBob")]
    private float headBobTime;
    public float headBobAmplitude = 0.005f;
    private float headBobSpeed;
    private float headBobAimDividerX;
    private float headBobAimDividerY;
    private Vector3 targetBobPosition;

    [Header("Input")]
    private float horizontalMove;
    private float verticalMove;
    public bool isAiming;

    [Header("Movement")]
    public float currentSpeed;
    public float idleSpeed = 0;
    public float walkSpeed = 150;
    public float crouchSpeed = 0;
    public float runSpeed = 200;
    public float acceleration = 10f;
    public float defaultJumpForce = 30;
    public float bounceJumpForce = 150;

    [Header("States")]
    public bool wantsToSprint;
    public bool isGrounded;
    public bool wantsToCrouch;
    public bool wantsToJump;
    public bool isOnPad;

    [Header("Components")]
    public float rbDrag = 10f;
    private Rigidbody rigidBody;
    public Transform mesh;

    [Header("Scale and direction")]
    public Vector3 standScale = new Vector3(1, 2f, 1);
    public Vector3 crouchScale = new Vector3(1, 0.7f, 1);
    private Vector3 moveDirection;
    public Transform defaultCameraPosition;
    public Vector3 crouchedCameraPosition;

    [Header("Jump")]
    public float jumpForce;
    public bool readyToJump = true;
    public float jumpCooldown = 0.6f;

    [Header("Drag")]
    public float defaultDrag = 10f;
    public float airDrag = 2f;
    public float slideDrag = 0.2f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask Ground;
    public LayerMask JumpPad;

    [Header("Hit Ground")]
    [SerializeField] private GameObject hitGroundParticles;
    [SerializeField] private AudioClip hitGroundSound;
    [SerializeField] private AudioClip bounceSound;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        playerInventory = FindObjectOfType<PlayerInventory>();
        inventoryOrigin = playerInventory.transform.localPosition;
        idleInventoryRotation = Vector3.zero;
        sprintInventoryRotation = new Vector3(0, -60f, 0);
        crouchedCameraPosition = new Vector3(defaultCameraPosition.position.x, defaultCameraPosition.position.y - crouchScale.y, defaultCameraPosition.position.z);
    }
    private void Update()
    {
        PlayerInput();
        ControlDrag();
        ControlSpeed();
        ControlHeadBob();
        if (wantsToJump && (isGrounded || isOnPad) && readyToJump) Jump();
    }
    void FixedUpdate()
    {
        MovePlayer();
        Crouch(wantsToCrouch);

    }
    public void PlayerInput()
    {
        //Input direccion
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");
        if (playerInventory.GetCurrentItem() != null && playerInventory.GetCurrentItem().typeOfItem == GameUtils.TypeOfItem.GUN)
        {
            isAiming = playerInventory.GetCurrentItem().gameObject.GetComponent<Weapon>().IsAming;
        }
        else isAiming = false;
        wantsToJump = Input.GetButton("Jump");
        wantsToCrouch = Input.GetKey(KeyCode.LeftControl);
        wantsToSprint = Input.GetKey(KeyCode.LeftShift);

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, Ground);
        isOnPad = Physics.CheckSphere(groundCheck.position, groundDistance, JumpPad);

    }

    void ControlHeadBob()
    {
        //headbob's speed goes along player's speed
        headBobSpeed = Mathf.Max(currentSpeed / 30, 1);
        headBobAimDividerX = isAiming ? 10 : 1;
        headBobAimDividerY = isAiming ? 5 : 1;

        if (isGrounded)
        {
            headBobTime += Time.deltaTime * headBobSpeed;
            playerInventory.transform.localPosition = Vector3.Lerp(playerInventory.transform.localPosition,
                HeadBob(headBobTime, headBobAmplitude / headBobAimDividerX * headBobSpeed,
                headBobAmplitude / headBobAimDividerY * headBobSpeed), Time.deltaTime * 5);
        }

    }
    Vector3 HeadBob(float timePoint, float xIntensity, float yIntensity)
    {
        targetBobPosition = inventoryOrigin + new Vector3(Mathf.Cos(timePoint) * xIntensity, Mathf.Sin(timePoint * 2) * yIntensity, -headBobSpeed / 90);
        return targetBobPosition;
    }
    private void Crouch(bool crouch)
    {
        if (crouch)
        {
            Vector3.Lerp(defaultCameraPosition.position, crouchedCameraPosition, Time.deltaTime);
            mesh.localScale = crouchScale;
        }
        else
        {
            Vector3.Lerp(crouchedCameraPosition, defaultCameraPosition.position, Time.deltaTime);
            mesh.localScale = standScale;
        }

    }

    void MovePlayer()
    {
        //Extra gravity
        rigidBody.AddForce(-transform.up * Time.deltaTime * 20, ForceMode.VelocityChange);

        //orientation
        moveDirection = orientation.forward * verticalMove + orientation.right * horizontalMove;
        moveDirection.Normalize();

        //apply force to the player to move it
        rigidBody.AddForce(moveDirection * currentSpeed * GetMultiplier() * Time.deltaTime, ForceMode.Impulse);

    }
    public void Jump()
    {
        armsAnimator.Play("Jump");
        rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z);
        if (isOnPad)
        {
            jumpForce = bounceJumpForce;
            AudioManager.Instance.PlaySound(bounceSound);
        }
        else jumpForce = defaultJumpForce;
        rigidBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        readyToJump = false;
        Invoke("ResetJump", jumpCooldown);
    }
    public void ResetJump()
    {
        readyToJump = true;
    }

    public float GetMultiplier()
    {
        float multiplier;
        if (isGrounded)
        {
            if (wantsToCrouch)
                multiplier = 0.7f;//crouch
            else
                multiplier = 1f;//default
        }
        else multiplier = 0.2f;//air
        return multiplier;
    }
    void ControlDrag()
    {
        if (isGrounded)
        {
            if (wantsToCrouch)
                rigidBody.drag = slideDrag;
            else
                rigidBody.drag = defaultDrag;
        }
        else rigidBody.drag = airDrag;
    }
    void ControlSpeed()
    {
        int aimDivider = isAiming ? 3 : 1;
        float desiredSpeed;
        if (horizontalMove != 0 || verticalMove != 0)//moving
        {
            if (wantsToCrouch && isGrounded)
            {
                desiredSpeed = crouchSpeed;
            }
            else
             if (wantsToSprint && isGrounded && !wantsToCrouch)
            {
                desiredSpeed = runSpeed;
            }
            else
            {
                desiredSpeed = walkSpeed;
            }
        }
        else desiredSpeed = idleSpeed;

        currentSpeed = Mathf.Lerp(currentSpeed, desiredSpeed / aimDivider, acceleration * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Scenary")) || collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
        {
            Destroy(Instantiate(hitGroundParticles, collision.contacts[0].point, Quaternion.identity, gameObject.transform), 5);
            AudioManager.Instance.PlaySound(hitGroundSound);
        }

    }
}
