using System;
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    private GameManager gameManager;
    private EnemyIA enemyIA;
    private EnemyBlueprint enemyData;
    private GameObject enemyPrefab;
    private HealthSystem healthSystem;
    private Animator animatorController;
    private Rigidbody[] rigidbodies;
    private Collider groundCollider;
    public PhysicMaterial physicMaterial;

    [Header("Properties")]
    private const int MINITEMSDROP = 2;
    private const int MAXITEMSDROP = 4;
    private float hurtDelay = 1f;
    private float bigHurtDelay = 2.5f;
    private float awareTimeWhenHit = 8f;
    public EnemyBlueprint EnemyData { get => enemyData; set => enemyData = value; }
    public Animator AnimatorController { get => animatorController; set => animatorController = value; }

    private void OnDisable()
    {
        healthSystem.OnDamaged -= HitAnimate;
        healthSystem.OnDamaged -= IncreaseSensors;
        healthSystem.OnDead -= Die;
        healthSystem.OnDead -= DropItems;
    }
    private void Start()
    {
        gameManager = GameManager.Instance;
        enemyIA = GetComponent<EnemyIA>();
        healthSystem = GetComponent<HealthSystem>();
        groundCollider = GetComponent<Collider>();

        EnemyData = GenerateEnemy();
        enemyPrefab = Instantiate(EnemyData.enemyModel, transform);
        AnimatorController = enemyPrefab.GetComponentInChildren<Animator>();
        rigidbodies = enemyPrefab.transform.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            EnemyHitBox hitBox = rb.gameObject.AddComponent<EnemyHitBox>();
            hitBox.HealthSystem = healthSystem;
            rb.GetComponent<Collider>().material = physicMaterial;

        }
        setKinematic(true);
        healthSystem.OnDamaged += HitAnimate;
        healthSystem.OnDamaged += IncreaseSensors;
        healthSystem.OnDead += Die;
        healthSystem.OnDead += DropItems;
    }
    private EnemyBlueprint GenerateEnemy()
    {
        return gameManager.enemyTypes[UnityEngine.Random.Range(0, gameManager.enemyTypes.Length)];
    }
    private void setKinematic(bool state)
    {
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = state;
        }
    }
    //the more health it has or the more damage it recieves, the more chances there are to play hit animation
    private void HitAnimate(object sender, HealthEventArgs damage)
    {
        int hitChance = UnityEngine.Random.Range(0, 100);
        if (hitChance < damage.Amount * 100 / Math.Max((healthSystem.CurrentHealth + healthSystem.CurrentArmor), 1))
        {
            if (damage.Amount > 145)
            {
                AnimatorController.Play("BigHit");
                if (enabled) StartCoroutine(HurtCoroutine(bigHurtDelay));
            }
            else
            {
                AnimatorController.SetInteger("HitIndex", UnityEngine.Random.Range(0, 3));
                AnimatorController.SetTrigger("Hit");
                if (enabled) StartCoroutine(HurtCoroutine(hurtDelay));
            }

        }
    }
    //if hit by player, its sight becomes larger in order to try to detect him
    private void IncreaseSensors(object sender, HealthEventArgs hit)
    {
        if (hit.ByPlayer)
        {
            StartCoroutine(IncreaseSensorsCorroutine());
        }
    }
    IEnumerator IncreaseSensorsCorroutine()
    {
        enemyIA.sightRange = EnemyData.sightRange * 2;
        yield return new WaitForSeconds(awareTimeWhenHit);
        enemyIA.sightRange = EnemyData.sightRange;
    }
    IEnumerator HurtCoroutine(float delay)
    {
        enemyIA.isHurted = true;
        yield return new WaitForSeconds(delay);
        enemyIA.isHurted = false;
    }
    void DropItems(object sender, EventArgs e)
    {
        for (int i = 0; i < UnityEngine.Random.Range(MINITEMSDROP, MAXITEMSDROP + 1); i++)
        {
            GameObject item = gameManager.spawnableItems[UnityEngine.Random.Range(0, gameManager.spawnableItems.Length)];
            Instantiate(item, transform.position + new Vector3(UnityEngine.Random.Range(-2, 2), 3f, UnityEngine.Random.Range(-2, 2)), item.transform.rotation);
        }
    }

    void Die(object sender, EventArgs e)
    {
        gameManager.EntitiesLeft--;
        groundCollider.enabled = false;
        AnimatorController.enabled = false;
        enemyIA.enabled = false;
        healthSystem.enabled = false;
        setKinematic(false);
        Destroy(gameObject, 1.5f);
    }
}
