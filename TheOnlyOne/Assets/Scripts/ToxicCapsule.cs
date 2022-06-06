using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicCapsule : MonoBehaviour
{
    private GameManager gameManager;
    //private float safeRadius;
    public GameObject toxicCupule;//visual
    //Damage
    public float rate=2;//each this seconds
    private float cont = 0;
    public int damage = 25;//deals this damage
    //Shrink
    private float shrinkCont = 0;
    public float shrinkContlimit = 0.01f;//each this seconds
    public float shrinkAmount=1f;//shrinks this much
    private HealthSystem[] aliveEntities;
    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        aliveEntities = FindObjectsOfType<HealthSystem>();
        //safeRadius = gameManager.SafeRadius;
        //shrinkAmount = GetShrinkValue(gameManager.minutesToPlay);

    }
    private void Update()
    {
        cont += Time.deltaTime;
        if (cont >= rate) {
            cont = 0;
            CheckCapsuleToxic();
        }

        shrinkCont += Time.deltaTime;
        if (shrinkCont >= shrinkContlimit)
        {
            float newRadius = gameManager.SafeRadius - shrinkAmount;
            gameManager.SafeRadius = (float)Mathf.Lerp(gameManager.SafeRadius, newRadius, Time.deltaTime);
            //Vector3 newScale=  new Vector3(toxicCupule.transform.localScale.x-shrinkAmount* 30, toxicCupule.transform.localScale.y-shrinkAmount * 30, toxicCupule.transform.localScale.z-shrinkAmount * 30);
            //gameManager.SafeRadius -= shrinkAmount/45;
            //visual(cupule)
            Vector3 newScale = new Vector3(gameManager.SafeRadius, gameManager.SafeRadius, gameManager.SafeRadius);
            toxicCupule.transform.localScale = Vector3.Lerp(toxicCupule.transform.localScale, newScale, Time.deltaTime);
            shrinkCont = 0;
        }


    }
    private void CheckCapsuleToxic()
    {
        foreach(HealthSystem entity in aliveEntities)
        {
            if (entity)
            {
                if (Mathf.Pow(entity.transform.position.x, 2) + Mathf.Pow(entity.transform.position.z, 2) > Mathf.Pow(gameManager.SafeRadius, 2))//distance to center larger than radius)
                {
                    entity.Damage(damage, false,transform);
                }
            }
            
        }
        
    }
    
}
