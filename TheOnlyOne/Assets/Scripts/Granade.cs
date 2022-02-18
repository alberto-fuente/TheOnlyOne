using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour
{
    public GranadeBlueprint granadeData;
    Rigidbody granadeRigidbody;
    float countdown;
    bool hasExploded;
    bool hasBeenthrown;

    void Start()
    {
        granadeRigidbody = GetComponent<Rigidbody>();
        countdown = granadeData.delay;
    }


    void Update()
    {
        if (hasBeenthrown)
        {
            countdown -= Time.deltaTime;
        }
        
        if (countdown <= 0f&&!hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }

    private void Explode()
    {
        Instantiate(granadeData.explosionEffect, transform.position, transform.rotation);
        Collider[] colliders= Physics.OverlapSphere(transform.position, granadeData.radius);
        foreach(Collider nearObject in colliders)
        {
            Rigidbody objectRigidBody = nearObject.GetComponent<Rigidbody>();
            if (objectRigidBody != null)
            {
                objectRigidBody.AddExplosionForce(granadeData.explosionForce, transform.position, granadeData.radius);
            }
            //Damage
        }
        Destroy(gameObject);
    }

    public void Throw(Vector3 direction)
    {
        granadeRigidbody.AddForce(direction * granadeData.throwForce, ForceMode.VelocityChange);
        hasBeenthrown = true;
    }
}
