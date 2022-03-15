using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    [SerializeField] private int bounceForce;
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("boing");
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        Vector3 dir = collision.contacts[0].normal;
        if (rb != null)
        {
            rb.AddForce(dir * bounceForce, ForceMode.Impulse);
        }
    }
}
