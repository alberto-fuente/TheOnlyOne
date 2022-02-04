using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFPS : MonoBehaviour
{
    public Transform playerTransf;
    public float lookSensitivity = 200f;

    float verticalRotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Mouse X") * lookSensitivity * Time.deltaTime;
        float y = Input.GetAxis("Mouse Y") * lookSensitivity * Time.deltaTime;

        verticalRotation -= y;
        verticalRotation = Mathf.Clamp(verticalRotation, -90, 90);
        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        playerTransf.Rotate(Vector3.up * x);
    }
}
