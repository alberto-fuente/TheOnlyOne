using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class WeaponBlueprint : ScriptableObject
{
    [Header("Attributes")]
    public string weaponName;
    public GameObject prefab;
    public Animator anim;
    [Space]
    [Header("Stats")]
    public int maxClipAmmo;
    public int currentAmmo;
    public int totalAmmo;
    public float range;
    public float fireRate;
    public float reloadTime;
    public float aimSpeed;
    public bool autoShoot;
    public float damage;
    [Space]
    [Header("Sway")]
    public float swayIntensity;
    public float swaySpeed;
    [Space]
    [Header("ShootRecoil")]
    public Vector3 recoilRotation;
    public Vector3 aimRecoilRotation;
    public float recoilSpeed;
    public float returnSpeed;
    [Space]
    [Header("VisualRecoil")]
    public Vector3 vRecoilRotation;// (10, 5, 7);
    public Vector3 vRecoilKickBack;//(0.015f, 0, -0.2f);
    [Space]
    public Vector3 vRecoilRotationAim;//(10, 4, 6);
    public Vector3 vRecoilKickBackAim;//(0.015f, 0, -0.2f);
    [Space]
    public float vPositionRecoilSpeed;
    public float vRotationRecoilSpeed;
    [Space]
    public float vPositionReturnSpeed;
    public float vRotationReturnSpeed;
    [Space]
    [Header("VFX")]
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public float pitch;
    public float pitchRand;
    public GameObject bulletDecal;
    [Range(0, 100)] public float mainFOV;
    [Range(0, 100)] public float aimFOV;

}
