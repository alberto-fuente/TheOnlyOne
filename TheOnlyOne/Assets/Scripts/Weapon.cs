using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Special Weapon properties")]
    public WeaponBlueprint weaponData;
    public ItemRarityBlueprint rarityData;

    [Header("Shared properties")]
    [Header("Cameras")]
    public Transform cameraHolder;
    public Camera weaponCam;
    public Camera playerCam;
    public PlayerLook playerLook;

    [Header("Recoil")]
    private Recoil actualRecoil;
    private VisualRecoil viusalRecoil;

    [Header("Fire and stats")]
    public int currentAmmo;
    public int totalAmmo;
    private float nextTimeToFire;
    public bool isReloading;
    const int HEADSHOTMULTIPLIER = 2;

    [Header("Aim")]
    public bool isAming;
    private float sensMultAim = 0.8f;
    private float sensMultDefault = 2f;
    private Transform anchor;
    private Transform hipState;
    private Transform aimState;

    [Header("Other references")]
    private GameManager gameManager;
    private PlayerInventory playerInventory;
    private AudioSource audioSource;
    private Animator animator;
    private LayerMask shootLayer;
    private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject bulletHit;
    [SerializeField] private AudioClip impactSound;
    private Transform prefabContainer;
    private GameObject prefab;
    


    private void Awake()
    {
        gameManager = GameManager.Instance;
        cameraHolder = GameObject.Find("/CameraHolder").transform;
        playerCam = GameObject.Find("/CameraHolder/CameraRecoil/MainCamera").GetComponent<Camera>();
        weaponCam = GameObject.Find("/CameraHolder/CameraRecoil/WeaponCamera").GetComponent<Camera>();
        playerLook = FindObjectOfType<PlayerLook>();
        playerInventory = FindObjectOfType<PlayerInventory>();
        audioSource = GetComponent<AudioSource>();
        actualRecoil = FindObjectOfType<Recoil>();
        viusalRecoil = GetComponent<VisualRecoil>();
        shootLayer = ~(1 << 7);//all except from weapon layer
        anchor = transform.Find("Anchor");
        hipState = transform.Find("States/Hip");
        aimState = transform.Find("States/Aim");
        prefabContainer = transform.Find("Anchor/Design");
        GenerateWeapon();
        float damagePerSecond = weaponData.damage * rarityData.multiplier / weaponData.fireRate;
        prefab = GetComponentInChildren<Collider>().gameObject;
        gameManager.GenerateLabel(prefab.transform, prefab.transform.position + new Vector3(0.07f, 0.63f, 0.28f), weaponData.weaponName, rarityData.rarity, rarityData.labelIcon, ((int)damagePerSecond).ToString(), rarityData.color);
        muzzleFlash = GetComponentInChildren<ParticleSystem>();
        animator = GetComponentInChildren<Animator>();
        currentAmmo = weaponData.maxClipAmmo;
        totalAmmo = weaponData.maxClipAmmo * 4;
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
            case 3: //Sniper
                rarityData = GetRarityWeapon(gameManager.rarityDataSnipers);
                break;
            case 4: //Shotgun
                rarityData = GetRarityWeapon(gameManager.rarityDataShotguns);
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
        return collection[0];
    }
    private void OnEnable()
    {
        playerInventory.OnItemRemoved += CutReload;
        playerInventory.OnNewItemSwitched += CutReload;
        gameManager.HUDCrosshair.sprite = weaponData.crosshair;
        animator.Play("WeaponUp");
    }
    private void OnDisable()
    {
        playerInventory.OnItemRemoved -= CutReload;
        playerInventory.OnOldItemSwitched -= CutReload;
        if (gameManager.HUDCrosshair != null) gameManager.HUDCrosshair.sprite = null;
    }
    private void FixedUpdate()
    {
        anchor.rotation = cameraHolder.rotation;

    }
    void Update()
    {
        nextTimeToFire += Time.deltaTime;
        if (playerInventory.isActiveAndEnabled)
        {
            ListenAimInput();
            ListenReloadInput();
            ListenShootInput();
            Sway();
        }
    }
    public void ListenReloadInput()
    {
        if ((Input.GetKeyDown(KeyCode.R) || currentAmmo <= 0) && CanReload())
        {
            StartCoroutine("Reload");
        }
    }
    bool CanReload()
    {
        return !isReloading && currentAmmo < weaponData.maxClipAmmo && totalAmmo > 0;
    }
    public void CutReload(object sender, InventoryEventArgs e)
    {
        if (audioSource != null)
            audioSource.Stop();
        if (gameObject != null)
            StopCoroutine("Reload");
        isReloading = false;

    }
    public IEnumerator Reload()
    {
        isReloading = true;
        animator.SetTrigger("Reload");
        audioSource.pitch = 1;
        audioSource.PlayOneShot(weaponData.reloadSound, 1f);
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
        return !playerInventory.IsChanging && !isReloading && nextTimeToFire > weaponData.fireRate && currentAmmo > 0;
    }
    private void Shoot()
    {
        audioSource.pitch = Random.Range(weaponData.pitch - weaponData.pitchRand, weaponData.pitch + weaponData.pitchRand);
        audioSource.PlayOneShot(weaponData.shootSound, 0.3f);
        if (muzzleFlash!=null) muzzleFlash.Play();
        //recoil
        if (isAming)
        {
            actualRecoil.RecoilFire(weaponData.aimRecoilRotation);
            viusalRecoil.VisualRecoilFire(weaponData.vRecoilRotationAim, weaponData.vRecoilKickBackAim);
        }
        else
        {
            actualRecoil.RecoilFire(weaponData.recoilRotation);
            viusalRecoil.VisualRecoilFire(weaponData.vRecoilRotation, weaponData.vRecoilKickBack);
        }
        currentAmmo--;
        nextTimeToFire = 0;

        RaycastHit hit;
        Physics.Raycast(weaponCam.transform.position, weaponCam.transform.forward, out hit, weaponData.range, shootLayer);

        if (hit.transform)
        {
            CheckEnemyHit(hit);
        }

    }
    private bool CheckEnemyHit(RaycastHit hit)
    {
        GameObject bulletParticles = Instantiate(bulletHit, hit.point + hit.normal * 0.001f, Quaternion.identity);
        bulletParticles.transform.LookAt(hit.point + hit.normal);
        Destroy(bulletParticles, 2);
        //enemy hit
        var hitBox = hit.collider.GetComponent<EnemyHitBox>();
        if (hitBox != null)
        {
            bulletParticles.transform.localScale *= 2;
            hitBox.HealthSystem.waslastHitHead = false;//reset variable
            if (hitBox.CompareTag("Head"))//Headshot
            {
                hitBox.HealthSystem.waslastHitHead = true;
                hitBox.OnHit(weaponData.damage * rarityData.multiplier * HEADSHOTMULTIPLIER, transform);
                audioSource.PlayOneShot(gameManager.headshotSound, 0.2f);
            }
            else
            {
                hitBox.OnHit(weaponData.damage * rarityData.multiplier, transform);
                audioSource.pitch = Random.Range(0.95f, 1.05f);
                audioSource.PlayOneShot(impactSound, 0.5f);
                audioSource.pitch = 1;//reset pitch
            }
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
            gameManager.HUDCrosshair.rectTransform.localScale = new Vector3(weaponData.crosshairSizeAim, weaponData.crosshairSizeAim, weaponData.crosshairSizeAim);
            playerLook.sensMult = sensMultAim;//reduce sensitivity when aiming
        }
        else
        {
            anchor.position = Vector3.Lerp(anchor.position, hipState.position, Time.deltaTime * weaponData.aimSpeed);
            playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, weaponData.mainFOV, weaponData.aimSpeed * Time.deltaTime);
            gameManager.HUDCrosshair.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);//default size of the crosshair
            playerLook.sensMult = sensMultDefault;
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
}
