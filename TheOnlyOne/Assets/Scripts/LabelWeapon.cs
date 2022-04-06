using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LabelWeapon: MonoBehaviour
{
    public TMP_Text weaponNameText;

    public Weapon weapon;
    public WeaponBlueprint data;
    public ItemRarityBlueprint rarity;
    public Image damageBar;
    public Image rangeBar;
    public Image FireRateBar;

    [SerializeField] private Image mainCanvas;
    [SerializeField] private Image pickCanvas;

    private Color mainColor=new Color(7f,0f,15f, 10f);
    private Color pickColor = new Color(3f, 16f, 2f,36f);

    void Start()
    {
        data=weapon.weaponData;
        rarity = weapon.rarityData;
        weaponNameText.text = data.weaponName;
        mainCanvas = GetComponent<Image>();
        pickCanvas = transform.GetChild(0).GetComponent<Image>();
        mainCanvas.color = mainColor;
        pickCanvas.color = pickColor;
        damageBar.fillAmount = data.damage*rarity.multiplier / GameUtils.maxDamage;
        rangeBar.fillAmount = data.range / GameUtils.maxRange;
        FireRateBar.fillAmount = GameUtils.maxFireRate - data.fireRate;
    }
  
}
