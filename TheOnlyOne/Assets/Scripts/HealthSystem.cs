using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthSystem : MonoBehaviour
{
    public event EventHandler OnDamaged;
    public event EventHandler OnHealthHealed;
    public event EventHandler OnShieldHealed;
    public event EventHandler OnShieldDestroyed;
    public event EventHandler OnDead;

    int maxHealth=100;
    int maxShield=60;

    int currentHealth;
    int currentShield;

    bool isDead;
    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public int CurrentShield { get => currentShield; set => currentShield = value; }

    private void Awake()
    {
        CurrentHealth = maxHealth;
        CurrentShield = maxShield;
    }
    private void Update()
    {
        if (currentHealth <= 0&&!isDead)
        {
            Die();
        }

    }
    public void HealHealth(int amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth >maxHealth) CurrentHealth = maxHealth;
        if (OnHealthHealed != null) OnHealthHealed(this, EventArgs.Empty);
    }
    public void HealShield(int amount)
    {
        CurrentShield += amount;
        if (CurrentShield > maxShield) CurrentShield = maxShield;
        if (OnShieldHealed != null) OnShieldHealed(this, EventArgs.Empty);
    }
    public void Damage(int amount)
    {
        var dif = CurrentShield - amount;
        CurrentShield = dif;
        if (CurrentShield < 0)
        {
            CurrentShield = 0;
            if (OnShieldDestroyed != null) OnShieldDestroyed(this, EventArgs.Empty);
            CurrentHealth -= Mathf.Abs(dif);
        }

        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
            if (OnDead != null) OnDead(this, EventArgs.Empty);
        }
        if (OnDamaged != null) OnDamaged(this, EventArgs.Empty);
    }
    public void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }
    public float GetHealthNormalized()
    {
        return (float)CurrentHealth / maxHealth;
    }
    public float GetShieldNormalized()
    {
        return (float)CurrentShield / maxShield;
    }
}
