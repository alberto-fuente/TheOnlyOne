using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //atributos propios del arma
    public WeaponBlueprint weaponData;
    public ItemRarityBlueprint rarityData;
    //atributos comunes de todas las armas
    public Transform cameraHolder;
    public Camera weaponCam;
    public Camera playerCam;
    public PlayerMove playerMove;
    public AudioSource audioSource;
    private Recoil recoilScript;
    private VisualRecoil viusalRecoilScript;
    private ItemHolder weaponChanger;
    private GameManager gameManager;
    public int currentAmmo;
    public int totalAmmo;
    private float nextTimeToFire = 0f;
    public bool isReloading = false;
    public bool isAming;
    Transform anchor;
    Transform hipState;
    Transform aimState;
    public Transform prefabContainer;
    public ParticleSystem muzzleFlash;
    //public bool isSafe;
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        cameraHolder = GameObject.Find("/CameraHolder").transform;
        playerCam = GameObject.Find("/CameraHolder/CameraRecoil/MainCamera").GetComponent<Camera>();
        weaponCam = GameObject.Find("/CameraHolder/CameraRecoil/WeaponCamera").GetComponent<Camera>();
        weaponChanger = FindObjectOfType<ItemHolder>();
        audioSource = GetComponent<AudioSource>();
        recoilScript = FindObjectOfType<Recoil>();
        playerMove = FindObjectOfType<PlayerMove>();
        viusalRecoilScript = GetComponent<VisualRecoil>();
        anchor = transform.Find("Anchor");
        hipState = transform.Find("States/Hip");
        aimState = transform.Find("States/Aim");
        prefabContainer = transform.Find("Anchor/Design");
        GenerateWeapon();
        Transform[] weaponChildren = GetComponentsInChildren<Transform>();
        foreach(Transform child in weaponChildren)
        {
            if(child.name.Equals("MuzzleFlash")) muzzleFlash=child.GetComponent<ParticleSystem>();
        }
    }
    void Start()
    {
        currentAmmo = weaponData.maxClipAmmo;
        totalAmmo = 90;
        weaponChanger.OnItemRemoved += CutReload;
        weaponChanger.OnNewItemSwitched += CutReload;
        //Equip
        //prefab = Instantiate(ws.prefab, prefabContainer.position, prefabContainer.rotation, prefabContainer);
        // muzzleFlash = prefab.GetComponentInChildren<ParticleSystem>();
    }

    private void GenerateWeapon()
    {

        switch (weaponData.weaponID)
        {
            case 0: //Pistol
                rarityData = GetRarityWeapon(gameManager.rarityDataPistols);
                break;
            case 1: //Subfusil
                rarityData = GetRarityWeapon(gameManager.rarityDataSubfusils);
                break;
            case 2: //Rifle
                rarityData = GetRarityWeapon(gameManager.rarityDataRifles);
                break;

        }
        Instantiate(rarityData.prefab, prefabContainer);
    }
    ItemRarityBlueprint GetRarityWeapon(ItemRarityBlueprint[] collection)
    {
        int i = Random.Range(0, 100);
        for (int j = 0; j < collection.Length; j++)
        {
            if (i >= collection[j].Minprobabilty && i <= collection[j].Maxprobabilty)
            {
                return collection[j];
            }
        }
        return collection[0];//si hay algún error genera un arma común
    }
    private void OnEnable()
    {
        weaponChanger.OnItemRemoved += CutReload;
        weaponChanger.OnNewItemSwitched += CutReload;
        gameManager.hudCrosshair.sprite = weaponData.crosshair;
    }
    private void OnDisable()
    {
        weaponChanger.OnItemRemoved -= CutReload;
        weaponChanger.OnNewItemSwitched -= CutReload;
        if(gameManager.hudCrosshair.sprite!=null)
            gameManager.hudCrosshair.sprite = null;
    }
    private void FixedUpdate()
    {
        anchor.rotation = cameraHolder.rotation;
    }
    void Update()
    {
        
        nextTimeToFire += Time.deltaTime;
        Sway();
        ListenReloadInput();
        ListenAimInput();
        ListenShootInput();
        
    }
    public void ListenReloadInput()
    {
        if ((Input.GetKeyDown(KeyCode.R) || currentAmmo <= 0) && CanReload())
        {
            StartCoroutine("Reload");
        }
    }
    bool CanReload() {
        return !isReloading && currentAmmo < weaponData.maxClipAmmo && totalAmmo > 0;//&& gameManager.IsSafeToReload; 
    }
    public void CutReload(object sender, InventoryEventArgs e)
    {
        if(audioSource!=null)
        audioSource.Stop();
        if(gameObject!=null)
        StopCoroutine("Reload");
        isReloading = false;

    }
    public IEnumerator Reload()
    {
        isReloading = true;
        if (weaponData.anim != null) weaponData.anim.SetTrigger("Reload");
        audioSource.pitch = 1;
        audioSource.PlayOneShot(weaponData.reloadSound, 0.2f);
        yield return new WaitForSeconds(weaponData.reloadTime);
        if (totalAmmo + currentAmmo < weaponData.maxClipAmmo)
        {
            currentAmmo += totalAmmo;
            totalAmmo = 0;
        }
        else
        {
            totalAmmo -= weaponData.maxClipAmmo - currentAmmo;
            currentAmmo = weaponData.maxClipAmmo;
        }
        isReloading = false;
        
    }
    private void ListenAimInput()
    {
        isAming = Input.GetMouseButton(1);
        Aim(isAming);
    }

    private void ListenShootInput()
    {

        if (weaponData.autoShoot)
        {
            if (Input.GetMouseButton(0) && CanShoot())
            {
                Shoot();
            }
        }
        else
        if (Input.GetMouseButtonDown(0) && CanShoot())
        {
            Shoot();
        }
    }
    private bool CanShoot()
    {
        return !weaponChanger.IsChanging && !isReloading && nextTimeToFire > weaponData.fireRate && currentAmmo > 0;
    }
    private void Shoot()
    {

        if (weaponData.anim != null) weaponData.anim.SetTrigger("Shoot");
        audioSource.pitch = Random.Range(weaponData.pitch - weaponData.pitchRand, weaponData.pitch + weaponData.pitchRand);
        audioSource.PlayOneShot(weaponData.shootSound, 0.3f);
        if(muzzleFlash!=null) muzzleFlash.Play();

        //recoil
        if (isAming)
        {
            recoilScript.RecoilFire(weaponData.aimRecoilRotation);
            viusalRecoilScript.VisualRecoilFire(weaponData.vRecoilRotationAim, weaponData.vRecoilKickBackAim);
        }
        else
        {
            recoilScript.RecoilFire(weaponData.recoilRotation);
            viusalRecoilScript.VisualRecoilFire(weaponData.vRecoilRotation, weaponData.vRecoilKickBack);
        }


        RaycastHit hit;
        if (Physics.Raycast(weaponCam.transform.position, weaponCam.transform.forward, out hit, weaponData.range))
        {
            GameObject decal = Instantiate(weaponData.bulletDecal, hit.point + (hit.normal * 0.025f), Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;//se instancia el decal
                                                                                                                                                               //Se rota el decal para adaptarse a la superficie
            decal.transform.parent = hit.transform;//el decal se "pega" al objeto con el que impacte
            Destroy(decal, 10f);//Se destruye el decal a los 10 segundos
        }
        currentAmmo--;
        nextTimeToFire = 0;
        if (hit.transform != null)
        {
            CheckEnemyHit(hit);
            //Destroy(hit.transform.gameObject);
            //FindObjectOfType<GameManager>().remainingTargets--;
        }

    }
    private bool CheckEnemyHit(RaycastHit hit)
    {
        HealthSystem enemyHealthSystem=null;
        if (hit.transform.CompareTag("Enemy"))
            enemyHealthSystem = hit.transform.gameObject.GetComponentInParent<HealthSystem>();
        if (enemyHealthSystem != null)
        {
            float damage = weaponData.damage * rarityData.multiplier;
            enemyHealthSystem.Damage((int)damage);
            return true;
        }
        return false;
    }
    private void Aim(bool aiming)
    {

        if (aiming)
        {

            anchor.position = Vector3.Lerp(anchor.position, aimState.position, Time.deltaTime * weaponData.aimSpeed);
            playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, weaponData.aimFOV, weaponData.aimSpeed * Time.deltaTime);
        }
        else
        {

            anchor.position = Vector3.Lerp(anchor.position, hipState.position, Time.deltaTime * weaponData.aimSpeed);
            playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, weaponData.mainFOV, weaponData.aimSpeed * Time.deltaTime);

        }
    }

    public void Sway()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Quaternion xSway = Quaternion.AngleAxis(weaponData.swayIntensity * -mouseX, Vector3.up);//horizontal sway
        Quaternion ySway = Quaternion.AngleAxis(weaponData.swayIntensity * mouseY, Vector3.right);//vertical sway
        Quaternion target_rotation = xSway * ySway;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, target_rotation, Time.deltaTime * weaponData.swaySpeed);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //ver la direccion de las balas en el editor
        Debug.DrawRay(weaponCam.transform.position, weaponCam.transform.forward);
    }

}
