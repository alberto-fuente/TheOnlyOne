using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canon : MonoBehaviour
{
    public GameObject granade;
    public Transform shotPoint;
    public ProjectPath projectPath;
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            projectPath.SimulateProjection(shotPoint);

        }
        if (Input.GetMouseButtonUp(1))
        {
            projectPath.StopSimulateProjection();
        }
        if (Input.GetMouseButtonDown(0))
        {
            GameObject cratedGranade = Instantiate(granade, shotPoint.position, shotPoint.rotation);
            cratedGranade.GetComponent<Rigidbody>().AddForce(shotPoint.forward * 25, ForceMode.Impulse);
        }
    }
}

