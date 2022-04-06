using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorPack : MonoBehaviour
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
            * Common: +15
            * Rare: +30
            * Common: +45
            * Common: +60
            */
        healthSystem.HealShield(healthSystem.MaxShield / 4 * packData.multiplier);
        Destroy(gameObject);
    }
}
