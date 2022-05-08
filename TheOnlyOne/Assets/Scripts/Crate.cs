using UnityEngine;

public class Crate : MonoBehaviour
{
    private GameManager gameManager;
    //[SerializeField] private Transform plate;
    public Transform[] spawnPoints;

    //Visual
    public Animator animator;
    private AudioSource audioSource;
    public AudioClip openSound;
    public ParticleSystem smoke;
    public Color closedColor = new Color(11, 191, 188) * 10;
    public Color openedColor = new Color(191, 11, 11) * 10;
    // Start is called before the first frame update
    public bool canBeOpened;
    public bool hasBeenOpened;
    private Canvas labelCanvas;
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        labelCanvas = GetComponentInChildren<Canvas>();
        GetComponent<Renderer>().material.SetColor("_EmissionColor", closedColor);
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckLabelVisible();
    }
    public void Open()
    {
        foreach (Transform point in spawnPoints)
        {
            Instantiate(gameManager.spawnableItems[Random.Range(0, gameManager.spawnableItems.Length)], point.position, point.rotation);
        }
        GetComponent<Renderer>().material.SetColor("_EmissionColor", openedColor);
        animator.Play("Open");
        audioSource.PlayOneShot(openSound);
        smoke.Play();
        hasBeenOpened = true;
    }
    private void CheckLabelVisible()
    {
        if (!hasBeenOpened)
        {
            labelCanvas.enabled = canBeOpened;
        }
        else
        {
            labelCanvas.enabled = false;
        }
    }
}
