// Some stupid rigidbody based movement by Dani

using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement1 : MonoBehaviour {

    //Assingables
    public Transform head;
    //public Transform orientation;
    private Vector2 rotation;
    Vector3 moveDirection;
    public float rbDrag = 6f;
    public float moveMultiplier = 10f;
    //Other
    private Rigidbody rb;
    private Vector3 direction;
    Vector2 mouseLook;
    Vector2 smoothV;
    public float dpi = 2.0f;
    public float smoothing = 3.0f;
    //Rotation and look
    private float xRotation;
    private float yRotation;
    public float sensitivity = 50f;
    public float sensMult = 2f;
    //Movement
    public float moveSpeed;
    public float walkSpeed = 6;
    public float runSpeed = 10;


    public bool grounded;
    public LayerMask whatIsGround;
    
    public float maxSlopeAngle = 35f;

    //Crouch & Slide
    //private float desiredDuration = 10f;
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;

    public float slideForce = 400;
    public float slideCounterMovement = 0.2f;

    //Jumping
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;

    //Input
    float h,v;
    public bool jumping, sprinting, crouching;
    
    //Sliding
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    
    void Start() {
        hideCursor();
        playerScale = transform.localScale;
    }
    
    private void FixedUpdate() {
        Move();
    }
    private void Update()
    {
        Look();
        MyInput();
        ControlDrag();

    }
    private void hideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Move()
    {
        rb.AddForce(Vector3.down * Time.deltaTime * 10);
        //Some multipliers
        float multiplier = 1f;

        // Movement in air
        if (!grounded)
        {
            multiplier = 0.5f;
        }
        // Movement while sliding
        if (grounded && crouching) multiplier = 0f;

        moveSpeed = (sprinting && grounded) ? runSpeed : walkSpeed;

        moveDirection = transform.forward * v + transform.right * h;

        rb.AddForce(moveDirection.normalized * moveSpeed * moveMultiplier *multiplier, ForceMode.Acceleration);

        if (readyToJump && jumping) Jump();

        if (crouching && grounded)
        {
            rb.AddForce(Vector3.down * Time.deltaTime * 300);
            return;
        }
    }
    private void Look()
    {
        //input
        float mouseX = Input.GetAxis("Mouse X") * sensMult;
        float mouseY = Input.GetAxis("Mouse Y")  * sensMult;

        //rotacion horizontal
        //yRotation += mouseX;
        //transform.Rotate(Vector3.up * mouseX);

        //rotacion vertical
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        head.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
      // transform.rotation = Quaternion.Euler(0, yRotation, 0);
        transform.Rotate(Vector3.up * mouseX);
    }
    void ControlDrag()
    {
        rb.drag = rbDrag;
    }

    /// <summary>
    /// Find user input. Should put this in its own class but im lazy
    /// </summary>
    private void MyInput() {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.LeftControl);
        sprinting = Input.GetKey(KeyCode.LeftShift);


        //Crouching
        if (Input.GetKeyDown(KeyCode.LeftControl))
            ModifyScale(crouchScale);

        if (Input.GetKeyUp(KeyCode.LeftControl))
            ModifyScale(playerScale);
    }
  
    private void ModifyScale(Vector3 desiredScale) {
        transform.localScale = desiredScale;
        if (rb.velocity.magnitude > 0.5f && grounded) {
            rb.AddForce(transform.forward * slideForce);
        }
    }


    private void Jump() {
        if (grounded && readyToJump) {
            readyToJump = false;

            //Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);
            /*
            //If jumping while falling, reset y velocity.
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0) 
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);
            
            */
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    
    private void ResetJump() {
        readyToJump = true;
    }

    private bool IsFloor(Vector3 v) {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private bool cancellingGrounded;
    
    /// <summary>
    /// Handle ground detection
    /// </summary>
    private void OnCollisionStay(Collision other) {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++) {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal)) {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded) {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void StopGrounded() {
        grounded = false;
    }
    
}
