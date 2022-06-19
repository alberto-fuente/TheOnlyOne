using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Pickable Item", menuName = "Pickable Item")]
public class PickableBlueprint : ScriptableObject
{
    public string itemName;
    public Rigidbody itemRigidBody;
    public Collider itemCollider;
    public GameObject prefab;
    public Sprite Icon;
    public Canvas labelCanvas;
    public bool isStackable;
    public string itemID;
    public PlayerInventory weaponHolder;
    public GameUtils.TypeOfItem typeOfItem;
}
