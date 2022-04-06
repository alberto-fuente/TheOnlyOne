using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour
{
    public GranadeBlueprint granadeData;
    public GameObject mesh;
    public PickableItem item;
    Rigidbody granadeRigidbody;
    public Collider granadeIdleCollider;
    private TrailRenderer trail;
    float countdown;
    bool hasExploded;
    bool hasBeenthrown;

    void Start()
    {
        granadeRigidbody = GetComponent<Rigidbody>();
        item = GetComponent<PickableItem>();
        countdown = granadeData.delay;
        mesh=Instantiate(granadeData.prefab, transform);
        trail = mesh.transform.GetChild(0).GetComponent<TrailRenderer>();
        trail.enabled = false;
    }


    void Update()
    {
        if (hasBeenthrown)
        {
            countdown -= Time.deltaTime;
            mesh.GetComponent<Renderer>().material = granadeData.onMaterial;
        }
        
        if (countdown <= 0f&&!hasExploded)
        {
            Explode();

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
            HealthSystem healthSystem = nearObject.gameObject.GetComponentInParent<HealthSystem>();
            if (healthSystem != null)
            {
                healthSystem.Damage(granadeData.damage);
            }
        }
        hasExploded = true;
        Destroy(gameObject);
    }

    public void Throw(Vector3 position,Vector3 direction)
    {
        granadeIdleCollider.enabled = false;
        trail.enabled = true;
        item.enabled = false;
        gameObject.transform.position = position;
        granadeRigidbody.AddForce(direction * granadeData.throwForce, ForceMode.Impulse);
        hasBeenthrown = true;
    }
}
