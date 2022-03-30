using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BillBoardOld : MonoBehaviour
{
    private Transform cameraTransform;
    public WeaponBlueprint weapon;
    private float numberOfStats;
    public float maxDamage, maxRange, maxFireRate;
    public Slider slider;
    public TMP_Text weaponName;
    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = GameObject.Find("/CameraHolder/CameraRecoil/MainCamera").transform;

        weaponName.text = weapon.weaponName;

        slider.maxValue = maxDamage;
        slider.value = weapon.damage;
/*
        slider[1].maxValue = maxRange;
        slider[1].value = weapon.range;

        slider[2].maxValue = maxFireRate;
        slider[2].value = weapon.fireRate;
*/
    }

     void LateUpdate()
    {
        transform.LookAt(transform.position + cameraTransform.forward);
    }


}
