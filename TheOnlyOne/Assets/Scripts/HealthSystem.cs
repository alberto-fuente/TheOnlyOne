using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler<HealthArgs> OnDamaged;
    public event EventHandler<HealthArgs> OnHealthHealed;
    public event EventHandler<HealthArgs> OnShieldHealed;
    public event EventHandler<HealthArgs> OnShieldDestroyed;
    public event EventHandler OnDead;

    [SerializeField] const int MAXHEALTH = 100;
    [SerializeField] const int MAXSHIELD = 100;

    int currentHealth;
    int currentShield;

    public bool isDead;
    public bool waslastHitHead;
    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public int CurrentShield { get => currentShield; set => currentShield = value; }
    public int MaxHealth { get => MAXHEALTH; }
    public int MaxShield { get => MAXSHIELD; }

    private void Awake()
    {
        CurrentHealth = MaxHealth;
        CurrentShield = MaxShield;
    }
    public void HealHealth(int amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth;
        if (OnHealthHealed != null) OnHealthHealed(this, new HealthArgs(amount));
    }
    public void HealShield(int amount)
    {
        CurrentShield += amount;
        if (CurrentShield > MaxShield) CurrentShield = MaxShield;
        if (OnShieldHealed != null) OnShieldHealed(this, new HealthArgs(amount));
    }
    public void Damage(int amount)
    {
        var dif = CurrentShield - amount;
        CurrentShield = dif;
        if (CurrentShield < 0)
        {
            CurrentShield = 0;
            if (OnShieldDestroyed != null) OnShieldDestroyed(this, new HealthArgs(amount));
            CurrentHealth -= Mathf.Abs(dif);
        }

        if (OnDamaged != null) OnDamaged(this, new HealthArgs(amount));

        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
            if (!isDead) Die();
        }
        
    }
    public void Die()
    {
        isDead = true;
        if (OnDead != null) OnDead(this, EventArgs.Empty);
    }
    public float GetHealthNormalized()
    {
        return (float)CurrentHealth / MaxHealth;
    }
    public float GetShieldNormalized()
    {
        return (float)CurrentShield / MaxShield;
    }
}
