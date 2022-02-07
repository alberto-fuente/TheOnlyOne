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

    public float pickRange = 50;
    [SerializeField] private int pickSpeed=2;
    [SerializeField] private float dropForce = 500;

    private bool isChanging;
    private float changeDirection;
    

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
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, pickRange))
            {
                if (hit.transform.GetComponent<PickableItem>())
                {
                    Pick(hit.transform.GetComponent<PickableItem>());
                }
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

    }
     void Pick(PickableItem itemToPick)
    {
        bool newItem = true;

        GetComponent<AudioSource>().PlayOneShot(pickSound,0.3f);
        
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
                Drop(inventory[currentIndex]);
            }
            List < PickableItem > itemSlot=new List<PickableItem>();
            inventory.Add(itemSlot);
            itemSlot.Add(itemToPick);
        }

        itemToPick.transform.SetParent(transform);
        itemToPick.transform.localPosition = Vector3.zero;
        itemToPick.transform.localRotation = Quaternion.Euler(Vector3.zero);

        itemToPick.IsEquiped = true;

        RefreshInventory();
    }
   
     void Drop(PickableItem item)
    {
        inventory[currentIndex].Remove(item);
        item.IsEquiped = false;
        item.transform.SetParent(null);

        item.itemRigidBody.AddForce(playerCam.transform.forward * dropForce, ForceMode.Impulse);
        float random = Random.Range(-1f, 1f);
        item.itemRigidBody.AddTorque(new Vector3(random, random, random) * 10);

        if (inventory[currentIndex].Count == 0) inventory.Remove(inventory[currentIndex]);
        currentIndex=Mathf.Max(currentIndex-1,0);
        RefreshInventory();
    }

     void Drop(List<PickableItem> itemGroup)
    {
        foreach(PickableItem item in itemGroup)
        {
            Drop(item);
        }
    }






























    /*
    [SerializeField] public List<List<Pickable>> items;
    public int lenght;
    public int maxItems = 5;
    public int currentIndex;
    public Camera playerCam;
    public float pickRange = 50;
    public GameManager gameManager;
    [SerializeField] private bool isChanging = false;
    private float changeDirection;

    private void Start()
    {
        
        items = new List<List<Pickable>>();
        for (int i = 0; i < maxItems - 1; i++)
        {

        }
        currentIndex = 0;
    }

    private void Update()
    {
        lenght = items.Count;
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit))
            {

                if (hit.transform.GetComponent<Pickable>())
                {

                    Pick(hit.transform.GetComponent<Pickable>());
                }
            }
        }
    }
    public bool IsChanging()
    {
        return isChanging;
    }
    public bool isEmpty()
    {
        return items.Count == 0;
    }
    public Pickable GetCurrentItem()
    {
        return items[currentIndex][0];
    }

    public void Pick(Pickable item)
    {
        bool newItem = true;
        item.isEquiped = true;
        foreach (List<Pickable> i in items)
        {
            //if(i.Count==0)
            if (i[0].itemID.Equals(item.itemID))
            {
                i.Add(item);
                newItem = false;
                break;
            }
            
        }
        if (newItem)
        {
            if (items.Count >= maxItems)
            {
                Drop(items[currentIndex]);
            }
            items[1].Add(item);
        }

        item.transform.SetParent(transform);
        item.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
    public void Drop(List<Pickable> items)
    {
        foreach(Pickable i in items)
        {
            i.isEquiped = false;
            i.transform.SetParent(null);
        }
        items.Clear();
    }
    */



































    /*
    public Pickable[] items;
    public int lenght;
    public int maxItems=5;
    public int currentIndex;
    public Camera playerCam;
    public float pickRange = 50;
    public GameManager gameManager;
    [SerializeField] private bool isChanging = false;
    private float changeDirection;

    void Awake()
    {
        items = new Pickable[maxItems];
        currentIndex = 0;

        for (int i = 0; i < transform.childCount; i++)
        {
            items[i] = transform.GetChild(i).GetComponent<Pickable>();
        }
        gameManager = FindObjectOfType<GameManager>();
        
    }
    public Pickable GetCurrentItem()
    {
        return items[currentIndex];
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit))
            {

                if (hit.transform.GetComponent<Pickable>())
                {

                    Pick(hit.transform.GetComponent<Pickable>());
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (items[currentIndex] != null)
            {
                Drop(items[currentIndex]);
            }
        }
        if (!isEmpty() && GetCurrentItem().typeOfItem == Pickable.TypeOfItem.GUN)
        {
            gameManager.ammoPanel.SetActive(true);
            gameManager.currentAmmoText.text = GetCurrentItem().GetComponent<Weapon>().ws.currentAmmo.ToString();
            gameManager.totalAmmoText.text = GetCurrentItem().GetComponent<Weapon>().ws.totalAmmo.ToString();
        }
        else
        {
            gameManager.ammoPanel.SetActive(false);
        }
        
        
        changeDirection = Input.GetAxisRaw("Mouse ScrollWheel");
        if (changeDirection != 0 && !IsChanging())
        {

            isChanging = true;
            changeItem(changeDirection);

        }
    }
    public bool isEmpty()
    {
        return transform.childCount == 0;
    }
    private void changeItem(float changeDirection)
    {

        if (transform.childCount <= 1) return;
        items[currentIndex].disableItem();

        int next=changeDirection<0?-1:1;

        if(currentIndex==0&& next == -1)
        {
            currentIndex = transform.childCount - 1;
        }else
        if (currentIndex >= transform.childCount - 1&& next == 1)
        {
            currentIndex =0;
        }
        else currentIndex += next;

        items[currentIndex].EnableItem();
        isChanging = false;
        //recorremos el array
        for (int i = 0; i < transform.childCount; i++)
        {
            items[i] = transform.GetChild(i).GetComponent<Pickable>();
        }
    }
    public bool IsChanging()
    {
        return isChanging;
    }

    public void Pick(Pickable item)
    {
        item.transform.SetParent(transform);
        item.transform.localRotation = Quaternion.Euler(Vector3.zero);

        item.isEquiped = true;

        if (transform.childCount < maxItems)
        {
            items[transform.childCount-1] = item;
        }
        else
        {
            Drop(items[currentIndex]);
            items[currentIndex] = item;
            item.transform.SetSiblingIndex(currentIndex);
        }
        
    }
    public void Drop(Pickable item)
    {
        item.transform.SetParent(null);
        item.isEquiped = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        //ver la direccion de las balas en el editor
        Debug.DrawRay(playerCam.transform.position, playerCam.transform.forward);
    }*/
}
