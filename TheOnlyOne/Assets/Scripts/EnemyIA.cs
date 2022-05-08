using UnityEngine;
using UnityEngine.AI;

public class EnemyIA : MonoBehaviour
{
    private EnemyController enemyController;
    private enum State
    {
        Wander,
        ChaseTarget,
        AttackTarget
    }
    public Vector3 wanderPosition;
    public LayerMask whatIsEntity, whatIsObstacle;

    public int scanFrecuency = 30;
    public float scanInterval;
    public float scanTimer;

    public bool targetSet;

    public bool isFrozen;

    bool isAttacking;
    public bool isHurted;
    bool entityInSightRange;

    bool entityInAttackRange;

    //Scan for enemies
    Transform[] children;//se cambia de layer a todos los hijos del enemigo durante el escaneo para que no se detecte a sí mismo
    int enemyLayer;
    int temporaryIgnoreLayer;

    public GameObject target;
    HealthSystem targetHealthSystem;

    public Animator animatorController;
    Collider[] entitiesInRange = new Collider[100];
    public bool hurt;
    public bool destinationSet;
    [SerializeField] State state;
    public NavMeshAgent agent;

    private const float WALKSPEED= 2.7f;
    private const float CHASESPEED= 3.1f;
    //temporal
    private AudioSource audioSource;

    GameObject player;//Referencia al jugador para perseguirle cuendo le dispare
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        state = State.Wander;
    }
    private void Start()
    {
        enemyLayer = LayerMask.NameToLayer("Enemy");
        temporaryIgnoreLayer = LayerMask.NameToLayer("Default");
        enemyController = GetComponent<EnemyController>();
        animatorController = enemyController.enemyPrefab.GetComponentInChildren<Animator>();
        wanderPosition = GetWanderPosition(transform.position);
        audioSource = GetComponent<AudioSource>();
        scanInterval = 1f / scanFrecuency;
        player = GameObject.FindGameObjectWithTag("Player");
        children = gameObject.GetComponentsInChildren<Transform>();
    }
    private void Update()
    {
        //entityInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsEntity);
        //entityInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsEntity);
        //RaycastHit hit;
        //entityInSightRange = Physics.Raycast(transform.position, transform.forward, out hit, sightRange);

        //sightAngle += transform.eulerAngles.y;
        //new Vector3(Mathf.Sin(sightAngle * Mathf.Deg2Rad), 0, Mathf.Cos(sightAngle * Mathf.Deg2Rad));
        if (!isHurted)
        {
            scanTimer -= Time.deltaTime;
            if (scanTimer < 0)
            {
                scanTimer += scanInterval;
                //if(target==null)
                target = FindTarget(enemyController.enemyData.sightRange);
                if (target != null)
                {
                    entityInSightRange = true;
                    entityInAttackRange = Vector3.Distance(transform.position, target.transform.position) < enemyController.enemyData.attackRange;
                }
            }

            if (entityInAttackRange) state = State.AttackTarget;
            else if (entityInSightRange) state = State.ChaseTarget;
            else state = State.Wander;

            targetSet = target != null;
            if (!isFrozen)
            {
                animatorController.SetBool("Frozen", false);
                switch (state)
                {
                    case State.Wander:
                        animatorController.SetInteger("State", 1);//Walk
                        agent.isStopped = false;
                        agent.speed = WALKSPEED;
                        float destination = 1f;
                        if (!destinationSet || Vector3.Distance(transform.position, wanderPosition) < destination)
                        {
                            wanderPosition = GetWanderPosition(transform.position);
                        }
                        destinationSet = agent.SetDestination(wanderPosition);

                        break;
                    case State.ChaseTarget:
                        animatorController.SetInteger("State", 2);//Run
                        agent.isStopped = false;
                        agent.speed = CHASESPEED;
                        if (target != null)
                        {
                            agent.SetDestination(target.transform.position);
                            transform.LookAt(target.transform.position);
                            if (Vector3.Distance(transform.position, target.transform.position) > enemyController.enemyData.stopChasingRange)
                            {
                                entityInSightRange = false;
                                entityInAttackRange = false;
                            }
                        }
                        else
                            entityInSightRange = false;
                        break;
                    case State.AttackTarget:
                        animatorController.SetInteger("State", 3);
                        agent.isStopped = true;
                        if (target != null)
                        {
                            targetHealthSystem = target.GetComponentInParent<HealthSystem>();
                            var lookPos = target.transform.position - transform.position;
                            lookPos.y = 0;
                            var rotation = Quaternion.LookRotation(lookPos);
                            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2);
                            if (!isAttacking && targetHealthSystem != null)
                            {
                                isAttacking = true;
                                if (Random.value <= enemyController.enemyData.hitProbability)
                                {

                                    animatorController.SetTrigger("Shoot");
                                    targetHealthSystem.Damage(Random.Range(enemyController.enemyData.minDamage, enemyController.enemyData.maxDamage));
                                    audioSource.PlayOneShot(enemyController.enemyData.shootSound, 0.2f);


                                }
                                Invoke(nameof(ResetAttack), enemyController.enemyData.attackDelay);
                            }
                        }
                        else
                        {
                            entityInAttackRange = false;
                        }
                        break;
                    default:
                        agent.isStopped = true;
                        break;
                }
            }
            else//congelado
            {
                //animatorController.SetInteger("State", 0);
                animatorController.SetBool("Frozen", true);
                agent.isStopped = true;
            }



        }
        else
        {
            agent.isStopped = true;
            //perseguir al jugador
            //entityInSightRange = true;
            //target = player;
        }
    }

    private GameObject FindTarget(float range)
    {

        int numberOfEntitiesInRange;
        
        foreach (Transform child in children) { 
            child.gameObject.layer = temporaryIgnoreLayer;
        }
        //enemyController.enemyPrefab.layer = temporaryIgnoreLayer;//evitar colisionar consigo mismo
        numberOfEntitiesInRange = Physics.OverlapSphereNonAlloc(transform.position, range, entitiesInRange, whatIsEntity);
        //enemyController.enemyPrefab.layer = enemyLayer;
        foreach (Transform child in children)
        {
            child.gameObject.layer = enemyLayer;
        }
        Collider closestTarget = null;
        if (numberOfEntitiesInRange > 0)
        {
            foreach (Collider coll in entitiesInRange)
            {
                if (coll == null) break;
                if (coll.gameObject.layer == 11)//Player
                {
                    closestTarget = coll;
                    break;
                }
                closestTarget = coll;
            }
            if (Physics.Linecast(transform.position, closestTarget.transform.position, whatIsObstacle))
                return null;
            return closestTarget.gameObject;
        }
        return null;
    }
    private void ResetAttack()
    {
        isAttacking = false;
    }
    private Vector3 GetWanderPosition(Vector3 currentPosition)
    {
        Vector3 randomDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
        Vector3 newDestination = currentPosition + randomDirection * Random.Range(10f, 70f);
        return newDestination;
        /* if (Physics.Raycast(newDestination, -transform.up, 2f, whatIsGround))
         {
             destinationSet = true;
             return newDestination; 
         }*/
    }

}
