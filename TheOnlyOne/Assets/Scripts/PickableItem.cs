using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PickableItem : MonoBehaviour
{
    [Header("Item Components")]
    public string itemName;
    public Rigidbody itemRigidBody;
    public Collider itemCollider;
    public Collider granadeIdleCollider;
    public GameObject prefab;
    public Sprite Icon;
    public Canvas labelCanvas;
    public bool isStackable;
    public string itemID;
    public ItemHolder weaponHolder;
    public GameUtils.TypeOfItem typeOfItem;

    public bool isEquiped;

    private InventorySlot slot;
    public InventorySlot Slot { get => slot; set => slot = value; }
    public int slotId;
    public float distanceToPlayer;
    
    

    void Start()
    {

        weaponHolder = FindObjectOfType<ItemHolder>();
        itemRigidBody = GetComponentInChildren<Rigidbody>();
        itemCollider = GetComponentInChildren<Collider>();
        labelCanvas = transform.GetComponentInChildren<Canvas>();
        labelCanvas.enabled = false;
    }

    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, weaponHolder.transform.position);
        CheckEquiped();
        //Si está equipado, lo renderiza la WeaponCam
        if (typeOfItem.Equals(GameUtils.TypeOfItem.THROWEABLE))
        {
            SetLayerRecursively(gameObject, isEquiped ? 13 : 12);
        }else

        SetLayerRecursively(gameObject, isEquiped ? 7 : 12);

    }

    public static void SetLayerRecursively(GameObject gameObject, int layer)
    {
        foreach (Transform transform in gameObject.GetComponentsInChildren<Transform>(true))
        {
            if (transform.name.Equals("Label")) break;
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
    public void CheckEquiped()
    {
        itemRigidBody.isKinematic = isEquiped;
        itemCollider.isTrigger = isEquiped;
        if (typeOfItem.Equals(GameUtils.TypeOfItem.GUN))
        {
            gameObject.GetComponent<Weapon>().enabled = isEquiped;
            gameObject.GetComponent<VisualRecoil>().enabled = isEquiped;
        }

    }
}
