using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] private PackBlueprint packData;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            Collect(other);
        }
    }
    void Collect(Collider collector)
    {
        HealthSystem healthSystem = collector.gameObject.GetComponentInParent<HealthSystem>();
        /*
            * Common: +25
            * Rare: +50
            * Common: +75
            * Common: +100
            */
        healthSystem.HealHealth(healthSystem.MaxHealth / 4 * packData.multiplier);
        Destroy(gameObject);

    }
}
