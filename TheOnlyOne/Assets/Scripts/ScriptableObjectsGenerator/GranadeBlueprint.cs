using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Granade", menuName = "Granade")]
public class GranadeBlueprint : ScriptableObject
{
    [Header("Attributes")]
    public string granadeName;
    public GameObject prefab;
    public Animator anim;
    public Material offMaterial;
    public Material onMaterial;
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
    [Header("VFX")]
    public GameObject explosionEffect;
    public GameObject icePilar;
    public GameObject icePilarFragmented;
    public AudioClip throwSound;
    public AudioClip counterSound;
    public AudioClip explodeSound;
    public AudioClip freezelessSound;


}
