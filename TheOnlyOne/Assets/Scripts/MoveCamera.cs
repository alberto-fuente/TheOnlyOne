using UnityEngine;

public class MoveCamera : MonoBehaviour {

    public Transform origin;
    public Transform goal;

    void LateUpdate() {
        if (!origin)
        {
            origin = gameObject.transform;
        }
            
        origin.position = goal.position;
    }
    
}
