using System;
using System.Collections;
using UnityEngine;
[RequireComponent(typeof(PlayerMove))]
[RequireComponent(typeof(PlayerLook))]
public class PlayerController : MonoBehaviour
{
    
    [Header("References")]
    public PlayerMove playerMove;
    public PlayerLook playerLook;
    public ItemHolder itemHolder;
    public HealthSystem healthSystem;
    public GameObject hurtPanel;
    public GameObject toxicFilter;
    private GameManager gameManager;
    public AudioClip toxicSound;
    private bool canPlayToxic=true;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerMove = GetComponent<PlayerMove>();
        playerLook = GetComponent<PlayerLook>();
        healthSystem.OnDead += Die;
        //inventory == FindObjectOfType(ItemHolder);
    }
    public void Die(object sender, EventArgs e)
    {
        playerLook.enabled = false;
        playerMove.enabled = false;
        itemHolder.enabled = false;
        healthSystem.enabled = false;
    }


    // Update is called once per frame
    void Update()
    {
        if(Mathf.Pow(transform.position.x, 2) + Mathf.Pow(transform.position.z, 2) > Mathf.Pow(gameManager.SafeRadius, 2))
        {
            toxicFilter.SetActive(true);
            if (canPlayToxic)
            {
                itemHolder.audioSource.PlayOneShot(toxicSound);
                canPlayToxic = false;
            }

        }
        else
        {
            toxicFilter.SetActive(false);
            canPlayToxic = true;
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
