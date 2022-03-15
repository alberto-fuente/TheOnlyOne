using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HealthBarFade : MonoBehaviour
{
    private const float DAMAGED_BAR_SHRINK_TIMER_MAX = 0.7f;
    
    public HealthSystem healthSystem;

    public Image healthFill;
    public Image shieldFill;

    public Image damagedHealthBar;
    public Image damagedShieldBar;
    private float damagedBarShrinkTimer;
    private float shrinkSpeed = 1.3f;

    public TMP_Text healthText;
    public TMP_Text shieldText;
    private void Awake()
    {
        healthFill = transform.Find("HealthFill").GetComponent<Image>();
        shieldFill = transform.Find("ShieldFill").GetComponent<Image>();
        damagedHealthBar = transform.Find("DamagedHealthBar").GetComponent<Image>();
        damagedShieldBar = transform.Find("DamagedShieldBar").GetComponent<Image>();
    }
    private void Start()
    {
        SetHealth(healthSystem.GetHealthNormalized());
        damagedHealthBar.fillAmount = healthFill.fillAmount;
        damagedShieldBar.fillAmount = shieldFill.fillAmount;

        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        healthSystem.OnHealthHealed += HealthSystem_OnHealthHealed;
        healthSystem.OnShieldHealed+= HealthSystem_OnShieldHealed;
    }
    private void Update()
    {
        damagedBarShrinkTimer -= Time.deltaTime;
        if (damagedBarShrinkTimer < 0)
        {
            if (shieldFill.fillAmount < damagedShieldBar.fillAmount)
            {
                damagedShieldBar.fillAmount -= shrinkSpeed * Time.deltaTime;
            }
            if (healthFill.fillAmount < damagedHealthBar.fillAmount)
            {
                damagedHealthBar.fillAmount -= shrinkSpeed * Time.deltaTime;
            }
        }
    }
    private void HealthSystem_OnHealthHealed(object sender, System.EventArgs e)
    {
        SetHealth(healthSystem.GetHealthNormalized());
        damagedHealthBar.fillAmount = healthFill.fillAmount;
    }

    private void HealthSystem_OnDamaged(object sender, System.EventArgs e)
    {

        damagedBarShrinkTimer = DAMAGED_BAR_SHRINK_TIMER_MAX;

        SetShield(healthSystem.GetShieldNormalized());
        SetHealth(healthSystem.GetHealthNormalized());

    }
    private void HealthSystem_OnShieldHealed(object sender, EventArgs e)
    {
        damagedShieldBar.fillAmount = shieldFill.fillAmount;
        SetShield(healthSystem.GetShieldNormalized());
    }

    private void SetHealth(float amount)
    {
        healthFill.fillAmount = amount;
        healthText.text = healthSystem.CurrentHealth.ToString("");
    }
    private void SetShield(float amount)
    {
        shieldFill.fillAmount = amount;
        shieldText.text = healthSystem.CurrentShield.ToString("");
    }
}