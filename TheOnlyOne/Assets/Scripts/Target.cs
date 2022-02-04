using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float speed = 5f;
    public Transform[] points;

    private int currentPoint=0;

    public float stopDistance = 0.05f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, points[currentPoint].position, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, points[currentPoint].position) < stopDistance)
        {
            if (currentPoint == points.Length-1)
            {
                currentPoint = 0;
            }else
            currentPoint++;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] != null)
            {
                if (i == 0)
                {
                    Gizmos.DrawLine(points[i].position, points[points.Length - 1].position);
                }
                else
                {
                    Gizmos.DrawLine(points[i].position, points[i - 1].position);
                }
            }
        }
    }
}
