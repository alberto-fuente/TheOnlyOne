using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    public HealthSystem healthSystem;
    public void OnHit(int damage)
    {
        healthSystem.Damage(damage);
    }
}