using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bola : MonoBehaviour
{
    public GameObject enemy;
    private void Start()
    {
        RaycastHit hit;
        Debug.Log("Go");
        Physics.Raycast(transform.position, -transform.up, out hit, 200);
        if(hit.transform.gameObject.layer==6)
        {
            Debug.Log("Yes");
            Instantiate(enemy, hit.point+new Vector3(0,0.2f,0),Quaternion.identity);
        }
    }


}
