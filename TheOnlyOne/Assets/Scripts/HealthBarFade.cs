using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HealthBarFade : MonoBehaviour
{
    public HealthSystem healthSystem;
    public bool isPlayer;
    public Image healthFill;
    public Image shieldFill;
    public Image healthDamaged;
    public Image shieldDamaged;

    private const float DAMAGED_BAR_SHRINK_TIMER_MAX = 0.3f;
    private const float SHRINKSPEED = 1.5f;
    private float damagedBarShrinkTimer;

    public TMP_Text healthText;
    public TMP_Text shieldText;

    //enemies
    public GameObject popUpText;
    private GameObject lastPopUpText;
    private float timeBetweenPopups = 0.1f;
    private float elapsedPopupTime = 0;
    //UI
    //DAmage
    public Image damageCanvas;
    private bool fadeDamage;
    public float damageshowTime;
    public float damageFadeSpeed;
    //Heal
    public Image healCanvas;
    private bool fadeHeal;
    public float healshowTime;
    public float healFadeSpeed;
    //Armor
    private bool fadeArmor;
    public Image armorCanvas;
    public float armorshowTime;
    public float armorFadeSpeed;
    private bool armorCanBeBroken=true;
    private AudioSource audioSource;
    [SerializeField] private AudioClip healSound;
    [SerializeField] private AudioClip armorSound;
    [SerializeField] private AudioClip armorBrokenSound;
    [SerializeField] private AudioClip hitSound;

    //KILL
    public GameObject killPanel;
    //CAmShake
    private CameraShake playerCamera;
    /*private void Awake()
    {
        healthFill = transform.Find("HealthFill").GetComponent<Image>();
        shieldFill = transform.Find("ShieldFill").GetComponent<Image>();
        damagedHealthBar = transform.Find("DamagedHealthBar").GetComponent<Image>();
        damagedShieldBar = transform.Find("DamagedShieldBar").GetComponent<Image>();
    }*/
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        playerCamera = FindObjectOfType<CameraShake>();
    }
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
        elapsedPopupTime += Time.deltaTime;
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
        if (isPlayer)
        {
            CheckUIfade(damageCanvas, fadeDamage,damageFadeSpeed);
            CheckUIfade(healCanvas, fadeHeal,healFadeSpeed);
            //CheckUIfade(armorCanvas, fadeArmor,armorFadeSpeed);
        }

    }
    private void HealthSystem_OnHealthHealed(object sender, HealthArgs damage)
    {
        //bhealthbar
        UISetHealth(healthSystem.GetHealthNormalized());
        healthDamaged.fillAmount = healthFill.fillAmount;
        //VFX
        audioSource.PlayOneShot(healSound);
        //UI
        fadeHeal = true;
        StartCoroutine(switchOff("Heal",healshowTime));
    }

    private void HealthSystem_OnDamaged(object sender, HealthArgs damage)
    {

        damagedBarShrinkTimer = DAMAGED_BAR_SHRINK_TIMER_MAX;

        UISetShield(healthSystem.GetShieldNormalized());
        UISetHealth(healthSystem.GetHealthNormalized());
        //enemy pop up damage
        if (popUpText&&elapsedPopupTime>=timeBetweenPopups)
        {
            elapsedPopupTime = 0;
            ShowPopUpText(damage.Amount, healthSystem.GetShieldNormalized());
        }
        if(isPlayer)//player
        {
            audioSource.PlayOneShot(hitSound);
            //UI
            playerCamera.ShakeCamera(0.1f, 0.005f * damage.Amount);
            UpdateArmorUI();
            if (!DamageIndicatorSystem.CheckIfObjectInSight(damage.SourceTransform))
            {
                DamageIndicatorSystem.CreateIndicator(damage.SourceTransform);
            }
            fadeDamage = true;
            StartCoroutine(switchOff("Damage", damageshowTime));
        }


    }

    private void ShowPopUpText(int amount,float shield)
    {
        if(lastPopUpText)Destroy(lastPopUpText);
        lastPopUpText = Instantiate(popUpText, healthSystem.transform.position+new Vector3(0,2f,0), Quaternion.identity,healthSystem.transform);
        PopUpText popUpTextScript = lastPopUpText.GetComponent<PopUpText>();
        popUpTextScript._text.text = amount.ToString();
        //popUpTextScript._text.material.SetColor("Color", shield > 0 ? popUpTextScript.shieldColor : popUpTextScript.noShieldColor);
        popUpTextScript._text.outlineColor = shield>0?popUpTextScript.shieldColor:popUpTextScript.noShieldColor;
        
    }

    private void HealthSystem_OnShieldHealed(object sender, HealthArgs amount)
    {
        shieldDamaged.fillAmount = shieldFill.fillAmount;
        UISetShield(healthSystem.GetShieldNormalized());
        audioSource.PlayOneShot(armorSound);
        //UI
        fadeArmor = true;
        if (isPlayer)
        {
            StartCoroutine(switchOff("Armor", armorshowTime));
            armorCanBeBroken = true;
            UpdateArmorUI();
        }

    }
    private void HealthSystem_OnDead(object sender, HealthArgs LastHit)
    {
        //gameObject.SetActive(false);
        if (isPlayer)
        {

        }
        if (LastHit.ByPlayer)
        {
            GameObject panel=Instantiate(killPanel, GameObject.Find("HUD").transform);
            FindObjectOfType<GameManager>().beatenEnemies++;
            FindObjectOfType<GameManager>().defeatedEnemies.text = FindObjectOfType<GameManager>().beatenEnemies.ToString();

        }
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

    void CheckUIfade(Image image, bool fadeIn,float speed)
    {
        bool fadeOut = true;
        if (fadeIn)
        {
            fadeOut = false;
            float alpha = image.color.a;
            if (alpha < 1)
            {
                alpha += Time.deltaTime*speed;
                if (alpha >= 1)
                {
                    alpha = 1;
                    fadeIn = false;
                    fadeOut = true;
 
                }
                image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            }
        }
        if (fadeOut)
        {
            float alpha = image.color.a;
            if (alpha >0)
            {
                alpha -= Time.deltaTime * speed;
                if (alpha <=0)
                {
                    alpha = 0;
                    fadeOut = false;
                }
                image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            }
        }
    }
    IEnumerator switchOff(string name, float delay)
    {
        yield return new WaitForSeconds(delay);
        switch (name)
        {
            case "Damage": 
                fadeDamage = false;
                break;
            case "Heal":
                fadeHeal = false;
                break;
            case "Armor":
                fadeArmor = false;
                break;
        }
        

    }
    private void UpdateArmorUI()//el escudo se ilumina más intensamente en funcion de lo que quede de él.
    {
        float armorLevel = healthSystem.GetShieldNormalized();
        armorCanvas.color = new Color(armorCanvas.color.r, armorCanvas.color.g, armorCanvas.color.b, armorLevel);
        if (armorLevel <= 0&&armorCanBeBroken)
        {
            audioSource.PlayOneShot(armorBrokenSound);
            armorCanBeBroken = false;
        }
    }
}
