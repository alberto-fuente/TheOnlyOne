using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Pack Type", menuName = "Pack Type")]
public class PackBlueprint : ScriptableObject
{
    public string name;
    public GameObject prefab;
    public int multiplier;
    public float Minprobabilty;
    public float Maxprobabilty;
}
