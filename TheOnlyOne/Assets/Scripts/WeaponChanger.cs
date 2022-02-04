using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChanger : MonoBehaviour
{
    public Weapon[] weapons;
    private Weapon activeWeapon;
    private static int index=0;
    public GameManager gm;

    [SerializeField] private bool isChanging = false;
    [SerializeField] private float changeRate = .5f;
    private float timer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        activeWeapon = weapons[0];
        gm = FindObjectOfType<GameManager>();
        
    }
    public Weapon GetWeapon()
    {
        return activeWeapon;
    }
    // Update is called once per frame
    void Update()
    {
        //recorremos la lista
        for (int i = 0; i < transform.childCount; i++)
        {
            weapons[i] = transform.GetChild(i).GetComponent<Weapon>();
        }
        //provisional
        if (Time.time >= timer)
        {
            timer = Time.time + changeRate;
            isChanging = false;
        }
        gm.currentAmmoText.text = activeWeapon.ws.currentAmmo.ToString();
        gm.totalAmmoText.text = activeWeapon.ws.totalAmmo.ToString();

        if (Input.GetAxisRaw("Mouse ScrollWheel") != 0 && !activeWeapon.isReloading && !IsChanging())
        {

            isChanging = true;
            changeWeapon(activeWeapon);

        }
    }
    private void changeWeapon(Weapon previousWeapon)
    {
        if (index < weapons.Length-1)
        {
            index++;
        }
        else index = 0;

        previousWeapon.Enable(false);
        activeWeapon = weapons[index];
        activeWeapon.Enable(true);
    }
    public bool IsChanging()
    {
        return isChanging;
    }
}
