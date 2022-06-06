using UnityEngine;


public class GrabbableItem : MonoBehaviour
{
    [Header("Item Components")]
    public string itemName;
    public Rigidbody itemRigidBody;
    public Collider itemCollider;
    public GameObject prefab;
    public Sprite Icon;
    public Label label;
    public bool isStackable;
    public string itemID;
    public ItemHolder weaponHolder;
    public GameUtils.TypeOfItem typeOfItem;
    public GameObject armMesh;//hide arms when not picked
    public bool isEquiped;
    private bool equipFlag;
    private GameManager gameManager;
    private InventorySlot slot;
    public InventorySlot Slot { get => slot; set => slot = value; }
    public int slotId;

    void Start()
    {

        gameManager = FindObjectOfType<GameManager>();
        //originalY = transform.position.y;
        weaponHolder = FindObjectOfType<ItemHolder>();
        label = GetComponentInChildren<Label>();
        itemRigidBody = GetComponentInChildren<Rigidbody>();
        itemCollider = GetComponentInChildren<Collider>();
        if (typeOfItem == GameUtils.TypeOfItem.THROWEABLE)
        {
            itemID = GetComponent<Granade>().granadeData.name;
            Icon = GetComponent<Granade>().granadeData.inventoryIcon;
        }
        else if (typeOfItem == GameUtils.TypeOfItem.GUN)
        {
            Icon = GetComponent<Weapon>().rarityData.inventoryIcon;
        }
        armMesh = GetComponentInChildren<SkinnedMeshRenderer>().gameObject;
        //itemRigidBody.isKinematic = true;
    }

    void Update()
    {
        CheckEquiped();
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
    public void DisableItem()
    {
        gameManager.hudCrosshair.enabled = false;
        gameObject.SetActive(false);
        gameObject.GetComponentInChildren<Animator>().enabled = false;
        //animacion de guardar objeto
    }
    public void CheckEquiped()
    {
        itemRigidBody.isKinematic = isEquiped;
        itemCollider.isTrigger = isEquiped;

        armMesh.SetActive(isEquiped);
        if (typeOfItem.Equals(GameUtils.TypeOfItem.GUN))
        {
            gameObject.GetComponent<Weapon>().enabled = isEquiped;
            gameObject.GetComponent<VisualRecoil>().enabled = isEquiped;
            gameObject.GetComponentInChildren<Animator>().enabled = isEquiped;
        }
        else if (typeOfItem.Equals(GameUtils.TypeOfItem.THROWEABLE))
        {
            if (!gameObject.GetComponent<Granade>().hasBeenthrown)
            {
                gameObject.GetComponent<Granade>().enabled = isEquiped;
                gameObject.GetComponentInChildren<Animator>().enabled = isEquiped;
            }
            else
            {
                gameObject.GetComponent<Granade>().enabled = true;
                gameObject.GetComponentInChildren<Animator>().enabled = true;
            }
        }

        //Si está equipado, lo renderiza la WeaponCam
        if (equipFlag != isEquiped)//para que solo se compruebe cuando se equipa o desequipa
        {
            equipFlag = isEquiped;
            if (typeOfItem.Equals(GameUtils.TypeOfItem.THROWEABLE))
            {
                SetLayerRecursively(gameObject, isEquiped ? LayerMask.NameToLayer("Throw") : LayerMask.NameToLayer("Pick"));
            }
            else if (typeOfItem.Equals(GameUtils.TypeOfItem.GUN))
            {
                SetLayerRecursively(gameObject, isEquiped ? LayerMask.NameToLayer("Weapon") : LayerMask.NameToLayer("Pick"));
            }

        }
    }
}
