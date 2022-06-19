using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler<HealthArgs> OnDamaged;
    public event EventHandler<HealthArgs> OnHealthHealed;
    public event EventHandler<HealthArgs> OnArmorHealed;
    public event EventHandler<HealthArgs> OnArmorDestroyed;
    public event EventHandler<HealthArgs> OnDead;

    [SerializeField] const int MAXHEALTH = 100;
    [SerializeField] const int MAXARMOR = 100;

    int currentHealth;
    int currentArmor;

    public bool isDead;
    public bool waslastHitHead;
    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public int CurrentArmor { get => currentArmor; set => currentArmor = value; }
    public int MaxHealth { get => MAXHEALTH; }
    public int MaxArmor { get => MAXARMOR; }

    private void Awake()
    {
        CurrentHealth = MaxHealth;
        CurrentArmor = MaxArmor;
    }
    public void HealHealth(int amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth;
        if (OnHealthHealed != null) OnHealthHealed(this, new HealthArgs(amount));
    }
    public void HealShield(int amount)
    {
        CurrentArmor += amount;
        if (CurrentArmor > MaxArmor) CurrentArmor = MaxArmor;
        if (OnArmorHealed != null) OnArmorHealed(this, new HealthArgs(amount));
    }
    public void Damage(int amount,bool byPlayer,Transform sourceTransform)
    {
        var dif = CurrentArmor - amount;
        CurrentArmor = dif;
        if (CurrentArmor < 0)
        {
            CurrentArmor = 0;
            if (OnArmorDestroyed != null) OnArmorDestroyed(this, new HealthArgs(amount));
            CurrentHealth -= Mathf.Abs(dif);
        }

        if (OnDamaged != null)
        {
            OnDamaged(this, new HealthArgs(amount,byPlayer, sourceTransform));//enemy sensors increase if hit by player
        }

        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
            if (!isDead) Die(byPlayer);
        }
        
    }
    public void Die(bool byPlayer)
    {
        isDead = true;
        if (OnDead != null) OnDead(this, new HealthArgs(byPlayer));
    }
    public float GetHealthNormalized()
    {
        return (float)CurrentHealth / MaxHealth;
    }
    public float GetArmorNormalized()
    {
        return (float)CurrentArmor / MaxArmor;
    }
}
