using System;
using UnityEngine;
public class HeadShot : MonoBehaviour
{
    [Header("Components")]
    public GameObject OriginalHead;
    public GameObject ScatteredHead;
    public GameObject explosionVFX;
    public AudioClip explodeSound;
    [Space]
    private bool hasExploded;

    [Header("References")]
    private AudioSource audioSource;
    private EnemyHitBox hitbox;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        hitbox = GetComponent<EnemyHitBox>();
        hitbox.HealthSystem.OnDead += DoHeadShot;
    }

    public void DoHeadShot(object sender, EventArgs e)
    {
        if (hitbox.HealthSystem.WaslastHitHead && !hasExploded)
        {
            hasExploded = true;
            OriginalHead.SetActive(false);
            Destroy(ScatteredHead = Instantiate(ScatteredHead, OriginalHead.transform.position, OriginalHead.transform.rotation), 10);
            Destroy(Instantiate(explosionVFX, OriginalHead.transform.position, OriginalHead.transform.rotation), 10);
            audioSource.PlayOneShot(explodeSound, 0.2f);
            Collider[] colliders = Physics.OverlapSphere(transform.position, 0.5f);
            foreach (Collider coll in colliders)
            {

                Rigidbody rigidBody = coll.GetComponent<Rigidbody>();
                if (rigidBody != null)
                {
                    rigidBody.AddForce(new Vector3(0, 55, 0), ForceMode.VelocityChange);
                }

            }
        }
    }
}


