using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Granade : MonoBehaviour
{
    [Header("References")]
    private GranadeBlueprint granadeData;
    private GameObject granadePrefab;
    private GrabbableItem item;
    private GameManager gameManager;
    private Transform anchor;
    private Rigidbody granadeRigidbody;
    private Collider granadeIdleCollider;
    private TrailRenderer trail;
    private AudioSource audioSource;
    private Animator animator;
    private CameraShake playerCamera;
    private Camera aimCamera;

    [Header("Properties")]
    private float countdown;
    private bool hasExploded;
    private bool hasBeenthrown;
    private string statText = "";

    public bool HasBeenthrown { get => hasBeenthrown; set => hasBeenthrown = value; }
    public GranadeBlueprint GranadeData { get => granadeData; set => granadeData = value; }
    public Collider GranadeIdleCollider { get => granadeIdleCollider; set => granadeIdleCollider = value; }
    private void Awake()
    {
        gameManager = GameManager.Instance;
        playerCamera = FindObjectOfType<CameraShake>();
        item = GetComponent<GrabbableItem>();
        audioSource = GetComponent<AudioSource>();
        granadeRigidbody = GetComponent<Rigidbody>();
        aimCamera = GameObject.Find("/CameraHolder/CameraRecoil/MainCamera").GetComponent<Camera>();
        anchor = transform.Find("Anchor");

        GranadeData = GenerateGranade(gameManager.granadeTypes);
        granadePrefab = Instantiate(GranadeData.prefab, anchor);
        SetLabelText();
        gameManager.GenerateLabel(granadePrefab.transform, granadePrefab.transform.position + new Vector3(0.07f, 0.35f, -0.37f), GranadeData.granadeName, GranadeData.granadeType, GranadeData.labelIcon, statText, GranadeData.color);

        animator = GetComponentInChildren<Animator>();
        GranadeIdleCollider = GetComponentInChildren<BoxCollider>();
        trail = granadePrefab.GetComponentInChildren<TrailRenderer>();
        trail.enabled = false;
        countdown = GranadeData.delay;
    }

    private void SetLabelText()
    {
        if (GranadeData.id == 0)//explosive
        {
            int totalDamage = GranadeData.damage * 11;//x11 because bots have 11 colliders
            statText = totalDamage.ToString();
        }
        else if (GranadeData.id == 1)//freeze
        {
            statText = GranadeData.effectDuration.ToString();
        }
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
        return collection[0];
    }

    void Update()
    {
        Sway();
        if (HasBeenthrown)
        {
            countdown -= Time.deltaTime;
        }
        else
        {
            ListenAimInput();
        }

        if (countdown <= 0f && !hasExploded)
        {
            Explode();
        }

    }

    private void Explode()
    {
        hasExploded = true;
        RaycastHit hit;
        Physics.Raycast(transform.position, -transform.up, out hit);
        Destroy(Instantiate(GranadeData.explosionEffect, hit.point, Quaternion.identity), GranadeData.effectDuration);
        audioSource.PlayOneShot(GranadeData.explodeSound);
        Collider[] colliders = Physics.OverlapSphere(transform.position, GranadeData.radius);
        EnemyIA enemyIA = null;
        playerCamera.ShakeCamera(GranadeData.shakeDuration, GranadeData.shakeMagnitude);
        foreach (Collider nearObject in colliders)
        {
            if (GranadeData.id == 0)//explosive
            {

                Rigidbody objectRigidBody = nearObject.GetComponent<Rigidbody>();
                if (objectRigidBody)
                {
                    objectRigidBody.AddExplosionForce(GranadeData.explosionForce, transform.position, GranadeData.radius);

                }
                HealthSystem healthSystem = nearObject.gameObject.GetComponentInParent<HealthSystem>();
                if (healthSystem)
                {
                    healthSystem.Damage(GranadeData.damage, true, transform);
                }
            }
            else if (GranadeData.id == 1)//explosive
            {
                if (nearObject.gameObject.GetComponentInParent<EnemyIA>() != enemyIA)
                {//si no lo he detectado ya (para cuando hay varios enemigos a congelar)
                    enemyIA = nearObject.gameObject.GetComponentInParent<EnemyIA>();//lo marca para congelarlo
                    if (enemyIA)
                    {
                        EnemyIA enemyRef = enemyIA;
                        GameObject icePilarRef = Instantiate(GranadeData.icePilar, enemyIA.gameObject.transform.position, Quaternion.Euler(-90f, 0, 0));
                        enemyIA.isFrozen = true;
                        StartCoroutine(FreezeCoroutine(enemyRef, enemyRef.transform, icePilarRef));
                    }
                }
            }

        }
        granadePrefab.GetComponentInChildren<MeshRenderer>().enabled = false;
        trail.enabled = false;
        Destroy(gameObject, 15);
    }
    private IEnumerator FreezeCoroutine(EnemyIA _enemyRef, Transform _point, GameObject _icePilarRef)
    {
        Vector3 pointPosition = _point.position;
        yield return new WaitForSeconds(GranadeData.effectDuration);
        audioSource.PlayOneShot(GranadeData.freezelessSound);
        if(_enemyRef!=null)_enemyRef.isFrozen = false;
        GameObject fragments = Instantiate(GranadeData.icePilarFragmented, pointPosition, _point.rotation);
        if (_point != null)
        {
            Destroy(_icePilarRef);
            Destroy(fragments, 3);
        }
    }

    public void Throw(Vector3 _position, Vector3 _direction)
    {
        animator.SetTrigger("Throw");
        audioSource.PlayOneShot(GranadeData.throwSound);
        audioSource.PlayOneShot(GranadeData.counterSound, .1f);
        GranadeIdleCollider.enabled = false;
        trail.enabled = true;
        item.enabled = false;
        gameObject.transform.position = _position;
        granadeRigidbody.AddForce(_direction * GranadeData.throwForce, ForceMode.Impulse);

    }
    private void ListenAimInput()
    {
        if (Input.GetMouseButton(1))
        {
            animator.SetBool("Aiming", true);
            aimCamera.fieldOfView = GranadeData.aimFOV;
        }
        else
        {
            animator.SetBool("Aiming", false);
            aimCamera.fieldOfView = GranadeData.mainFOV;
        }
    }
    public void Sway()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Quaternion xSway = Quaternion.AngleAxis(GranadeData.swayIntensity * -mouseX, Vector3.up);//horizontal sway
        Quaternion ySway = Quaternion.AngleAxis(GranadeData.swayIntensity * mouseY, Vector3.right);//vertical sway
        Quaternion target_rotation = xSway * ySway;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, target_rotation, Time.deltaTime * GranadeData.swaySpeed);
    }
}