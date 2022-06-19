using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cameraTransform;
    void Start()
    {
        cameraTransform = GameObject.Find("/CameraHolder/CameraRecoil/MainCamera").transform;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + cameraTransform.forward);
    }
}
