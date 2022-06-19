using UnityEngine;

public class ToxicCapsule : MonoBehaviour
{
    [Header("References")]
    private GameManager gameManager;
    public GameObject toxicCupule;
    private HealthSystem[] aliveEntities;

    [Header("Damage")]
    private float cont = 0;
    [SerializeField] private float damageRate = 2;//each this seconds
    [SerializeField] private int damageAmount = 25;//deals this damage

    [Header("Shrink")]
    private float shrinkCont = 0;
    private float shrinkThreshold = 0.05f;//each this seconds
    private float shrinkAmount = 1f;//shrinks this much

    private void Start()
    {
        gameManager = GameManager.Instance;
        aliveEntities = FindObjectsOfType<HealthSystem>();

    }
    private void Update()
    {
        cont += Time.deltaTime;
        if (cont >= damageRate)
        {
            cont = 0;
            CheckEntitiesInside();
        }

        shrinkCont += Time.deltaTime;
        if (shrinkCont >= shrinkThreshold)
        {
            //Logic
            float newRadius = gameManager.SafeRadius - shrinkAmount;
            gameManager.SafeRadius = (float)Mathf.Lerp(gameManager.SafeRadius, newRadius, Time.deltaTime);
            //Visual
            Vector3 newScale = new Vector3(gameManager.SafeRadius, gameManager.SafeRadius, toxicCupule.transform.localScale.z);
            toxicCupule.transform.localScale = Vector3.Lerp(toxicCupule.transform.localScale, newScale, Time.deltaTime);
            shrinkCont = 0;
        }


    }
    //check for entities inside toxic cloud to deal damage
    private void CheckEntitiesInside()
    {
        foreach (HealthSystem entity in aliveEntities)
        {
            if (entity != null)
            {
                if (Mathf.Pow(entity.transform.position.x, 2) + Mathf.Pow(entity.transform.position.z, 2) > Mathf.Pow(gameManager.SafeRadius, 2))//distance to center larger than radius
                {
                    entity.Damage(damageAmount, false, transform);
                }
            }
        }
    }
}
