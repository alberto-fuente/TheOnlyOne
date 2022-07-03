using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystemVisuals : MonoBehaviour
{
    [Header("References")]
    private AudioManager audioManager;
    public HealthSystem healthSystem;
    private CameraShake playerCamera;
    public GameObject killPanel;

    [Header("Player Health")]
    public bool isPlayer;
    public Image healthFill;
    public Image armorFill;
    public Image healthDamaged;
    public Image armorDamaged;
    public TMP_Text healthText;
    public TMP_Text armorText;
    private const float DAMAGED_BAR_SHRINK_TIMER_MAX = 0.3f;
    private const float SHRINKSPEED = 1.5f;
    private float damagedBarShrinkTimer;

    [Header("Enemies")]
    public GameObject popUpText;
    private GameObject lastPopUpText;
    private float timeBetweenPopups = 0.1f;
    private float elapsedPopupTime = 0;

    [Header("UI")]
    [Header("Damage")]
    public Image damageCanvas;
    private bool fadeDamage;
    public float damageshowTime;
    public float damageFadeSpeed;

    [Header("Heal")]
    public Image healCanvas;
    private bool fadeHeal;
    public float healshowTime;
    public float healFadeSpeed;

    [Header("Armor")]
    public Image armorCanvas;
    public float armorshowTime;
    public float armorFadeSpeed;
    private bool armorCanBeBroken = true;

    [Header("Sounds")]
    [SerializeField] private AudioClip healSound;
    [SerializeField] private AudioClip armorSound;
    [SerializeField] private AudioClip armorBrokenSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip killSound;

    private void Awake()
    {
        audioManager = AudioManager.Instance;
        playerCamera = FindObjectOfType<CameraShake>();
    }
    private void Start()
    {
        UISetHealth(healthSystem.GetHealthNormalized());
        UISetArmor(healthSystem.GetArmorNormalized());

        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        healthSystem.OnHealthHealed += HealthSystem_OnHealthHealed;
        healthSystem.OnArmorHealed += HealthSystem_OnarmorHealed;
        healthSystem.OnDead += HealthSystem_OnDead;
    }
    private void OnDisable()
    {
        healthSystem.OnDamaged -= HealthSystem_OnDamaged;
        healthSystem.OnHealthHealed -= HealthSystem_OnHealthHealed;
        healthSystem.OnArmorHealed -= HealthSystem_OnarmorHealed;
        healthSystem.OnDead -= HealthSystem_OnDead;
    }
    private void Update()
    {
        damagedBarShrinkTimer -= Time.deltaTime;
        elapsedPopupTime += Time.deltaTime;
        if (damagedBarShrinkTimer < 0)
        {
            if (armorFill.fillAmount < armorDamaged.fillAmount)
            {
                armorDamaged.fillAmount -= SHRINKSPEED * Time.deltaTime;
            }
            if (healthFill.fillAmount < healthDamaged.fillAmount)
            {
                healthDamaged.fillAmount -= SHRINKSPEED * Time.deltaTime;
            }
        }
        if (isPlayer)
        {
            CheckUIfade(damageCanvas, fadeDamage, damageFadeSpeed);
            CheckUIfade(healCanvas, fadeHeal, healFadeSpeed);
        }

    }
    private void HealthSystem_OnHealthHealed(object sender, HealthEventArgs damage)
    {
        UISetHealth(healthSystem.GetHealthNormalized());
        healthDamaged.fillAmount = healthFill.fillAmount;
        audioManager.PlaySound(healSound);
        fadeHeal = true;
        StartCoroutine(switchOff("Heal", healshowTime));
    }

    private void HealthSystem_OnDamaged(object sender, HealthEventArgs damage)
    {
        damagedBarShrinkTimer = DAMAGED_BAR_SHRINK_TIMER_MAX;

        UISetArmor(healthSystem.GetArmorNormalized());
        UISetHealth(healthSystem.GetHealthNormalized());
        //enemy pop up damage
        if (popUpText && elapsedPopupTime >= timeBetweenPopups)
        {
            elapsedPopupTime = 0;
            ShowPopUpText(damage.Amount, healthSystem.GetArmorNormalized());
        }
        if (isPlayer)
        {
            audioManager.PlaySound(hitSound);
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

    private void ShowPopUpText(int amount, float armor)
    {
        if (lastPopUpText) Destroy(lastPopUpText);
        lastPopUpText = Instantiate(popUpText, healthSystem.transform.position + new Vector3(0, 2f, 0), Quaternion.identity, healthSystem.transform);
        PopUpText popUpTextScript = lastPopUpText.GetComponent<PopUpText>();
        popUpTextScript.Text.text = amount.ToString();
        popUpTextScript.Text.outlineColor = armor > 0 ? popUpTextScript.ArmorColor : popUpTextScript.NoArmorColor;
        //popUpTextScript._text.material.SetColor("Color", armor > 0 ? popUpTextScript.armorColor : popUpTextScript.noarmorColor);

    }

    private void HealthSystem_OnarmorHealed(object sender, HealthEventArgs amount)
    {
        armorDamaged.fillAmount = armorFill.fillAmount;
        UISetArmor(healthSystem.GetArmorNormalized());
        audioManager.PlaySound(armorSound);
        if (isPlayer)
        {
            StartCoroutine(switchOff("Armor", armorshowTime));
            armorCanBeBroken = true;
            UpdateArmorUI();
        }

    }
    private void HealthSystem_OnDead(object sender, HealthEventArgs LastHit)
    {
        if (LastHit.ByPlayer)
        {
            if (killPanel != null)
            {
                audioManager.PlaySound(killSound);
                Destroy(Instantiate(killPanel, GameObject.Find("HUD").transform), 5);
            }
            GameManager.Instance.beatenEnemies++;
            GameManager.Instance.defeatedEnemies.text = GameManager.Instance.beatenEnemies.ToString();

        }
    }
    private void UISetHealth(float amount)
    {
        healthFill.fillAmount = amount;
        if (healthText) healthText.text = healthSystem.CurrentHealth.ToString("");
    }
    private void UISetArmor(float amount)
    {
        armorFill.fillAmount = amount;
        if (armorText) armorText.text = healthSystem.CurrentArmor.ToString("");
    }
    //armor's visibility depends on its value
    private void UpdateArmorUI()
    {
        float armorLevel = healthSystem.GetArmorNormalized();
        armorCanvas.color = new Color(armorCanvas.color.r, armorCanvas.color.g, armorCanvas.color.b, armorLevel);
        if (armorLevel <= 0 && armorCanBeBroken)
        {
            audioManager.PlaySound(armorBrokenSound);
            armorCanBeBroken = false;
        }
    }
    //fades out UI panels
    void CheckUIfade(Image image, bool fadeIn, float speed)
    {
        bool fadeOut = true;
        if (fadeIn)
        {
            fadeOut = false;
            float alpha = image.color.a;
            if (alpha < 1)
            {
                alpha += Time.deltaTime * speed;
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
            if (alpha > 0)
            {
                alpha -= Time.deltaTime * speed;
                if (alpha <= 0)
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
        }
    }
}
