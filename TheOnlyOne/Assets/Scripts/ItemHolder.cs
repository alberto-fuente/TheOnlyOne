using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemHolder : MonoBehaviour
{
    [SerializeField] private const int SLOTS = 5;
    public IList<InventorySlot> inventory = new List<InventorySlot>();

    public event EventHandler<InventoryEventArgs> OnItemAdded;
    public event EventHandler<InventoryEventArgs> OnItemRemoved;
    public event EventHandler<InventoryEventArgs> OnNewItemSwitched;
    public event EventHandler<InventoryEventArgs> OnOldItemSwitched;

    public Camera playerCam;
    public GameManager gameManager;
    [SerializeField] private AudioClip pickSound;
    [SerializeField] private AudioClip dropSound;

    public int activeSlotIndex;
    public int slotsOc;
    public float pickRange = 5;
    [SerializeField] private float dropForce = 5;

    [SerializeField] private bool isChanging;
    private float changeDirection;

    private Canvas canvas;
    public bool IsChanging { get => isChanging; private set => isChanging = value; }

    private void Start()
    {
        activeSlotIndex = 0;
        if (OnNewItemSwitched != null) OnNewItemSwitched(this, new InventoryEventArgs(activeSlotIndex));
    }

    private void Update()
    {
        ListenDropInput();
        ListenPickInput();
        ListenChangeInput();
    }
    public ItemHolder()
    {
        for (int i = 0; i < SLOTS; i++)
        {
            inventory.Add(new InventorySlot(i));
        }
    }
    private InventorySlot FindAvailableSlot(PickableItem item)
    {
        foreach (InventorySlot slot in inventory)
        {
            if (slot.IsStackable(item)||slot.IsEmpty())
            {
                if (activeSlot().IsEmpty())
                {
                    if (OnOldItemSwitched != null) OnOldItemSwitched(this, new InventoryEventArgs(activeSlotIndex));
                    activeSlotIndex = slot.Id;
                    if (OnNewItemSwitched != null) OnNewItemSwitched(this, new InventoryEventArgs(activeSlotIndex));
                }
                return slot;
            }
        }

        //tirar lo que ya haya en ese slot
        while (!activeSlot().IsEmpty())
        {
            DropItem(GetCurrentItem());
        }
        return activeSlot();

    }
    public void PickItem(PickableItem item)
    {
        GetComponent<AudioSource>().PlayOneShot(pickSound, 0.3f);
        InventorySlot slot = FindAvailableSlot(item);
        slot.AddItem(item);
        //attach
        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.Euler(Vector3.zero);
        item.isEquiped = true;
        item.CheckEquiped();
        if (OnItemAdded != null) OnItemAdded(this, new InventoryEventArgs(item, slot.Id));
        item.disableItem();
        RefreshInventory();
    }
    public void DropItem(PickableItem item)
    {
        //gameManager.IsSafeToReload = false;
        if (activeSlot().RemoveItem(item))
        {
            
            item.isEquiped = false;
            item.transform.parent = null;
            item.CheckEquiped();

            if (item.typeOfItem.Equals(GameUtils.TypeOfItem.THROWEABLE))
            {
                item.gameObject.GetComponent<Granade>().granadeIdleCollider.enabled = true;
            }
            else {
                GetComponent<AudioSource>().PlayOneShot(dropSound, 0.3f);
                item.itemRigidBody.AddForce((playerCam.transform.forward + playerCam.transform.up) * dropForce, ForceMode.Impulse);
            }
            float random = UnityEngine.Random.Range(-1f, 1f);
            item.itemRigidBody.AddTorque(new Vector3(random, random, random) * 10);
            if (OnItemRemoved != null) OnItemRemoved(this, new InventoryEventArgs(item,activeSlotIndex));
            
            RefreshInventory();
        }
    }
    private void ListenPickInput()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit))
        {
            if (hit.transform.GetComponent<PickableItem>() && hit.transform.GetComponent<PickableItem>().enabled&&hit.transform.GetComponent<PickableItem>().distanceToPlayer < pickRange)
            {
                canvas = hit.transform.GetComponent<PickableItem>().LabelCanvas;
                canvas.enabled = true;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    canvas.enabled = false;
                    PickItem(hit.transform.GetComponent<PickableItem>());
                }
            }
            else if (canvas != null)
            {
                canvas.enabled = false;
            }
        }
    }

    private void ListenDropInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
             DropItem(GetCurrentItem());
        }
    }

    private void ListenChangeInput()
    {
        changeDirection = Input.GetAxisRaw("Mouse ScrollWheel");
        if (changeDirection != 0 && !isChanging)
        {
            switchItem(changeDirection);
        }
    }

    public int SlotsOccupied()
    {
    int emptySlots = 0;
    foreach (InventorySlot slot in inventory) {

    if (slot.IsEmpty()){ 
        emptySlots++;
    } 
    }
        return SLOTS - emptySlots;
    }
    public bool isEmpty()
    {
        return SlotsOccupied() <= 0;
    }
    public PickableItem GetCurrentItem()
    {
        return activeSlot().FirstItem();
    }
    public InventorySlot activeSlot()
    {
        return inventory[activeSlotIndex];
    }
    void switchItem(float changeDirection)
    {
        //gameManager.IsSafeToReload = false;
        if (OnOldItemSwitched != null) OnOldItemSwitched(this, new InventoryEventArgs(activeSlotIndex));
        IsChanging = true;
        if (!activeSlot().IsEmpty()) GetCurrentItem().disableItem();
        int nextDirection = changeDirection < 0 ? 1 : -1;
        activeSlotIndex = (activeSlotIndex + nextDirection)% SLOTS;
        if (activeSlotIndex < 0) activeSlotIndex = SLOTS-1;
        RefreshInventory();
        IsChanging = false;
        if (OnNewItemSwitched != null) OnNewItemSwitched(this, new InventoryEventArgs(activeSlotIndex));

    }
    public void RefreshInventory()
    {
        PickableItem activeItem = activeSlot().FirstItem();
        if (activeItem!=null) activeItem.EnableItem();
        //gameManager.IsSafeToReload = true;
    }
  
}






