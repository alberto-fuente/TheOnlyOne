using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler<HealthEventArgs> OnDamaged;
    public event EventHandler<HealthEventArgs> OnHealthHealed;
    public event EventHandler<HealthEventArgs> OnArmorHealed;
    public event EventHandler<HealthEventArgs> OnDead;

    [SerializeField] const int MAXHEALTH = 100;
    [SerializeField] const int MAXARMOR = 100;

    int currentHealth;
    int currentArmor;

    private bool isDead;
    private bool waslastHitHead;
    public int MaxHealth { get => MAXHEALTH; }
    public int MaxArmor { get => MAXARMOR; }
    public int CurrentHealth { get => currentHealth; private set => currentHealth = value; }
    public int CurrentArmor { get => currentArmor; private set => currentArmor = value; }
    public bool IsDead { get => isDead; private set => isDead = value; }
    public bool WaslastHitHead { get => waslastHitHead; set => waslastHitHead = value; }

    private void Awake()
    {
        CurrentHealth = MaxHealth;
        CurrentArmor = MaxArmor;
    }
    public void HealHealth(int amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth;
        if (OnHealthHealed != null) OnHealthHealed(this, new HealthEventArgs(amount));
    }
    public void HealShield(int amount)
    {
        CurrentArmor += amount;
        if (CurrentArmor > MaxArmor) CurrentArmor = MaxArmor;
        if (OnArmorHealed != null) OnArmorHealed(this, new HealthEventArgs(amount));
    }
    public void Damage(int amount, bool byPlayer, Transform sourceTransform)
    {
        var dif = CurrentArmor - amount;
        CurrentArmor = dif;
        if (CurrentArmor < 0)
        {
            CurrentArmor = 0;
            CurrentHealth -= Mathf.Abs(dif);
        }
        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
            if (!IsDead) Die(byPlayer);
        }
        if (OnDamaged != null)
        {
            OnDamaged(this, new HealthEventArgs(amount, byPlayer, sourceTransform));//enemy sensors increase if hit by player
        }
    }
    public void Die(bool byPlayer)
    {
        IsDead = true;
        if (OnDead != null) OnDead(this, new HealthEventArgs(byPlayer));
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
