using UnityEngine;

public class MoveCamera : MonoBehaviour {

    public Transform cameraPos;

    void Update() {

        if(transform!=null)
        transform.position = cameraPos.position;
    }
    
}
