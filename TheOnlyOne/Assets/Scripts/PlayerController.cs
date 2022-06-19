using System;
using UnityEngine;
[RequireComponent(typeof(PlayerMove))]
[RequireComponent(typeof(PlayerLook))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public PlayerMove playerMove;
    public PlayerLook playerLook;
    public PlayerInventory playerInventory;
    public HealthSystem healthSystem;
    public GameObject hurtPanel;
    public GameObject toxicFilter;
    private GameManager gameManager;
    private AudioManager audioManager;
    public AudioClip toxicSound;

    private bool canPlayToxic = true;

    void Start()
    {
        gameManager = GameManager.Instance;
        audioManager = AudioManager.Instance;
        playerMove = GetComponent<PlayerMove>();
        playerLook = GetComponent<PlayerLook>();
        playerInventory = FindObjectOfType<PlayerInventory>();
        healthSystem.OnDead += Die;
    }
    public void Die(object sender, EventArgs e)
    {
        playerLook.enabled = false;
        playerMove.enabled = false;
        playerInventory.enabled = false;
        healthSystem.enabled = false;
    }

    void Update()
    {
        //player out of the safe zone
        if (Mathf.Pow(transform.position.x, 2) + Mathf.Pow(transform.position.z, 2) > Mathf.Pow(gameManager.SafeRadius, 2))
        {
            toxicFilter.SetActive(true);
            if (canPlayToxic)
            {
                audioManager.PlaySound(toxicSound);
                canPlayToxic = false;
            }
        }
        else
        {
            toxicFilter.SetActive(false);
            canPlayToxic = true;
        }
    }

}
