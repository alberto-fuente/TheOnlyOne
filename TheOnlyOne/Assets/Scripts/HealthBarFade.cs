using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HealthBarFade : MonoBehaviour
{
    public HealthSystem healthSystem;

    public Image healthFill;
    public Image shieldFill;
    public Image healthDamaged;
    public Image shieldDamaged;

    private const float DAMAGED_BAR_SHRINK_TIMER_MAX = 0.3f;
    private const float SHRINKSPEED = 1.5f;
    private float damagedBarShrinkTimer;

    public TMP_Text healthText;
    public TMP_Text shieldText;

    public GameObject popUpText;
    private GameObject lastPopUpText;
    /*private void Awake()
    {
        healthFill = transform.Find("HealthFill").GetComponent<Image>();
        shieldFill = transform.Find("ShieldFill").GetComponent<Image>();
        damagedHealthBar = transform.Find("DamagedHealthBar").GetComponent<Image>();
        damagedShieldBar = transform.Find("DamagedShieldBar").GetComponent<Image>();
    }*/

    private void Start()
    {
        //healthFill = transform.Find("HealthFill").GetComponent<Image>();
        //healthDamaged = transform.Find("HealthDamaged").GetComponent<Image>();
        //shieldFill = transform.Find("ShieldFill").GetComponent<Image>();
        //shieldDamaged = transform.Find("ShieldDamaged").GetComponent<Image>();
        UISetHealth(healthSystem.GetHealthNormalized());
        healthFill.fillAmount = healthFill.fillAmount;
        shieldFill.fillAmount = shieldFill.fillAmount;

        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        healthSystem.OnHealthHealed += HealthSystem_OnHealthHealed;
        healthSystem.OnShieldHealed+= HealthSystem_OnShieldHealed;
        healthSystem.OnDead += HealthSystem_OnDead;
    }

    private void Update()
    {
        damagedBarShrinkTimer -= Time.deltaTime;
        if (damagedBarShrinkTimer < 0)
        {
            if (shieldFill.fillAmount < shieldDamaged.fillAmount)
            {
                shieldDamaged.fillAmount -= SHRINKSPEED * Time.deltaTime;
            }
            if (healthFill.fillAmount < healthDamaged.fillAmount)
            {
                healthDamaged.fillAmount -= SHRINKSPEED * Time.deltaTime;
            }
        }
    }
    private void HealthSystem_OnHealthHealed(object sender, HealthArgs damage)
    {
        UISetHealth(healthSystem.GetHealthNormalized());
        healthDamaged.fillAmount = healthFill.fillAmount;
    }

    private void HealthSystem_OnDamaged(object sender, HealthArgs damage)
    {

        damagedBarShrinkTimer = DAMAGED_BAR_SHRINK_TIMER_MAX;

        UISetShield(healthSystem.GetShieldNormalized());
        UISetHealth(healthSystem.GetHealthNormalized());
        if (popUpText)
        {
            ShowPopUpText(damage.Amount, healthSystem.GetShieldNormalized());
        }
    }

    private void ShowPopUpText(int amount,float shield)
    {
        Destroy(lastPopUpText);
        lastPopUpText = Instantiate(popUpText, healthSystem.transform.position, Quaternion.identity, healthSystem.transform);
        PopUpText popUpTextScript = lastPopUpText.GetComponent<PopUpText>();
        popUpTextScript._text.text = amount.ToString();
        //popUpTextScript._text.material.SetColor("Color", shield > 0 ? popUpTextScript.shieldColor : popUpTextScript.noShieldColor);
        popUpTextScript._text.outlineColor = shield>0?popUpTextScript.shieldColor:popUpTextScript.noShieldColor;
    }

    private void HealthSystem_OnShieldHealed(object sender, HealthArgs damage)
    {
        shieldDamaged.fillAmount = shieldFill.fillAmount;
        UISetShield(healthSystem.GetShieldNormalized());
    }
    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        gameObject.SetActive(false);
    }
    private void UISetHealth(float amount)
    {
        healthFill.fillAmount = amount;
        if(healthText)
            healthText.text = healthSystem.CurrentHealth.ToString("");
    }
    private void UISetShield(float amount)
    {
        shieldFill.fillAmount = amount;
        if (shieldText)
            shieldText.text = healthSystem.CurrentShield.ToString("");
    }
}
