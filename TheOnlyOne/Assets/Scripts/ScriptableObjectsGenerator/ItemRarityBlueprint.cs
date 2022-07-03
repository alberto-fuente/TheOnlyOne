using UnityEngine;
[CreateAssetMenu(fileName = "New Item Rarity Type", menuName = "Item Rarity Type")]
public class ItemRarityBlueprint : ScriptableObject
{
    [Header("Properties")]
    public string itemName;
    public string rarity;
    public GameObject prefab;
    public int multiplier;
    public float Minprobabilty;
    public float Maxprobabilty;
    [Space]
    [Header("Visual")]
    public Sprite labelIcon;
    public Color color;
    public Sprite inventoryIcon;
}
