using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeThrower : MonoBehaviour
{
    ItemHolder weaponHolder;
    public ProjectPath projectPath;
    public Transform shotPoint;

    private GrabbableItem currentItem;
    // Start is called before the first frame update
    void Start()
    {
        weaponHolder = FindObjectOfType<ItemHolder>();
    }

    // Update is called once per frame
    void Update()
    {
        ListenThrowInput();
        //ListenAimInput();
    }
    private void FixedUpdate()
    {
       
    }

    private void ListenAimInput()
    {
        if (currentItemIsThroweable())
        {
            if (Input.GetMouseButton(1))
            {
                projectPath.SimulateProjection(shotPoint);

            }
            if (Input.GetMouseButtonUp(1))
            {
                projectPath.StopSimulateProjection();
            }
        }
    }

    void ListenThrowInput()
    {
        if (Input.GetMouseButtonDown(0)&&currentItemIsThroweable())
        {
            currentItem.GetComponent<Granade>().hasBeenthrown = true;
            weaponHolder.DropItem(currentItem);
            currentItem.GetComponent<Granade>().Throw(shotPoint.position,shotPoint.forward);
            //projectPath.StopSimulateProjection();
        }
    }
    private bool currentItemIsThroweable()
    {
        currentItem = weaponHolder.GetCurrentItem();
        if (currentItem != null && weaponHolder.GetCurrentItem().typeOfItem.Equals(GameUtils.TypeOfItem.THROWEABLE))
            return true;
        return false;
    }
}
