using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerMove playerMove;

    [Header("Movement")]
    [SerializeField] Transform orientation;

    [Header("Checks")]
    [SerializeField] float wallDist = .5f;
    [SerializeField] float minJumpHeight = 1.5f;

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
    void CheckWall()
    {
        isWallLeft = Physics.Raycast(transform.position, -orientation.right,out wallLeftRC,wallDist);
        isWallRight = Physics.Raycast(transform.position, orientation.right, out wallRightRC, wallDist);
    }

    bool CanWallRun()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight);
    }

    void StartWallRun()
    {
        rb.useGravity = false;

        if(playerMove.moveSpeed<playerMove.runSpeed)
        playerMove.moveSpeed =playerMove.runSpeed;
        rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, wallRunFov, wallRunFovTime * Time.deltaTime);

        if (isWallLeft)
            tilt = Mathf.Lerp(tilt, -camTilt, camTiltTime * Time.deltaTime);
        else if(isWallRight)
            tilt = Mathf.Lerp(tilt, camTilt, camTiltTime * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerMove.armsAnimator.Play("Jump");
            playerMove.wholeAnimator.Play("Jump");
            Vector3 wallRunDirection =Vector3.zero;
            if (isWallLeft)
            {
                 wallRunDirection = transform.up + wallLeftRC.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunDirection * wallRunJumpForce * 100, ForceMode.Force);

            }
            else if (isWallRight)
            {
                 wallRunDirection = transform.up + wallRightRC.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunDirection * wallRunJumpForce * 100, ForceMode.Force);
            }
            
        }
    }
    void StopWallRun()
    {
        rb.useGravity = true;
        //playerMove.moveSpeed = playerMove.walkSpeed;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, wallRunFovTime * Time.deltaTime);
        tilt = Mathf.Lerp(tilt, 0, camTiltTime * Time.deltaTime);

    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerMove = GetComponent<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckWall();
        if (CanWallRun())
        {
            if (isWallLeft)
            {
                StartWallRun();
            }
            else if (isWallRight)
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
}
