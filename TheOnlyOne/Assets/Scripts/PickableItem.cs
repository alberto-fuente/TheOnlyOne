using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PickableItem : MonoBehaviour
{
    [Header("Item Components")]
    public Rigidbody itemRigidBody;
    public Collider itemCollider;

    public WeaponHolder weaponHolder;
    public GameUtils.TypeOfItem typeOfItem;

    private bool isEquiped;
    public string itemID;

    private float distanceToPlayer;

    public bool IsEquiped { get => isEquiped; set => isEquiped = value; }

    void Start()
    {
        weaponHolder = FindObjectOfType<WeaponHolder>();
        itemRigidBody = GetComponentInChildren<Rigidbody>();
        itemCollider = GetComponentInChildren<Collider>();
    }

    void Update()
    {
        distanceToPlayer = Vector3.Distance(weaponHolder.transform.position, transform.position);
        CheckEquiped();

        //Si está equipado, lo renderiza la WeaponCam
        SetLayerRecursively(gameObject, IsEquiped ? 7 : 0);

    }
    public static void SetLayerRecursively(GameObject gameObject, int layer)
    {
        foreach (Transform transform in gameObject.GetComponentsInChildren<Transform>(true))
        {
            transform.gameObject.layer = layer;
        }
    }
    public void EnableItem()
    {
        gameObject.SetActive(true);

        //animacion de sacar objeto
    }
    public void disableItem()
    {
        gameObject.SetActive(false);

        //animacion de guardar objeto
    }
    void CheckEquiped()
    {
        itemRigidBody.isKinematic = IsEquiped;
        itemCollider.isTrigger = IsEquiped;
        if (typeOfItem.Equals(GameUtils.TypeOfItem.GUN)) gameObject.GetComponent<Weapon>().enabled = IsEquiped;
    }
    
}
