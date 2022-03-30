using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyIA : MonoBehaviour
{/*
    public int maxHealth;
    public int currentHealth;
    public bool isDead;
    public NavMeshAgent agent;
    GameObject target;
    float distanceToTarget;
    public float stopDistance = 3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

    }
    private void Move()
    {
        distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        if (distanceToTarget < stopDistance || target.GetComponent<PlayerController>().hasInvisibilityPowerUp)
        {
            StopGoTowardsTarget();

        }
        else if (!isDead)
        {

            GoTowardsTarget();

        }

    }
    IEnumerator TimeToHeadShot()
    {
        HeadCanExplode = true;
        yield return new WaitForSeconds(0.1f);
        HeadCanExplode = false;
    }
    private void GoTowardsTarget()
    {
        if (agent.isActiveAndEnabled && !isDead && !target.GetComponent<PlayerController>().isDead)
        {
            agent.isStopped = false;
            Vector3 _dir = target.transform.position - transform.position;
            _dir.Normalize();
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_dir), turnSpeed * Time.deltaTime);
            /* directionToMove = (target.transform.position - transform.position).normalized;
             Vector3 directionToMoveWithSpeed = directionToMove.normalized * Time.deltaTime * speed;
             agent.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToMove), turnSpeed * Time.deltaTime);//look towards player
             agent.transform.position = transform.position + Vector3.ClampMagnitude(directionToMoveWithSpeed, dist);//move towards player
            agent.SetDestination(target.transform.position);
            animator.SetBool("walking", true);
            animator.SetBool("Attacking", false);
            if (!agent.hasPath) animator.SetBool("walking", false);
        }
    }
    private void StopGoTowardsTarget()
    {
        if (agent.isActiveAndEnabled)
        {
            agent.isStopped = true;
            animator.SetBool("walking", false);
            Attack();
        }
    }
    private void Attack()
    {
        if (!isDead && Time.time - lastAttack > AttackCoolDown && dist < stopDistance && !target.GetComponent<PlayerController>().hasInvisibilityPowerUp && !target.GetComponent<PlayerController>().isDead && !seller)
        {
            lastAttack = Time.time;
            animator.SetBool("Attacking", true);
            target.GetComponent<PlayerController>().TakeDamage(damage);//player loses health

        }
    }
    public void TakeDamage(int amount)
    {
        health -= amount;
        //animator.Play("Hurt");
        int x = UnityEngine.Random.Range(0, 10);
        if (x == 0)
        {
            audioManager.Play("EnemyHurt1");
        }
        else if (x == 1)
        {
            audioManager.Play("EnemyHurt2");

        }
        healthBar.SetHealth(health);
        if (health <= 0)
        {
            Die();
        }

    }
    void setRigidBodyState(bool state)
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = state;
        }
        GetComponent<Rigidbody>().isKinematic = !state;
    }

    void setColliderState(bool state)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider coll in colliders)
        {
            if (coll.gameObject.CompareTag("Head")) coll.enabled = true;
            else coll.enabled = state;
        }
        GetComponent<Collider>().enabled = !state;
    }
    void Die()
    {
        if (!isDead)//avoid colling it twice or more
        {
            isDead = true;
            StartCoroutine(TimeToHeadShot());
            gameManager.forks += forksDropped;
            gameManager.enemiesKilled += 1;
            Destroy(gameObject, 5f);
            agent.GetComponent<NavMeshAgent>().enabled = false;
            GetComponentInChildren<Animator>().enabled = false;
            setRigidBodyState(false);
            setColliderState(true);
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                audioManager.Play("EnemyHurt1");
            }
            else
            {
                audioManager.Play("EnemyHurt2");

            }
            //drop
            int x = Random.Range(1, 5);
            Vector3 dropPosition = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
            switch (x)
            {
                case 1:
                    SpawnObject(ammoPack, dropPosition, 70);
                    break;

                case 2:
                    SpawnObject(healthPack, dropPosition, 30);
                    break;
                case 3:
                    SpawnObject(InvisivilityPotionPU, dropPosition, 20);
                    break;
                case 4:
                    SpawnObject(SlowMoPU, dropPosition, 20);
                    break;

            }
        }
    }*/
    private enum State
    {
        Wander,
        ChaseTarget,
        AttackTarget
    }
    private Vector3 startPosition;
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
        startPosition = transform.position;
        wanderPosition = GetWanderPosition();
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
                    wanderPosition = GetWanderPosition();
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
    private Vector3 GetWanderPosition()
    {
        Vector3 randomDirection= new Vector3(UnityEngine.Random.Range(-1f, 1f),0, UnityEngine.Random.Range(-1f, 1f)).normalized;
        Vector3 newDestination= startPosition + randomDirection * Random.Range(10f, 70f);
        return newDestination;
        /* if (Physics.Raycast(newDestination, -transform.up, 2f, whatIsGround))
         {
             destinationSet = true;
             return newDestination; 
         }*/


    }
  
}
