using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    private HealthSystem healthSystem;
    public HealthSystem HealthSystem { get => healthSystem; set => healthSystem = value; }

    public void OnHit(int damage,Transform tranform)
    {
        HealthSystem.Damage(damage,true,tranform);
    }
}
