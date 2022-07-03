using System.Collections;
using UnityEngine;

public class Crate : MonoBehaviour
{
    [Header("Components")]
    public Transform[] spawnPoints;
    public Animator animator;
    private AudioSource audioSource;
    public AudioClip openSound;
    public ParticleSystem smoke;
    private Renderer[] crateRenderers;
    private Canvas labelCanvas;
    public Color closedColor = new Color(27, 175, 191);
    public Color openedColor = new Color(191, 27, 18);
    private bool canBeOpened;
    private bool hasBeenOpened;
    public bool CanBeOpened { get => canBeOpened; set => canBeOpened = value; }
    public bool HasBeenOpened { get => hasBeenOpened; set => hasBeenOpened = value; }

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        labelCanvas = GetComponentInChildren<Canvas>();
        crateRenderers = GetRenderers();
        foreach (Renderer renderer in crateRenderers)
        {
            renderer.material.SetColor("_EmissionColor", closedColor);
        }
    }
    private Renderer[] GetRenderers()
    {
        Renderer[] meshes = GetComponentsInChildren<Renderer>();
        return meshes;
    }
    void Update()
    {
        CheckLabelVisible();
    }
    public void Open()
    {
        HasBeenOpened = true;
        StartCoroutine(spawnItems());
        foreach (Renderer renderer in crateRenderers)
        {
            renderer.material.SetColor("_EmissionColor", openedColor);
        }
        animator.Play("Open");
        audioSource.PlayOneShot(openSound);
        smoke.Play();
    }
    IEnumerator spawnItems()
    {
        yield return new WaitForSeconds(2.2f);
        foreach (Transform point in spawnPoints)
        {
            yield return new WaitForSeconds(.12f);
            GameObject item = Instantiate(GameManager.Instance.spawnableItems[Random.Range(0, GameManager.Instance.spawnableItems.Length)], point.position, point.rotation);

        }
    }
    private void CheckLabelVisible()
    {
        if (HasBeenOpened)
        {
            labelCanvas.enabled = false;
        }
        else
        {
            labelCanvas.enabled = CanBeOpened;
        }
    }
}
