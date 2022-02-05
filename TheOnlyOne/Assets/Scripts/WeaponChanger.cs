using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChanger : MonoBehaviour
{
    public Pickable[] items;
    public int lenght;
    public int maxItems=5;
    public int currentIndex;
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
      


        if (GetCurrentItem().typeOfItem == Pickable.TypeOfItem.GUN)
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
        if ((changeDirection != 0|| GetCurrentItem() == null) && !IsChanging())
        {

            isChanging = true;
            changeItem(changeDirection);

        }
    }
    private void changeItem(float changeDirection)
    {

        if (transform.childCount == 1) return;
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

    public void Pick(int index)
    {

    }
    public void Drop(int index)
    {

    }
}
