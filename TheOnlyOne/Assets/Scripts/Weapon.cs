using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon properties")]
    public WeaponBlueprint weaponData;
    private ItemRarityBlueprint rarityData;

    [Header("Shared properties")]
    [Header("Cameras")]
    private Transform cameraHolder;
    private Camera weaponCam;
    private Camera playerCam;
    private PlayerLook playerLook;

    [Header("Recoil")]
    private Recoil actualRecoil;
    private VisualRecoil viusalRecoil;

    [Header("Fire and stats")]
    private int currentAmmo;
    private int totalAmmo;
    private float nextTimeToFire;
    private bool isReloading;

    [Header("Aim")]
    private bool isAming;
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
    private Transform prefabContainer;
    private GameObject prefab;

    public ItemRarityBlueprint RarityData { get => rarityData; set => rarityData = value; }
    public Transform CameraHolder { get => cameraHolder; set => cameraHolder = value; }
    public Camera WeaponCam { get => weaponCam; set => weaponCam = value; }
    public Camera PlayerCam { get => playerCam; set => playerCam = value; }
    public PlayerLook PlayerLook { get => playerLook; set => playerLook = value; }
    public int CurrentAmmo { get => currentAmmo; set => currentAmmo = value; }
    public int TotalAmmo { get => totalAmmo; set => totalAmmo = value; }
    public bool IsReloading { get => isReloading; set => isReloading = value; }
    public bool IsAming { get => isAming; set => isAming = value; }

    private void Awake()
    {
        gameManager = GameManager.Instance;
        CameraHolder = GameObject.Find("/CameraHolder").transform;
        PlayerCam = GameObject.Find("/CameraHolder/CameraRecoil/MainCamera").GetComponent<Camera>();
        WeaponCam = GameObject.Find("/CameraHolder/CameraRecoil/WeaponCamera").GetComponent<Camera>();
        PlayerLook = FindObjectOfType<PlayerLook>();
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
        float damagePerSecond = weaponData.damage * RarityData.multiplier / weaponData.fireRate;
        prefab = GetComponentInChildren<Collider>().gameObject;
        gameManager.GenerateLabel(prefab.transform, prefab.transform.position + new Vector3(0.07f, 0.63f, 0.28f), weaponData.weaponName, RarityData.rarity, RarityData.labelIcon, ((int)damagePerSecond).ToString(), RarityData.color);
        muzzleFlash = GetComponentInChildren<ParticleSystem>();
        animator = GetComponentInChildren<Animator>();
        CurrentAmmo = weaponData.maxClipAmmo;
        TotalAmmo = weaponData.maxClipAmmo * 4;
    }

    private void GenerateWeapon()
    {

        switch (weaponData.weaponID)
        {
            case 0: //Pistol
                RarityData = GetRarityWeapon(gameManager.rarityDataPistols);
                break;
            case 1: //Subfusil
                RarityData = GetRarityWeapon(gameManager.rarityDataSubfusils);
                break;
            case 2: //Rifle
                RarityData = GetRarityWeapon(gameManager.rarityDataRifles);
                break;
            case 3: //Sniper
                RarityData = GetRarityWeapon(gameManager.rarityDataSnipers);
                break;
            case 4: //Shotgun
                RarityData = GetRarityWeapon(gameManager.rarityDataShotguns);
                break;
        }
        Instantiate(RarityData.prefab, prefabContainer);
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
        anchor.rotation = CameraHolder.rotation;

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
        if ((Input.GetKeyDown(KeyCode.R) || CurrentAmmo <= 0) && CanReload())
        {
            StartCoroutine("Reload");
        }
    }
    bool CanReload()
    {
        return !IsReloading && CurrentAmmo < weaponData.maxClipAmmo && TotalAmmo > 0;
    }
    public void CutReload(object sender, InventoryEventArgs e)
    {
        if (audioSource != null)
            audioSource.Stop();
        if (gameObject != null)
            StopCoroutine("Reload");
        IsReloading = false;

    }
    public IEnumerator Reload()
    {
        IsReloading = true;
        animator.SetTrigger("Reload");
        audioSource.pitch = 1;
        audioSource.PlayOneShot(weaponData.reloadSound, 1f);
        yield return new WaitForSeconds(weaponData.reloadTime);
        if (TotalAmmo + CurrentAmmo < weaponData.maxClipAmmo)
        {
            CurrentAmmo += TotalAmmo;
            TotalAmmo = 0;
        }
        else
        {
            TotalAmmo -= weaponData.maxClipAmmo - CurrentAmmo;
            CurrentAmmo = weaponData.maxClipAmmo;
        }
        IsReloading = false;

    }
    private void ListenAimInput()
    {
        IsAming = Input.GetMouseButton(1);
        Aim(IsAming);
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
        return !playerInventory.IsChanging && !IsReloading && nextTimeToFire > weaponData.fireRate && CurrentAmmo > 0;
    }
    private void Shoot()
    {
        audioSource.pitch = Random.Range(weaponData.pitch - weaponData.pitchRand, weaponData.pitch + weaponData.pitchRand);
        audioSource.PlayOneShot(weaponData.shootSound, 0.3f);
        if (muzzleFlash != null) muzzleFlash.Play();
        //recoil
        if (IsAming)
        {
            actualRecoil.RecoilFire(weaponData.aimRecoilRotation);
            viusalRecoil.VisualRecoilFire(weaponData.vRecoilRotationAim, weaponData.vRecoilKickBackAim);
        }
        else
        {
            actualRecoil.RecoilFire(weaponData.recoilRotation);
            viusalRecoil.VisualRecoilFire(weaponData.vRecoilRotation, weaponData.vRecoilKickBack);
        }
        CurrentAmmo--;
        nextTimeToFire = 0;

        RaycastHit hit;
        Physics.Raycast(WeaponCam.transform.position, WeaponCam.transform.forward, out hit, weaponData.range, shootLayer);

        if (hit.transform)
        {
            CheckEnemyHit(hit);
        }

    }
    private bool CheckEnemyHit(RaycastHit hit)
    {
        GameObject bulletParticles = Instantiate(weaponData.impactPrefab, hit.point + hit.normal * 0.001f, Quaternion.identity);
        bulletParticles.transform.LookAt(hit.point + hit.normal);
        Destroy(bulletParticles, 2);
        //enemy hit
        var hitBox = hit.collider.GetComponent<EnemyHitBox>();
        if (hitBox != null)
        {
            bulletParticles.transform.localScale *= 2;
            hitBox.HealthSystem.WaslastHitHead = false;//reset variable
            if (hitBox.CompareTag("Head"))//Headshot
            {
                hitBox.HealthSystem.WaslastHitHead = true;
                hitBox.OnHit((int)(weaponData.damage * RarityData.multiplier * weaponData.headshotMultiplier), transform);
                audioSource.PlayOneShot(gameManager.headshotSound, 0.2f);
            }
            else
            {
                hitBox.OnHit(weaponData.damage * RarityData.multiplier, transform);
                audioSource.pitch = Random.Range(0.95f, 1.05f);
                audioSource.PlayOneShot(weaponData.impactSound, 0.5f);
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
            PlayerCam.fieldOfView = Mathf.Lerp(PlayerCam.fieldOfView, weaponData.aimFOV, weaponData.aimSpeed * Time.deltaTime);
            gameManager.HUDCrosshair.rectTransform.localScale = new Vector3(weaponData.crosshairSizeAim, weaponData.crosshairSizeAim, weaponData.crosshairSizeAim);
            PlayerLook.sensMult = weaponData.sensitivityMultiplierAim;//reduce sensitivity when aiming
        }
        else
        {
            anchor.position = Vector3.Lerp(anchor.position, hipState.position, Time.deltaTime * weaponData.aimSpeed);
            PlayerCam.fieldOfView = Mathf.Lerp(PlayerCam.fieldOfView, weaponData.mainFOV, weaponData.aimSpeed * Time.deltaTime);
            gameManager.HUDCrosshair.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);//default size of the crosshair
            PlayerLook.sensMult = weaponData.sensitivityMultiplierDefault;
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
