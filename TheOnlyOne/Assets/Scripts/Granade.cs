using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Granade : MonoBehaviour
{
    public GranadeBlueprint granadeData;
    public GameObject prefab;
    public GrabbableItem item;
    public GameManager gameManager;
    public ItemHolder itemHolder;
    public Transform shotPoint;
    Rigidbody granadeRigidbody;
    public Collider granadeIdleCollider;
    private TrailRenderer trail;
    public AudioSource audioSource;
    public Animator animator;
    public float countdown;
    public bool hasExploded;
    public bool hasBeenthrown;
    public bool isAming;
    Transform anchor;
    public CameraShake playerCamera;
    public Camera aimCamera;
    //Freeze
    private void OnEnable()
    {
        //animator.Play("GranadeUp");
    }
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        itemHolder = FindObjectOfType<ItemHolder>();
        playerCamera = FindObjectOfType<CameraShake>();
        aimCamera = GameObject.Find("/CameraHolder/CameraRecoil/MainCamera").GetComponent<Camera>();
        granadeData = GenerateGranade(gameManager.granadeTypes);
        anchor = transform.Find("Anchor");
        //label
        string statText = "";
        if (granadeData.granadeType.Equals("daño explosivo"))
        {
            int totalDamage = granadeData.damage * 10;//se multiplica por 10 porque hay de media 10 colliders en los enemigos
            statText = totalDamage.ToString();
        }
        else if (granadeData.granadeType.Equals("congelación"))
        {
            statText = granadeData.effectDuration.ToString();
        }
        prefab = Instantiate(granadeData.prefab, anchor);
        gameManager.GenerateLabel(prefab.transform, prefab.transform.position+ new Vector3(0.07f, 0.35f, -0.37f), granadeData.name, granadeData.granadeType, granadeData.labelIcon, statText, granadeData.color);
        animator = GetComponentInChildren<Animator>();
        granadeIdleCollider = GetComponentInChildren<BoxCollider>();
        granadeRigidbody = GetComponent<Rigidbody>();
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
        return collection[0];//si hay algún error genera la primera
    }

    void Start()
    {

        item = GetComponent<GrabbableItem>();
        audioSource = GetComponent<AudioSource>();
        countdown = granadeData.delay;
        trail = prefab.GetComponentInChildren<TrailRenderer>();
        trail.enabled = false;
    }


    void Update()
    {
        Sway();
        if (hasBeenthrown)
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
        Physics.Raycast(transform.position,-transform.up, out hit);
        Destroy(Instantiate(granadeData.explosionEffect, hit.point, Quaternion.identity), granadeData.effectDuration);
        audioSource.PlayOneShot(granadeData.explodeSound);
        Collider[] colliders = Physics.OverlapSphere(transform.position, granadeData.radius);
        EnemyIA enemyIA = null;
        playerCamera.ShakeCamera(granadeData.shakeDuration, granadeData.shakeMagnitude);
        foreach (Collider nearObject in colliders)
        {
            if (granadeData.granadeType.Equals("daño explosivo"))
            {
                
                Rigidbody objectRigidBody = nearObject.GetComponent<Rigidbody>();
                if (objectRigidBody)
                {
                    objectRigidBody.AddExplosionForce(granadeData.explosionForce, transform.position, granadeData.radius);

                }
                HealthSystem healthSystem = nearObject.gameObject.GetComponentInParent<HealthSystem>();
                if (healthSystem)
                {
                    healthSystem.Damage(granadeData.damage,true,transform);
                }
            }
            else if (granadeData.granadeType.Equals("congelación"))
            {
                if (nearObject.gameObject.GetComponentInParent<EnemyIA>() != enemyIA)
                {//si no lo he detectado ya (para cuando hay varios enemigos a congelar)
                    enemyIA = nearObject.gameObject.GetComponentInParent<EnemyIA>();//lo marca para congelarlo
                    if (enemyIA)
                    {
                        EnemyIA enemyRef = enemyIA;
                        GameObject icePilarRef = Instantiate(granadeData.icePilar, enemyIA.gameObject.transform.position, Quaternion.Euler(-90f, 0, 0));
                        enemyIA.isFrozen = true;
                        StartCoroutine(freezeCoroutine(enemyRef, enemyRef.transform, icePilarRef));
                    }
                }
            }

        }
        prefab.GetComponentInChildren<MeshRenderer>().enabled = false;
        trail.enabled = false;
        Destroy(gameObject, 15);
    }
    private IEnumerator freezeCoroutine(EnemyIA enemyRef, Transform point, GameObject icePilarRef)
    {
        yield return new WaitForSeconds(granadeData.effectDuration);
        audioSource.PlayOneShot(granadeData.freezelessSound);
        enemyRef.isFrozen = false;
        GameObject fragments = Instantiate(granadeData.icePilarFragmented, point.position, point.rotation);
        if (point)
        {
            Destroy(icePilarRef);
            Destroy(fragments, 3);
        }
    }

    public void Throw(Vector3 position, Vector3 direction)
    { 
        animator.SetTrigger("Throw");
        audioSource.PlayOneShot(granadeData.throwSound);
        audioSource.PlayOneShot(granadeData.counterSound);
        granadeIdleCollider.enabled = false;
        trail.enabled = true;
        item.enabled = false;
        gameObject.transform.position = position;
        granadeRigidbody.AddForce(direction * granadeData.throwForce, ForceMode.Impulse);

    }
    private void ListenAimInput()
    {
        if (Input.GetMouseButton(1))
        {
            animator.SetBool("Aiming", true);
            aimCamera.fieldOfView = granadeData.aimFOV;
        }
        else
        {
            animator.SetBool("Aiming", false);
            aimCamera.fieldOfView = granadeData.mainFOV;
        }
    }
    public void Sway()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Quaternion xSway = Quaternion.AngleAxis(granadeData.swayIntensity * -mouseX, Vector3.up);//horizontal sway
        Quaternion ySway = Quaternion.AngleAxis(granadeData.swayIntensity * mouseY, Vector3.right);//vertical sway
        Quaternion target_rotation = xSway * ySway;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, target_rotation, Time.deltaTime * granadeData.swaySpeed);
    }
}
    /*private bool currentItemIsThroweable()
    {
        currentItem = weaponHolder.GetCurrentItem();
        if (currentItem != null && weaponHolder.GetCurrentItem().typeOfItem.Equals(GameUtils.TypeOfItem.THROWEABLE))
            return true;
        return false;
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