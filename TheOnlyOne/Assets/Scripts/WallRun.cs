using UnityEngine;

public class WallRun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerMove playerMove;

    [Header("Movement")]
    [SerializeField] Transform orientation;

    [Header("Checks")]
    [SerializeField] LayerMask whatIsWallrunnable;
    private float wallDist = 0.6f;
    private float minJumpHeight = 1.5f;

    [Header("WallRun")]
    [SerializeField] private float wallRunGravity;
    [SerializeField] private float wallRunJumpForce;

    [Header("Camera Effects")]
    [SerializeField] private Camera cam;
    [SerializeField] private float fov;
    [SerializeField] private float wallRunFov;
    [SerializeField] private float wallRunFovTime;
    [SerializeField] private float camTilt;
    [SerializeField] private float camTiltTime;
    public float tilt { get; private set; }

    bool isWallLeft = false;
    bool isWallRight = false;

    RaycastHit wallLeftRC;
    RaycastHit wallRightRC;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerMove = GetComponent<PlayerMove>();
    }
    void Update()
    {
        CheckWall();
        if (CanWallRun())
        {
            if (isWallLeft || isWallRight)
            {
                StartWallRun();
            }
            else
            {
                StopWallRun();
            }
        }
        else
        {
            StopWallRun();
        }
    }
    void CheckWall()
    {
        isWallLeft = Physics.Raycast(transform.position, -orientation.right, out wallLeftRC, wallDist, whatIsWallrunnable);
        isWallRight = Physics.Raycast(transform.position, orientation.right, out wallRightRC, wallDist, whatIsWallrunnable);
    }

    bool CanWallRun()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight);
    }

    void StartWallRun()
    {
        rb.useGravity = false;
        if (playerMove.currentSpeed < playerMove.runSpeed) playerMove.currentSpeed = playerMove.runSpeed;
        rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, wallRunFov, wallRunFovTime * Time.deltaTime);

        if (isWallLeft) tilt = Mathf.Lerp(tilt, -camTilt, camTiltTime * Time.deltaTime);
        else if (isWallRight) tilt = Mathf.Lerp(tilt, camTilt, camTiltTime * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerMove.armsAnimator.Play("Jump");
            Vector3 wallRunDirection = Vector3.zero;
            if (isWallLeft)
            {
                wallRunDirection = transform.up + wallLeftRC.normal;
            }
            else if (isWallRight)
            {
                wallRunDirection = transform.up + wallRightRC.normal;
            }
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(wallRunDirection * wallRunJumpForce * 118, ForceMode.Force);
        }
    }
    void StopWallRun()
    {
        rb.useGravity = true;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, wallRunFovTime * Time.deltaTime);
        tilt = Mathf.Lerp(tilt, 0, camTiltTime * Time.deltaTime);

    }

}
