using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Granade", menuName = "Granade")]
public class GranadeBlueprint : ScriptableObject
{
    [Header("Attributes")]
    public int id;
    public string name;
    public string granadeType;
    public GameObject prefab;
    public Material Material;
    public float Minprobabilty;
    public float Maxprobabilty;
    public Material freezeMaterial;
    public int effectDuration;
    [Space]
    [Header("Stats")]
    public float delay;
    public float radius;
    public float explosionForce;
    public float throwForce;
    public int damage;
    [Space]
    [Header("Sway")]
    public int swayIntensity;
    public int swaySpeed;
    [Space]
    [Header("VFX")]
    public GameObject explosionEffect;
    public GameObject icePilar;
    public GameObject icePilarFragmented;
    public AudioClip throwSound;
    public AudioClip counterSound;
    public AudioClip explodeSound;
    public AudioClip freezelessSound;
    public float shakeDuration;
    public float shakeMagnitude;
    [Range(0, 100)] public float mainFOV;
    [Range(0, 100)] public float aimFOV;
    [Space]
    [Header("Visual")]
    public Sprite labelIcon;
    public Color color;
    public Sprite inventoryIcon;



}
