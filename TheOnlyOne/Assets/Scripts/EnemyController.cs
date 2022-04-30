using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameManager gameManager;
    public EnemyIA enemyIA;
    public HealthSystem healthSystem;
    public HealthBarFade healthBar;
    private float hurtDelay=1f;
    private Animator animatorController;
    public EnemyBlueprint enemyData;
    public GameObject enemyPrefab;
    Rigidbody[] rigidbodies;
    private const int MINITEMSDROP=2;
    private const int MAXITEMSDROP = 5;
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        enemyIA = GetComponent<EnemyIA>();
        healthSystem = GetComponent<HealthSystem>();
        healthBar = GetComponent<HealthBarFade>();

        enemyData = GenerateEnemy();
        enemyPrefab=Instantiate(enemyData.enemyPrefab, transform);
    }

    private EnemyBlueprint GenerateEnemy()
    {
        return gameManager.enemyTypes[(UnityEngine.Random.Range(0, gameManager.enemyTypes.Length))];
    }

    private void Start()
    {
        animatorController = enemyIA.animatorController;
        rigidbodies = enemyPrefab.transform.GetComponentsInChildren<Rigidbody>();
        foreach(Rigidbody rb in rigidbodies)
        {
            EnemyHitBox hitBox= rb.gameObject.AddComponent<EnemyHitBox>();
            hitBox.healthSystem = healthSystem;

        }
        setRigidBodyState(false);

        healthSystem.OnDamaged += HitAnimate;
        healthSystem.OnDead += Die;
        healthSystem.OnDead += dropItems;
    }
    private void setRigidBodyState(bool state)
    {
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = !state;
        }
    }
    private void HitAnimate(object sender, HealthArgs damage)
    {
        int prob = UnityEngine.Random.Range(0, 100);
        if (prob<30)//30%chance
        {
            if (damage.Amount > 60)
            {
                animatorController.Play("HipHit");
            }
            else
            {
                animatorController.SetTrigger("Hit");
            }
            StartCoroutine(HurtCoroutine(hurtDelay));
        }
    }
    IEnumerator HurtCoroutine(float delay)
    {
        enemyIA.isHurted = true;
        yield return new WaitForSeconds(delay);
        enemyIA.isHurted = false;
    }
    void dropItems(object sender, EventArgs e)
    {
        for (int i = 0; i < UnityEngine.Random.Range(MINITEMSDROP,MAXITEMSDROP); i++)
        {
            GameObject item = gameManager.spawnableItems[UnityEngine.Random.Range(0, gameManager.spawnableItems.Length)];
            Instantiate(item, transform.position+new Vector3(UnityEngine.Random.Range(-2,2),3f, UnityEngine.Random.Range(-2, 2)), item.transform.rotation);
        }
    }

    void Die(object sender, EventArgs e)
    {
        setRigidBodyState(true);
        healthSystem.enabled = false;
        animatorController.enabled = false;
        enemyIA.enabled = false;
        Destroy(gameObject, 5);
    }
}
