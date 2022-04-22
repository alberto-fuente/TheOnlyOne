using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
   //[SerializeField] private Transform plate;
    public Transform[] spawnPoints;
    public GameObject[] spawnableItems;
    //Visual
    public Animator animator;
    private AudioSource audioSource;
    public AudioClip openSound;
    public ParticleSystem smoke;
    public Color closedColor=new Color(11,191,188)*10;
    public Color openedColor = new Color(191, 11, 11) * 10;
    // Start is called before the first frame update
    public bool canBeOpened;
    public bool hasBeenOpened;
    private Canvas labelCanvas;
    void Start()
    {
        labelCanvas = transform.GetComponentInChildren<Canvas>();
        labelCanvas.enabled = false;
        GetComponent<Renderer>().material.SetColor("_EmissionColor", closedColor);
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        

    }

    // Update is called once per frame
    void Update()
    {
        if (!hasBeenOpened)
        {
            if (canBeOpened)//el jugador la está mirando
            {
                labelCanvas.enabled = true;
                if (Input.GetKeyDown(KeyCode.E) && canBeOpened)
                {
                    foreach (Transform point in spawnPoints)
                    {
                        Instantiate(spawnableItems[Random.Range(0, spawnableItems.Length)], point.position, point.rotation, point);
                    }
                    GetComponent<Renderer>().material.SetColor("_EmissionColor", openedColor);
                    animator.Play("Open");
                    audioSource.PlayOneShot(openSound);
                    smoke.Play();
                    hasBeenOpened = true;
                    labelCanvas.enabled = false;

                }
            }
            else
            {
                labelCanvas.enabled = false;
            }
        }
        
    }
}
