using UnityEngine;
using System;
public class HeadShot : MonoBehaviour
{
    public GameObject OriginalHead;
    public GameObject ScatteredHead;
    public GameObject explosionVFX;
    private GameManager gameManager;
    public AudioClip explodeSound;
    public AudioSource audioSource;
    private bool hasExploded;
    private EnemyHitBox hitbox;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        hitbox = GetComponent<EnemyHitBox>();
        hitbox.healthSystem.OnDead += DoHeadShot;
    }

    // Update is called once per frame
    public void DoHeadShot(object sender, EventArgs e)
    {
        if (hitbox.healthSystem.waslastHitHead &&!hasExploded)
        {
            hasExploded = true;
            OriginalHead.SetActive(false);
            Destroy(ScatteredHead = Instantiate(ScatteredHead, OriginalHead.transform.position, OriginalHead.transform.rotation),10);
            Destroy(Instantiate(explosionVFX, OriginalHead.transform.position, OriginalHead.transform.rotation),10);
            audioSource.PlayOneShot(explodeSound);
            //GetComponent<Rigidbody>().AddExplosionForce(100, OriginalHead.transform.position,7);
            Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f);
            foreach (Collider coll in colliders)
            {
                if (coll.gameObject.CompareTag("Grab") || coll.gameObject.CompareTag("Pack"))
                {
                    continue;
                }
                else { 
                    Rigidbody rb = coll.GetComponent<Rigidbody>();
                    if (rb)
                    {
                        rb.AddForce(new Vector3(0, 50, 0), ForceMode.VelocityChange);

                    }
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.3f);
        //Physics.OverlapSphere(OriginalHead.transform.position, 0.5f);
    }

}

