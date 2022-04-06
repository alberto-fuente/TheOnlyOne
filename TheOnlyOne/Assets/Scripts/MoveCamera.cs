using UnityEngine;

public class MoveCamera : MonoBehaviour {

    public Transform cameraPos;

    void Update() {

        if(cameraPos!=null)
        transform.position = cameraPos.position;
    }
    
}
