using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LabelWeapon: MonoBehaviour
{
    private Transform cameraTransform;
    public TMP_Text weaponNameText;

    public WeaponBlueprint weapon;
    public Image damageBar;
    public Image rangeBar;
    public Image FireRateBar;

    void Start()
    {
        cameraTransform = GameObject.Find("/CameraHolder/CameraRecoil/MainCamera").transform;
        weaponNameText.text = weapon.weaponName;

        damageBar.fillAmount = weapon.damage / GameUtils.maxDamage;
        rangeBar.fillAmount = weapon.range / GameUtils.maxRange;
        FireRateBar.fillAmount = GameUtils.maxFireRate - weapon.fireRate;
    }
    void LateUpdate()
    {
        transform.LookAt(transform.position + cameraTransform.forward);

    }

}
