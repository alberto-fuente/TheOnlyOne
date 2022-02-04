using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    #region Variables
    public Transform cam;
    public Transform orientation;

    private float xRotation;
    private float yRotation;
    public float sensitivity;
    public float sensMult;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        hideCursor();
    }

    // Update is called once per frame
    void Update()
    {
        Look();
    }
    private void Look()
    {
        //input
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        
        yRotation += mouseX * Time.deltaTime * sensitivity * sensMult;
        //rotacion vertical
        xRotation -= mouseY * Time.deltaTime * sensitivity * sensMult;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        //rotacion horizontal
        cam.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        //rotamos también la orientacion horizontalmente
        orientation.transform.rotation= Quaternion.Euler(0, yRotation, 0);
        
    }
    private void hideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

}
