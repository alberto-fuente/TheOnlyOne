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
    private float bigHurtDelay = 2.5f;
    private Animator animatorController;
    public EnemyBlueprint enemyData;
    public GameObject enemyPrefab;
    Rigidbody[] rigidbodies;
    private const int MINITEMSDROP=2;
    private const int MAXITEMSDROP = 5;
    public PhysicMaterial physicMaterial;
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
        animatorController = enemyPrefab.GetComponentInChildren<Animator>();
        rigidbodies = enemyPrefab.transform.GetComponentsInChildren<Rigidbody>();
        foreach(Rigidbody rb in rigidbodies)
        {
            EnemyHitBox hitBox= rb.gameObject.AddComponent<EnemyHitBox>();
            hitBox.healthSystem = healthSystem;
            rb.GetComponent<Collider>().material = physicMaterial;

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
        int hitChance = UnityEngine.Random.Range(0, 100);
        if (hitChance < damage.Amount*100/ Math.Max((healthSystem.CurrentHealth + healthSystem.CurrentShield),1))//cuanto mas daño haga o menos vida tenga, más probabilidad de hacer la animacion (se suma 1 para evitar dividir por 0)
        {
            if (damage.Amount > 145)
            {
                animatorController.Play("BigHit");
                StartCoroutine(HurtCoroutine(bigHurtDelay));
            }
            else
            {
                animatorController.SetInteger("HitIndex", UnityEngine.Random.Range(0, 3));
                animatorController.SetTrigger("Hit");
                StartCoroutine(HurtCoroutine(hurtDelay));
            }
            
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
