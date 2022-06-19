using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeThrower : MonoBehaviour
{
    [Header("References")]
    private PlayerInventory weaponHolder;
    public Transform shootPoint;
    private GrabbableItem currentItem;
    void Start()
    {
        weaponHolder = FindObjectOfType<PlayerInventory>();
    }

    void Update()
    {
        ListenThrowInput();
    }

    void ListenThrowInput()
    {
        if (currentItemIsThroweable()&&Input.GetMouseButtonDown(0))
        {
            currentItem.GetComponent<Granade>().HasBeenthrown = true;
            weaponHolder.DropItem();
            currentItem.GetComponent<Granade>().Throw(shootPoint.position,shootPoint.forward);
        }
    }
    private bool currentItemIsThroweable()
    {
        currentItem = weaponHolder.GetCurrentItem();
        return (currentItem != null && weaponHolder.GetCurrentItem().typeOfItem.Equals(GameUtils.TypeOfItem.THROWEABLE));
    }
}
