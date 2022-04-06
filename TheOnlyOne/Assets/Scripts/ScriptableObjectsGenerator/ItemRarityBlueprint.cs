using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Item Rarity Type", menuName = "Item Rarity Type")]
public class ItemRarityBlueprint : ScriptableObject
{
    public string name;
    public GameObject prefab;
    public int multiplier;
    public float Minprobabilty;
    public float Maxprobabilty;

}
