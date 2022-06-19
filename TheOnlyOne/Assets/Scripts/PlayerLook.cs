using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("Refereneces")]
    private WallRun wallRun;
    public Transform cam;
    public Transform orientation;

    [Header("Properties")]
    private float xRotation;
    private float yRotation;
    public float sensitivity;
    public float sensMult;

    void Start()
    {
        wallRun = GetComponent<WallRun>();
        hideCursor();
    }

    void Update()
    {
        Look();
    }
    private void Look()
    {
        //user input
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        //horizontal rotation
        yRotation += mouseX * Time.deltaTime * sensitivity * sensMult;
        //vertical rotation
        xRotation -= mouseY * Time.deltaTime * sensitivity * sensMult;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        //apply rotation
        cam.transform.localRotation = Quaternion.Euler(xRotation, yRotation, wallRun.tilt);
        //rotate orientation accordingly
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, wallRun.tilt);

    }
    private void hideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
