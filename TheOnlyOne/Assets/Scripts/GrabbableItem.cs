using UnityEngine;

public class GrabbableItem : MonoBehaviour
{
    [Header("Item Components")]
    [SerializeField] private int itemID;
    private Sprite icon;
    private Label label;
    public GameUtils.TypeOfItem typeOfItem;

    [Header("References")]
    private GameManager gameManager;
    private Rigidbody itemRigidBody;
    private Collider itemCollider;
    private GameObject armMesh;

    [Header("State")]
    private bool isStackable;
    private bool isEquiped;
    private bool equipFlag;

    [Header("Inventory")]
    private InventorySlot slot;

    public int ItemID { get => itemID; set => itemID = value; }
    public bool IsStackable { get => isStackable; set => isStackable = value; }
    public bool IsEquiped { get => isEquiped; set => isEquiped = value; }
    public InventorySlot Slot { get => slot; set => slot = value; }
    public Rigidbody ItemRigidBody { get => itemRigidBody; set => itemRigidBody = value; }
    public Sprite Icon { get => icon; set => icon = value; }
    public Label Label { get => label; set => label = value; }

    void Start()
    {
        gameManager = GameManager.Instance;
        ItemRigidBody = GetComponentInChildren<Rigidbody>();
        itemCollider = GetComponentInChildren<Collider>();
        Label = GetComponentInChildren<Label>();
        armMesh = GetComponentInChildren<SkinnedMeshRenderer>().gameObject;

        if (typeOfItem.Equals(GameUtils.TypeOfItem.THROWEABLE))
        {
            ItemID = GetComponent<Granade>().GranadeData.id;
            Icon = GetComponent<Granade>().GranadeData.inventoryIcon;
            IsStackable = true;
        }
        else if (typeOfItem.Equals(GameUtils.TypeOfItem.GUN))
        {
            Icon = GetComponent<Weapon>().RarityData.inventoryIcon;
            IsStackable = false;
        }
    }
    private void Update()
    {
        CheckItemLayer();
    }
    private void LateUpdate()
    {
        CheckEquiped();
    }
    //changes item's layer depending on whether it is equipped
    public static void SetLayerRecursively(GameObject _gameObject, int _layer)
    {
        foreach (Transform transform in _gameObject.GetComponentsInChildren<Transform>(true))
        {
            if (transform.name.Equals("Label")) break;
            transform.gameObject.layer = _layer;
        }
    }
    public void EnableItem()
    {
        gameManager.HUDCrosshair.enabled = true;
        gameObject.SetActive(true);
    }
    public void DisableItem()
    {
        gameManager.HUDCrosshair.enabled=false;
        gameObject.GetComponentInChildren<Animator>().enabled = false;
        gameObject.SetActive(false);
    }
    private void CheckItemLayer()
    {
        //if it is equiped, it is rendered by weaponCam (change layer)
        if (equipFlag != IsEquiped)//just check it when is equiped/unequiped
        {
            equipFlag = IsEquiped;
            if (typeOfItem.Equals(GameUtils.TypeOfItem.THROWEABLE))
            {
                SetLayerRecursively(gameObject, IsEquiped ? LayerMask.NameToLayer("Throw") : LayerMask.NameToLayer("Pick"));
            }
            else if (typeOfItem.Equals(GameUtils.TypeOfItem.GUN))
            {
                SetLayerRecursively(gameObject, IsEquiped ? LayerMask.NameToLayer("Weapon") : LayerMask.NameToLayer("Pick"));
            }
        }
    }
    public void CheckEquiped()
    {
        ItemRigidBody.isKinematic = IsEquiped;
        itemCollider.isTrigger = IsEquiped;
        //show arms if equiped
        armMesh.SetActive(IsEquiped);

        if (typeOfItem.Equals(GameUtils.TypeOfItem.THROWEABLE))
        {
            if (!gameObject.GetComponent<Granade>().HasBeenthrown)
            {
                gameObject.GetComponent<Granade>().enabled = IsEquiped;
                gameObject.GetComponentInChildren<Animator>().enabled = IsEquiped;
            }
            else
            {
                gameObject.GetComponent<Granade>().enabled = true;
                gameObject.GetComponentInChildren<Animator>().enabled = true;
            }
        }
        else if (typeOfItem.Equals(GameUtils.TypeOfItem.GUN))
        {
            gameObject.GetComponent<Weapon>().enabled = IsEquiped;
            gameObject.GetComponent<VisualRecoil>().enabled = IsEquiped;
            gameObject.GetComponentInChildren<Animator>().enabled = IsEquiped;
        }


    }
}
