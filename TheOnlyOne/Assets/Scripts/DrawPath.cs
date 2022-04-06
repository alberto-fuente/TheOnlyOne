using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPath : MonoBehaviour
{
   // public Transform granadeThrower;
    public Transform shotPoint;
    LineRenderer lineRenderer;
    public GranadeBlueprint granadeData;
    public int numberOfPoints=50;
    public float distanceBetweenPoints=0.1f;
    public LayerMask collidableLayers;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.positionCount = numberOfPoints;
        List<Vector3> points = new List<Vector3>();
        Vector3 startingPos = shotPoint.position;
        Vector3 startingVel = shotPoint.up * granadeData.throwForce;
        for (float i = 0; i < numberOfPoints; i+=distanceBetweenPoints)
        {
            Vector3 newPoint = startingPos + i * startingVel;
            newPoint.y = startingPos.y + startingVel.y * i + Physics.gravity.y / 2f * i * i;
            points.Add(newPoint);
            if (Physics.OverlapSphere(newPoint, 2, collidableLayers).Length > 0)
            {
                lineRenderer.positionCount = points.Count;
                break;
            }
        }
        lineRenderer.SetPositions(points.ToArray());
    }
}
