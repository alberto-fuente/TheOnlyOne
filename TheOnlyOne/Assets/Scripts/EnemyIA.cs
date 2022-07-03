using UnityEngine;
using UnityEngine.AI;

public class EnemyIA : MonoBehaviour
{
    [Header("References")]
    private EnemyController enemyController;
    public Animator animatorController;
    public NavMeshAgent agent;
    private AudioSource audioSource;

    [Header("Properties")]
    private const float WALKSPEED = 2.7f;
    private const float CHASESPEED = 3.1f;
    public float sightRange;
    public float attackRange;
    public int scanFrecuency = 30;
    public float scanInterval;
    public float scanTimer;
    public bool targetSet;
    public bool destinationSet;
    private Vector3 wanderPosition;
    Transform[] children;
    int enemyLayer;
    int temporaryIgnoreLayer;

    [Header("States")]
    private State state;
    bool isAttacking;
    public bool isHurted;
    public bool isFrozen;
    private enum State
    {
        Wander,
        ChaseTarget,
        AttackTarget
    }

    [Header("Target")]
    public LayerMask whatIsEntity, whatIsObstacle;
    bool entityInSightRange;
    bool entityInAttackRange;
    private GameObject target;
    private HealthSystem targetHealthSystem;
    Collider[] entitiesInRange = new Collider[50];

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
        animatorController = enemyController.AnimatorController;
        sightRange = enemyController.EnemyData.sightRange;
        attackRange = enemyController.EnemyData.attackRange;
        audioSource = GetComponent<AudioSource>();
        scanInterval = 1f / scanFrecuency;
        children = GetComponentsInChildren<Transform>();
    }
    private void Update()
    {
        if (!isHurted)
        {
            scanTimer -= Time.deltaTime;
            if (scanTimer < 0)
            {
                scanTimer += scanInterval;
                target = FindTarget(sightRange);
                if (target != null)
                {
                    entityInSightRange = true;
                    entityInAttackRange = Vector3.Distance(transform.position, target.transform.position) < attackRange;
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
                        animatorController.SetInteger("State", 1);//Walk around
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
                        animatorController.SetInteger("State", 2);//Chase Target
                        agent.isStopped = false;
                        agent.speed = CHASESPEED;
                        if (target != null)
                        {
                            agent.SetDestination(target.transform.position);
                            transform.LookAt(target.transform.position);
                            if (Vector3.Distance(transform.position, target.transform.position) > enemyController.EnemyData.stopChasingRange)
                            {
                                entityInSightRange = false;
                                entityInAttackRange = false;
                            }
                        }
                        else
                            entityInSightRange = false;
                        break;
                    case State.AttackTarget:
                        animatorController.SetInteger("State", 3);//Shoot Target
                        agent.isStopped = true;
                        if (target != null)
                        {
                            targetHealthSystem = target.GetComponentInParent<HealthSystem>();
                            var lookPosition = target.transform.position - transform.position;
                            lookPosition.y = 0;
                            var rotation = Quaternion.LookRotation(lookPosition);
                            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2);
                            if (!isAttacking && targetHealthSystem != null)
                            {
                                isAttacking = true;
                                if (Random.value <= enemyController.EnemyData.hitProbability)
                                {
                                    animatorController.SetTrigger("Shoot");
                                    targetHealthSystem.Damage(Random.Range(enemyController.EnemyData.minDamage, enemyController.EnemyData.maxDamage), false, transform);
                                    audioSource.PlayOneShot(enemyController.EnemyData.shootSound, 0.2f);
                                }
                                Invoke(nameof(ResetAttack), enemyController.EnemyData.attackDelay);
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
            else//frozen
            {
                animatorController.SetBool("Frozen", true);
                agent.isStopped = true;
            }



        }
        else//hurted
        {
            agent.isStopped = true;
        }
    }

    private GameObject FindTarget(float range)
    {
        int numberOfEntitiesInRange;
        //enemy moves to another layer to prevent detect itself during scan
        foreach (Transform child in children)
        {
            child.gameObject.layer = temporaryIgnoreLayer;
        }
        //scans
        numberOfEntitiesInRange = Physics.OverlapSphereNonAlloc(transform.position, range, entitiesInRange, whatIsEntity);
        //enemy moves back to its normal layer
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
                if (coll.gameObject.layer == LayerMask.NameToLayer("Player"))//Prioritize attacking player rather than other bots
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
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        Vector3 newDestination = currentPosition + randomDirection * Random.Range(10f, 50f);
        //Avoid the toxic zone
        if ((Mathf.Pow(newDestination.x, 2) + Mathf.Pow(newDestination.z, 2) > Mathf.Pow(GameManager.Instance.SafeRadius, 2)))
        {
            return currentPosition;
        }
        return newDestination;
    }

}
