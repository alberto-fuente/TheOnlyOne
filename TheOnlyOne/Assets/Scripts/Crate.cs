using UnityEngine;
using System.Collections;

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
    private Renderer[] crateRenderers;
    public Color closedColor = new Color(27, 175, 191);
    public Color openedColor = new Color(191, 27, 18);
    public bool canBeOpened;
    public bool hasBeenOpened;
    private Canvas labelCanvas;
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        labelCanvas = GetComponentInChildren<Canvas>();
        crateRenderers = GetRenderers();
        foreach (Renderer renderer in crateRenderers)
        {
            renderer.material.SetColor("_EmissionColor", closedColor);
        }
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    private Renderer[] GetRenderers()
    {
        Renderer[] meshes=GetComponentsInChildren<Renderer>();
        return meshes;
    }
    // Update is called once per frame
    void Update()
    {
        CheckLabelVisible();
    }
    public void Open()
    {
        StartCoroutine(spawnItems());
        foreach (Renderer renderer in crateRenderers)
        {
            renderer.material.SetColor("_EmissionColor", openedColor);
        }
        animator.Play("Open");
        audioSource.PlayOneShot(openSound);
        smoke.Play();
        hasBeenOpened = true;
    }
    IEnumerator spawnItems()
    {
        yield return new WaitForSeconds(2f);
        foreach (Transform point in spawnPoints)
        {
            yield return new WaitForSeconds(.12f);
            GameObject item=Instantiate(gameManager.spawnableItems[Random.Range(0, gameManager.spawnableItems.Length)], point.position+new Vector3(0f,0,0), point.rotation);
           //item.GetComponentInChildren<Rigidbody>()?.AddForce(point.up*5,ForceMode.VelocityChange);

        }

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
