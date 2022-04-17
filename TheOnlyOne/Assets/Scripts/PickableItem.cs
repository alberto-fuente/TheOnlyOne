using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PickableItem : MonoBehaviour
{
    [Header("Item Components")]
    public string itemName;
    public Rigidbody itemRigidBody;
    public Collider itemCollider;
    public GameObject prefab;
    public Sprite Icon;
    private Canvas labelCanvas;
    public bool isStackable;
    public string itemID;
    public ItemHolder weaponHolder;
    public GameUtils.TypeOfItem typeOfItem;

    public bool isEquiped;
    private GameManager gameManager;
    private InventorySlot slot;
    public InventorySlot Slot { get => slot; set => slot = value; }
    public Canvas LabelCanvas { get => labelCanvas; set => labelCanvas = value; }

    public int slotId;
    public float distanceToPlayer;
    private float headBobTime;
    private float headBobSpeed = 5;

    public float originalY=0.5f;
    private float bobStrength = 0.05f ;
    void Start()
    {

        gameManager = FindObjectOfType<GameManager>();
        //originalY = transform.position.y;
        weaponHolder = FindObjectOfType<ItemHolder>();
        itemRigidBody = GetComponentInChildren<Rigidbody>();
        itemCollider = GetComponentInChildren<Collider>();
        LabelCanvas = transform.GetComponentInChildren<Canvas>();
        LabelCanvas.enabled = false;
        if (typeOfItem == GameUtils.TypeOfItem.THROWEABLE)
        {
            itemID = GetComponent<Granade>().granadeData.name;
        }
    }

    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, weaponHolder.transform.position);
        CheckEquiped();
        headBobTime += Time.deltaTime * headBobSpeed;
        if (!isEquiped)
        {
            //transform.position = new Vector3(transform.position.x, originalY + (float)Mathf.Sin(Time.time)*bobStrength, transform.position.z);
        }
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
        gameManager.hudCrosshair.enabled = true;
        gameObject.SetActive(true);
        

        //animacion de sacar objeto
    }
    public void disableItem()
    {
        gameManager.hudCrosshair.enabled=false;
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
