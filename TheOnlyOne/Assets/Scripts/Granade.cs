using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Granade : MonoBehaviour
{
    public GranadeBlueprint granadeData;
    public GameObject mesh;
    public PickableItem item;
    public GameManager gameManager;
    Rigidbody granadeRigidbody;
    public Collider granadeIdleCollider;
    private TrailRenderer trail;
    public AudioSource audioSource;
    float countdown;
    bool hasExploded;
    bool hasBeenthrown;
    //Freeze

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        granadeData=GenerateGranade(gameManager.granadeTypes);
    }

    private GranadeBlueprint GenerateGranade(GranadeBlueprint[] collection)
    {
        int i = Random.Range(0, 100);
        for (int j = 0; j < collection.Length; j++)
        {
            if (i >= collection[j].Minprobabilty && i <= collection[j].Maxprobabilty)
            {
                return collection[j];
            }
        }
        return collection[0];//si hay algún error genera un arma común
    }

    void Start()
    {
        granadeRigidbody = GetComponent<Rigidbody>();
        item = GetComponent<PickableItem>();
        audioSource = GetComponent<AudioSource>();
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
            mesh.GetComponentInChildren<Renderer>().material = granadeData.onMaterial;
        }
        
        if (countdown <= 0f&&!hasExploded)
        {
            Explode();

        }
            
    }

    private void Explode()
    {
        hasExploded = true;
        Destroy(Instantiate(granadeData.explosionEffect, transform.localPosition, Quaternion.identity),granadeData.effectDuration);
        audioSource.PlayOneShot(granadeData.explodeSound);
        Collider[] colliders= Physics.OverlapSphere(transform.position, granadeData.radius);
        EnemyIA enemyIA = null;
        foreach (Collider nearObject in colliders)
        {
            if (granadeData.granadeName.Equals("Explode"))
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
            else if (granadeData.granadeName.Equals("Freeze"))
            {
                if (nearObject.gameObject.GetComponentInParent<EnemyIA>()!= enemyIA) {//si no lo he detectado ya (para cuando hay varios enemigos a congelar)
                    enemyIA = nearObject.gameObject.GetComponentInParent<EnemyIA>();//lo marca para congelarlo
                    if (enemyIA)
                    {
                        EnemyIA enemyRef = enemyIA;
                        GameObject icePilarRef = Instantiate(granadeData.icePilar, enemyIA.gameObject.transform.position, Quaternion.Euler(-90f, 0, 0));
                        enemyIA.isFrozen = true;
                        StartCoroutine(freezeCoroutine(enemyRef, icePilarRef));
                    }
                }
            }
           
        }
        mesh.GetComponent<MeshRenderer>().enabled = false;
        trail.enabled = false;
        Destroy(gameObject,15);
    }
    private IEnumerator freezeCoroutine(EnemyIA enemyRef,GameObject icePilarRef)
    {
        yield return new WaitForSeconds(granadeData.effectDuration);
        audioSource.PlayOneShot(granadeData.freezelessSound);
        enemyRef.isFrozen = false;
        GameObject fragments = Instantiate(granadeData.icePilarFragmented, icePilarRef.transform.position, enemyRef.gameObject.transform.rotation);
        Destroy(icePilarRef);
        Destroy(fragments, 3);
    }
    public void Throw(Vector3 position,Vector3 direction)
    {
        audioSource.PlayOneShot(granadeData.throwSound);
        audioSource.PlayOneShot(granadeData.counterSound);
        granadeIdleCollider.enabled = false;
        trail.enabled = true;
        item.enabled = false;
        gameObject.transform.position = position;
        granadeRigidbody.AddForce(direction * granadeData.throwForce, ForceMode.Impulse);
        hasBeenthrown = true;
    }
}
/*public class Granade : MonoBehaviour
{
    public GranadeBlueprint granadeData;
    public GameObject mesh;
    public PickableItem item;
    public GameManager gameManager;
    Rigidbody granadeRigidbody;
    public Collider granadeIdleCollider;
    private TrailRenderer trail;
    public AudioSource audioSource;
    float countdown;
    bool hasExploded;
    bool hasBeenthrown;
    //Freeze
    float freezeCountdown;
    EnemyIA enemyRef;
    GameObject icePilarRef;
    bool hasBeenFrozen = false;
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        granadeData=GenerateGranade(gameManager.granadeTypes);
    }

    private GranadeBlueprint GenerateGranade(GranadeBlueprint[] collection)
    {
        int i = Random.Range(0, 100);
        for (int j = 0; j < collection.Length; j++)
        {
            if (i >= collection[j].Minprobabilty && i <= collection[j].Maxprobabilty)
            {
                return collection[j];
            }
        }
        return collection[0];//si hay algún error genera un arma común
    }

    void Start()
    {
        freezeCountdown = granadeData.effectDuration;
        granadeRigidbody = GetComponent<Rigidbody>();
        item = GetComponent<PickableItem>();
        audioSource = GetComponent<AudioSource>();
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
            mesh.GetComponentInChildren<Renderer>().material = granadeData.onMaterial;
        }
        
        if (countdown <= 0f&&!hasExploded)
        {
            Explode();

        }
        //Freeze
        if (enemyRef != null)
        {
            if (enemyRef.isFrozen)
            {
                freezeCountdown -= Time.deltaTime;
            }
            if (freezeCountdown <= 0&&!hasBeenFrozen)
            {
                //colliderRef.gameObject.GetComponentInChildren<Renderer>().material = originalMatRef;
                audioSource.PlayOneShot(granadeData.freezelessSound);
                enemyRef.isFrozen = false;
                GameObject fragments = Instantiate(granadeData.icePilarFragmented, icePilarRef.transform.position, enemyRef.gameObject.transform.rotation);
                Destroy(fragments, 3);
                enemyRef = null;
                hasBeenFrozen = true;

            }
        }
            
    }

    private void Explode()
    {
        Destroy(Instantiate(granadeData.explosionEffect, transform.localPosition, Quaternion.identity),granadeData.effectDuration);
        audioSource.PlayOneShot(granadeData.explodeSound);
        Collider[] colliders= Physics.OverlapSphere(transform.position, granadeData.radius);
        foreach (Collider nearObject in colliders)
        {
            if (granadeData.granadeName.Equals("Explode"))
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
            else if (granadeData.granadeName.Equals("Freeze"))
            { 
                EnemyIA enemyIA = nearObject.gameObject.GetComponentInParent<EnemyIA>();
                if (enemyIA != null)
                {
                    icePilarRef = Instantiate(granadeData.icePilar, enemyIA.gameObject.transform.position, Quaternion.Euler(0,0,0));
                    Destroy(icePilarRef, granadeData.effectDuration);
                    /*originalMatRef = nearObject.gameObject.GetComponentInChildren<Renderer>().material;
                    nearObject.gameObject.GetComponentInChildren<Renderer>().material = granadeData.freezeMaterial;
                    colliderRef = nearObject;
enemyRef = enemyIA;
enemyIA.isFrozen = true;
                }
            }
           
        }
        hasExploded = true;
mesh.GetComponent<MeshRenderer>().enabled = false;
trail.enabled = false;
Destroy(gameObject, 15);
    }

    public void Throw(Vector3 position, Vector3 direction)
{
    audioSource.PlayOneShot(granadeData.throwSound);
    audioSource.PlayOneShot(granadeData.counterSound);
    granadeIdleCollider.enabled = false;
    trail.enabled = true;
    item.enabled = false;
    gameObject.transform.position = position;
    granadeRigidbody.AddForce(direction * granadeData.throwForce, ForceMode.Impulse);
    hasBeenthrown = true;
}
}*/