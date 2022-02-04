using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //public CharacterController controller;

    public Rigidbody rb;
    public float speed = 10f;

    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask Ground;
    public bool isGrounded = false;

    public float gravity =-9.8f;
    public float jumpForce = 2f;
    Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }
    public void Move()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, Ground);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 direction = transform.right * x + transform.forward * z;

        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded)
        {
            speed = 20f;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = 10f;
        }
        //JUMP
        if (Input.GetButton("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;

        rb.AddForce(direction * speed * Time.deltaTime);
        rb.AddForce(velocity * Time.deltaTime);
    }
}
