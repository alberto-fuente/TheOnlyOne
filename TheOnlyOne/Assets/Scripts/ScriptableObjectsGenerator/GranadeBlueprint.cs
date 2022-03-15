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

    [Space]
    [Header("Stats")]
    public float delay;
    public float radius;
    public float explosionForce;
    public float throwForce;

    [Space]
    [Header("VFX")]
    public GameObject explosionEffect;

}