using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceRecoil : MonoBehaviour
{
    public Camera cam;
    public float shootRate = 1f;
    private float nextTimeToShoot = 0f;
    public int range = 5;
    public int radius = 2;
    public float force = 100f;
    public GameObject player;
    public AudioSource audioSource;
    public AudioClip shootClip;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0) && Time.time >= nextTimeToShoot)
        {
            nextTimeToShoot = Time.time + shootRate;
            Shoot();
            
        }
    }
    void Shoot()
    {
        RaycastHit hit;
        Rigidbody rb;
        audioSource.PlayOneShot(shootClip);
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            rb = hit.transform.gameObject.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(cam.transform.forward * 50, ForceMode.Impulse);
            }
            player.GetComponent<Rigidbody>().AddForce(-cam.transform.forward * force, ForceMode.Impulse);
        }
    }
}