/*
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    private List<PickableItem> inventory;
    public Camera playerCam;
    public GameManager gameManager;
    [SerializeField] private AudioClip pickSound;

    [SerializeField] private const int SLOTS = 5;
    private int currentIndex;

    public float pickRange = 2;
    [SerializeField] private float dropForce = 500;

    [SerializeField] private bool isChanging;
    private float changeDirection;

    public Canvas canvas;
    public bool IsChanging { get => isChanging; private set => isChanging = value; }

    private void Start()
    {
        inventory = new List<PickableItem>(SLOTS);
        currentIndex = 0;
    }

    private void Update()
    {
        ListenPickInput();
        ListenDropInput();
        ListenChangeInput();
    }

    private void ListenPickInput()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit))
        {
            if (hit.transform.GetComponent<PickableItem>() && hit.transform.GetComponent<PickableItem>().distanceToPlayer < pickRange)
            {
                canvas = hit.transform.GetComponent<PickableItem>().labelCanvas;
                canvas.enabled = true;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    canvas.enabled = false;
                    Pick(hit.transform.GetComponent<PickableItem>());
                }
            }
            else if (canvas != null)
            {
                canvas.enabled = false;
            }
        }
    }

    private void ListenDropInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!isEmpty())
            {
                Drop(inventory[currentIndex]);
            }
        }
    }

    private void ListenChangeInput()
    {
        changeDirection = Input.GetAxisRaw("Mouse ScrollWheel");
        if (changeDirection != 0 && !isChanging)
        {
            changeItem(changeDirection);
        }
    }

    public bool isEmpty()
    {
        return inventory.Count == 0;
    }
    public int InventorySize()
    {
        return inventory.Count;
    }
    public PickableItem GetCurrentItem()
    {
        if (!isEmpty()) return inventory[currentIndex];
        return null;
    }

    void changeItem(float changeDirection)
    {
        if (inventory.Count <= 1) return;

        IsChanging = true;
        gameManager.isSafeToReload = false;
        int nextDirection = changeDirection < 0 ? -1 : 1;

        if (currentIndex == 0 && nextDirection == -1)
        {
            currentIndex = InventorySize() - 1;
        }
        else
        if (currentIndex >= InventorySize() - 1 && nextDirection == 1)
        {
            currentIndex = 0;
        }
        else currentIndex += nextDirection;

        RefreshInventory();
        IsChanging = false;

    }
    public void RefreshInventory()
    {
        foreach (PickableItem item in inventory)
        {
            item.disableItem();
        }
        
        if (InventorySize() > 0)
        {
            inventory[currentIndex].EnableItem();
        }
        gameManager.isSafeToReload = true;

    }
    void Pick(PickableItem itemToPick)
    {
        bool newItem = true;

        GetComponent<AudioSource>().PlayOneShot(pickSound, 0.3f);

        foreach (PickableItem item in inventory)
        {
            if (item.pickableData.itemID.Equals(itemToPick.pickableData.itemID) && (itemToPick.pickableData.typeOfItem.Equals(GameUtils.TypeOfItem.THROWEABLE) || itemToPick.pickableData.typeOfItem.Equals(GameUtils.TypeOfItem.CONSUMIBLE)))
            {
                item.pickableData.Add(itemToPick);
                newItem = false;
                break;
            }
        }
        if (newItem)
        {
            if (InventorySize() >= SLOTS)
            {
                if (inventory.Count == 1)
                    Drop(inventory[currentIndex]);
                else
                    Drop(inventory[currentIndex]);
            }
            inventory.Add(itemToPick);
        }

        itemToPick.transform.SetParent(transform);
        itemToPick.transform.localPosition = Vector3.zero;
        itemToPick.transform.localRotation = Quaternion.Euler(Vector3.zero);

        itemToPick.IsEquiped = true;

        RefreshInventory();
    }

    public void Drop(PickableItem item)
    {
        gameManager.isSafeToReload = false;
        inventory[currentIndex].Remove(item);
        item.IsEquiped = false;
        item.transform.SetParent(null);
        /*
        item.itemRigidBody.AddForce(playerCam.transform.forward * dropForce, ForceMode.Impulse);
        item.itemRigidBody.AddForce(playerCam.transform.up * dropForce, ForceMode.Impulse);
        float random = Random.Range(-1f, 1f);
        item.itemRigidBody.AddTorque(new Vector3(random, random, random) * 10);
        
if (inventory[currentIndex].Count == 0)
{
    inventory.Remove(inventory[currentIndex]);
    currentIndex = Mathf.Max(currentIndex - 1, 0);
}
RefreshInventory();
    }

    public void Drop(List<PickableItem> itemGroup)
{
    foreach (PickableItem item in itemGroup)
    {
        Drop(item);
    }
}
*/
