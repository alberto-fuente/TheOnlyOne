using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[RequireComponent(typeof(PlayerMove))]
[RequireComponent(typeof(PlayerLook))]
public class PlayerController : MonoBehaviour
{
    
    [Header("References")]
    [SerializeField] PlayerMove playerMove;
    [SerializeField] PlayerLook playerLook;
    public ItemHolder itemHolder;
    public HealthSystem healthSystem;
    public GameObject hurtPanel;
    // Start is called before the first frame update
    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        playerLook = GetComponent<PlayerLook>();
        //inventory == FindObjectOfType(ItemHolder);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            healthSystem.HealHealth(40);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            healthSystem.HealShield(15);
        }

            /*hurtAnimator.SetInteger("health", health);

            if (hasTimePowerUp)
            {
                speed = 18;
            }
            else
            {
                speed = 10;
            }*/
        }
    
}
