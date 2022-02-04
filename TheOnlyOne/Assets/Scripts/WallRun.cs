using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] Transform orientation;

    [Header("Checks")]
    [SerializeField] float wallDist = .5f;
    [SerializeField] float minJumpHeight = 1.5f;

    [Header("WallRun")]
    [SerializeField] private float wallRunGravity;
    [SerializeField] private float wallRunJumpForce;

    bool isWallLeft = false;
    bool isWallRight = false;

    RaycastHit wallLeftRC;
    RaycastHit wallRightRC;

    private Rigidbody rb;
    void CheckWall()
    {
        isWallLeft = Physics.Raycast(transform.position, -orientation.right,out wallLeftRC,wallDist);
        isWallRight = Physics.Raycast(transform.position, -orientation.right, out wallRightRC, wallDist);
    }

    bool CanWallRun()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight);
    }

    void StartWallRun()
    {
        rb.useGravity = false;
        rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 wallRunDirection =Vector3.zero;
            if (isWallLeft)
            {
                 wallRunDirection = transform.up + wallLeftRC.normal;
                
            }else if (isWallRight)
            {
                 wallRunDirection = transform.up + wallRightRC.normal;
            }
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(wallRunDirection * wallRunJumpForce * 100, ForceMode.Force);
        }
    }
    void StopWallRun()
    {
        rb.useGravity = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
