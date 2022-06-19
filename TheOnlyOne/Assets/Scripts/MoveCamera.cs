using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [Header("Move points")]
    public Transform origin;
    public Transform goal;

    void LateUpdate()
    {
        if (!origin)
        {
            origin = gameObject.transform;
        }
        origin.position = goal.position;
    }
}