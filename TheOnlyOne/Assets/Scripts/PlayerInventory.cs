using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    //events
    public event EventHandler<InventoryEventArgs> OnItemAdded;
    public event EventHandler<InventoryEventArgs> OnItemRemoved;
    public event EventHandler<InventoryEventArgs> OnNewItemSwitched;
    public event EventHandler<InventoryEventArgs> OnOldItemSwitched;

    [Header("Porperties")]
    //inventory is a list of slots
    public IList<InventorySlot> inventory = new List<InventorySlot>();
    private const int SLOTS = 5;
    private float interactRange = 5;
    private float dropForce = 5;
    public int activeSlotIndex;
    private bool isChanging;
    private float changeDirection;
    private LayerMask layerMask;

    [Header("References")]
    private GameObject arms;
    public Camera playerCam;
    private Animator armsAnimator;
    private GrabbableItem itemRef = null;
    private Crate crateRef = null;
    private Pack packRef = null;

    public bool IsChanging { get => isChanging; private set => isChanging = value; }

    private void Start()
    {
        arms = GetComponentInChildren<SkinnedMeshRenderer>().gameObject;
        armsAnimator = transform.Find("Arms").GetComponent<Animator>();
        activeSlotIndex = 0;
        if (OnNewItemSwitched != null) OnNewItemSwitched(this, new InventoryEventArgs(activeSlotIndex));
    }
    private void Awake()
    {
        //Avoid raycast to collide with equiped weapon
        layerMask = ~(1 << 7);
    }
    private void Update()
    {
        ListenDropInput();
        ListenPickInput();
        ListenChangeInput();
        CheckArmsVisible();
    }
    //Constructor
    public PlayerInventory()
    {
        for (int i = 0; i < SLOTS; i++)
        {
            inventory.Add(new InventorySlot(i));
        }
    }
    private InventorySlot FindAvailableSlot(GrabbableItem _item)
    {
        foreach (InventorySlot slot in inventory)
        {
            if (slot.IsStackable(_item) || slot.IsEmpty())
            {
                if (ActiveSlot().IsEmpty())
                {
                    if (OnOldItemSwitched != null) OnOldItemSwitched(this, new InventoryEventArgs(activeSlotIndex));
                    activeSlotIndex = slot.Id;
                    if (OnNewItemSwitched != null) OnNewItemSwitched(this, new InventoryEventArgs(activeSlotIndex));
                }
                return slot;
            }
        }
        //if there is no slot available, all items of activeSlot are dropped to use it
        while (!ActiveSlot().IsEmpty())
        {
            DropItem();
        }
        return ActiveSlot();
    }
    public void PickItem(GrabbableItem _item)
    {
        InventorySlot slot = FindAvailableSlot(_item);
        slot.Push(_item);
        //attach item to inventory
        _item.transform.SetParent(transform);
        _item.transform.localPosition = Vector3.zero;
        _item.transform.localRotation = Quaternion.Euler(Vector3.zero);


        _item.CheckEquiped();
        if (OnItemAdded != null) OnItemAdded(this, new InventoryEventArgs(_item, slot.Id));
        _item.DisableItem();
        RefreshInventory();

    }
    public void DropItem()
    {
        GrabbableItem item = ActiveSlot().Peek();
        if (ActiveSlot().Pop())
        {
            item.IsEquiped = false;
            item.transform.parent = null;
            item.CheckEquiped();
            if (item.typeOfItem.Equals(GameUtils.TypeOfItem.THROWEABLE))
            {
                item.gameObject.GetComponent<Granade>().GranadeIdleCollider.enabled = true;
            }
            else
            {
                item.ItemRigidBody.AddForce((playerCam.transform.forward + playerCam.transform.up) * dropForce, ForceMode.Impulse);
            }
            float random = UnityEngine.Random.Range(-1f, 1f);
            item.ItemRigidBody.AddTorque(new Vector3(random, random, random) * 10);
            if (OnItemRemoved != null) OnItemRemoved(this, new InventoryEventArgs(item, activeSlotIndex));
            RefreshInventory();
        }
    }
    private void ListenPickInput()
    {
        if (itemRef != null) itemRef.Label.IsPointed = false;
        if (packRef != null) packRef.Label.IsPointed = false;
        if (crateRef != null) crateRef.CanBeOpened = false;
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, interactRange, layerMask))
        {
            if (hit.transform.CompareTag("Grab"))
            {
                itemRef = hit.transform.gameObject.GetComponentInParent<GrabbableItem>();
                if (!itemRef.IsEquiped) itemRef.Label.IsPointed = true;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    PickItem(itemRef);
                }
            }
            else if (hit.transform.CompareTag("Pack"))
            {
                packRef = hit.transform.gameObject.GetComponent<Pack>();
                packRef.Label.IsPointed = true;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    packRef.Collect(FindObjectOfType<PlayerController>().GetComponentInChildren<Collider>(), packRef.type);
                }
            }
            else if (hit.transform.CompareTag("Crate"))
            {
                crateRef = hit.transform.gameObject.GetComponent<Crate>();
                if (!crateRef.HasBeenOpened)
                {
                    crateRef.CanBeOpened = true;
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        crateRef.Open();
                    }
                }
            }
        }
    }

    private void ListenDropInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropItem();
        }
    }

    private void ListenChangeInput()
    {
        changeDirection = Input.GetAxisRaw("Mouse ScrollWheel");
        if (changeDirection != 0 && !isChanging)
        {
            SwitchItem(changeDirection);
        }
    }

    public int SlotsOccupied()
    {
        int emptySlots = 0;
        foreach (InventorySlot slot in inventory)
        {

            if (slot.IsEmpty())
            {
                emptySlots++;
            }
        }
        return SLOTS - emptySlots;
    }
    public bool IsEmpty()
    {
        return SlotsOccupied() <= 0;
    }
    public GrabbableItem GetCurrentItem()
    {
        return ActiveSlot().Peek();
    }
    public InventorySlot ActiveSlot()
    {
        return inventory[activeSlotIndex];
    }
    void SwitchItem(float changeDirection)
    {
        if (OnOldItemSwitched != null) OnOldItemSwitched(this, new InventoryEventArgs(activeSlotIndex));
        IsChanging = true;
        if (!ActiveSlot().IsEmpty())
        {
            int itemCount = ActiveSlot().Count();
            //More than one item in the slot
            if (itemCount > 1)
            {
                for (int i = 0; i < itemCount; i++)
                {
                    ActiveSlot().GetItemAt(i).DisableItem();
                }
            }
            else//one item in teh slot
                GetCurrentItem().DisableItem();
        }
        int nextDirection = changeDirection < 0 ? 1 : -1;
        activeSlotIndex = (activeSlotIndex + nextDirection) % SLOTS;
        if (activeSlotIndex < 0) activeSlotIndex = SLOTS - 1;
        RefreshInventory();
        IsChanging = false;
        if (OnNewItemSwitched != null) OnNewItemSwitched(this, new InventoryEventArgs(activeSlotIndex));

    }
    //enables first item of the active slot
    public void RefreshInventory()
    {
        GrabbableItem activeItem = ActiveSlot().Peek();
        if (activeItem != null && !activeItem.isActiveAndEnabled)
        {
            activeItem.EnableItem();
            activeItem.IsEquiped = true;
        }
    }
    private void CheckArmsVisible()
    {
        if (ActiveSlot().IsEmpty())
        {
            if (!arms.activeInHierarchy)
            {
                arms.SetActive(true);
                armsAnimator.Play("ArmsUp");
            }
        }
        else
        {
            arms.SetActive(false);
        }
    }
}