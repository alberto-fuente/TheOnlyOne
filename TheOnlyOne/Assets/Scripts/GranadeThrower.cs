using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeThrower : MonoBehaviour
{
    WeaponHolder weaponHolder;
    
    // Start is called before the first frame update
    void Start()
    {
        weaponHolder = FindObjectOfType<WeaponHolder>();

    }

    // Update is called once per frame
    void Update()
    {
        ListenThrowInput();
    }

    void ListenThrowInput()
    {
        PickableItem currentGranade;
        if (Input.GetMouseButtonDown(0) && !weaponHolder.isEmpty() && weaponHolder.GetCurrentItem().typeOfItem.Equals(GameUtils.TypeOfItem.THROWEABLE))
        {
            currentGranade = weaponHolder.GetCurrentItem();
            weaponHolder.Drop(currentGranade);
            currentGranade.GetComponent<Granade>().Throw(transform.forward);
        }
    }
}
