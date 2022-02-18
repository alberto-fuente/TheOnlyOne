using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Label : MonoBehaviour
{
    private Transform cameraTransform;
    public TMP_Text itemName;

    void Start()
    {
        cameraTransform = GameObject.Find("/CameraHolder/CameraRecoil/MainCamera").transform;

    }
    void LateUpdate()
    {
        transform.LookAt(transform.position + cameraTransform.forward);
    }
}
