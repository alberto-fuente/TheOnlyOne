using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    private List<List<PickableItem>> inventory;
    public Camera playerCam;
    public GameManager gameManager;
    [SerializeField] private AudioClip pickSound;

    [SerializeField] private int maxItems = 5;
    private int currentIndex;

    public float pickRange = 2;
    [SerializeField] private float dropForce = 500;

    [SerializeField] private bool isChanging;
    private float changeDirection;

    public Canvas canvas;
    public bool IsChanging { get => isChanging; private set => isChanging = value; }

    private void Start()
    {
        inventory = new List<List<PickableItem>>();
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
                Drop(inventory[currentIndex][0]);
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
        if (!isEmpty()) return inventory[currentIndex][0];
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
        foreach (List<PickableItem> itemSlot in inventory)
        {
            foreach (PickableItem item in itemSlot)
            {
                item.disableItem();
            }
        }
        if (InventorySize() > 0)
        {
            inventory[currentIndex][0].EnableItem();
        }
        gameManager.isSafeToReload = true;
 
    }
    void Pick(PickableItem itemToPick)
    {
        bool newItem = true;

        GetComponent<AudioSource>().PlayOneShot(pickSound, 0.3f);

        foreach (List<PickableItem> itemSlot in inventory)
        {
            if (itemSlot[0].itemID.Equals(itemToPick.itemID) && (itemToPick.typeOfItem.Equals(GameUtils.TypeOfItem.THROWEABLE) || itemToPick.typeOfItem.Equals(GameUtils.TypeOfItem.CONSUMIBLE)))
            {
                itemSlot.Add(itemToPick);
                newItem = false;
                break;
            }
        }
        if (newItem)
        {
            if (InventorySize() >= maxItems)
            {
                if(inventory[currentIndex].Count==1)
                Drop(inventory[currentIndex][0]);
                else
                Drop(inventory[currentIndex]);
            }
            List<PickableItem> itemSlot = new List<PickableItem>();
            inventory.Add(itemSlot);
            itemSlot.Add(itemToPick);
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
        */
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
}


