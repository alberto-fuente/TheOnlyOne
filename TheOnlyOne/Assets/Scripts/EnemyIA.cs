using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyIA : MonoBehaviour
{
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

    public float attackDelay;
    bool isAttacking;

    bool entityInSightRange;
    private float sightRange=10;

    [Range(0,360)]
    private float sightAngle=60;

    bool entityInAttackRange;
    private float attackRange=3;

    private float stopChasingRange = 40;

    public GameObject target;
    HealthSystem targetHealthSystem;

    Collider[] entitiesInRange = new Collider[100];
    private int minHitDamage = 3;
    private int maxHitDamage = 10;
    private float hitProbability = 0.5f;
    public bool destinationSet;
    [SerializeField] State state;
    private NavMeshAgent agent;
    //temporal
    private AudioSource audioSource;
    public AudioClip laser;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        state = State.Wander;
    }
    private void Start()
    {
        wanderPosition = GetWanderPosition(transform.position);
        audioSource = GetComponent<AudioSource>();
        scanInterval = 1f / scanFrecuency;

    }
    private void Update()
    {
        //entityInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsEntity);
        //entityInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsEntity);
        //RaycastHit hit;
        //entityInSightRange = Physics.Raycast(transform.position, transform.forward, out hit, sightRange);

        //sightAngle += transform.eulerAngles.y;
        //new Vector3(Mathf.Sin(sightAngle * Mathf.Deg2Rad), 0, Mathf.Cos(sightAngle * Mathf.Deg2Rad));

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
       
        switch (state)
        {
            case State.Wander:
                agent.isStopped = false;
                float destination = 1f;
                if (!destinationSet|| Vector3.Distance(transform.position, wanderPosition) < destination)
                {
                    wanderPosition = GetWanderPosition(transform.position);
                }
                destinationSet = agent.SetDestination(wanderPosition);
                
                break;
            case State.ChaseTarget:
                
                agent.isStopped = false;
                if (target != null)
                {
                    agent.SetDestination(target.transform.position);
                    transform.LookAt(target.transform.position);
                    if (Vector3.Distance(transform.position, target.transform.position) > stopChasingRange)
                    {
                        entityInSightRange = false;
                        entityInAttackRange = false;
                    }
                }
                else
                    entityInSightRange = false;
                break;
            case State.AttackTarget:
                agent.isStopped=true;
                if (target != null)
                {
                    targetHealthSystem = target.GetComponentInParent<HealthSystem>();
                    transform.LookAt(target.transform.position);
                    if (!isAttacking && targetHealthSystem != null)
                    {
                        isAttacking = true;
                        if (Random.value <= hitProbability)
                        {
                            targetHealthSystem.Damage(Random.Range(minHitDamage,maxHitDamage));
                            audioSource.PlayOneShot(laser, 0.1f);

                        }
                        Invoke(nameof(ResetAttack), attackDelay);
                    }
                }else
                    entityInAttackRange = false;
                break;
        }
       
    }
    private GameObject FindTarget(float range)
    {
        int enemyLayer = 14;
        int temporaryIgnoreLayer = 0;
        
        int numberOfEntitiesInRange;

        transform.GetChild(0).gameObject.layer = temporaryIgnoreLayer;//evitar colisionar consigo mismo
        numberOfEntitiesInRange = Physics.OverlapSphereNonAlloc(transform.position, range, entitiesInRange, whatIsEntity);
        transform.GetChild(0).gameObject.layer = enemyLayer;
        
        Collider closestTarget = null;
        if (numberOfEntitiesInRange > 0)
        {
            foreach (Collider coll in entitiesInRange)
            {
                if (coll == null) break;
                if (coll.gameObject.layer == 11)//Player
                {
                    closestTarget= coll;
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
        Vector3 randomDirection= new Vector3(UnityEngine.Random.Range(-1f, 1f),0, UnityEngine.Random.Range(-1f, 1f)).normalized;
        Vector3 newDestination= currentPosition + randomDirection * Random.Range(10f, 70f);
        return newDestination;
        /* if (Physics.Raycast(newDestination, -transform.up, 2f, whatIsGround))
         {
             destinationSet = true;
             return newDestination; 
         }*/


    }
  
}
