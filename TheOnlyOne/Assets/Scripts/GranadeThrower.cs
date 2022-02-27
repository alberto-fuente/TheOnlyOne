using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeThrower : MonoBehaviour
{
    ItemHolder weaponHolder;
    
    // Start is called before the first frame update
    void Start()
    {
        weaponHolder = FindObjectOfType<ItemHolder>();

    }

    // Update is called once per frame
    void Update()
    {
        ListenThrowInput();
    }

    void ListenThrowInput()
    {
        PickableItem currentGranade=weaponHolder.GetCurrentItem();
        if (Input.GetMouseButtonDown(0) && currentGranade != null && weaponHolder.GetCurrentItem().typeOfItem.Equals(GameUtils.TypeOfItem.THROWEABLE))
        {
            weaponHolder.DropItem(currentGranade);
            currentGranade.GetComponent<Granade>().Throw(transform.forward);
        }
    }
}
