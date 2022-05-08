using System.Collections;
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
    public AudioSource audioSource;
    private Recoil recoilScript;
    private VisualRecoil viusalRecoilScript;
    private ItemHolder weaponChanger;
    private GameManager gameManager;
    private Animator animator;
    public int currentAmmo;
    public int totalAmmo;
    private float nextTimeToFire;
    public bool isReloading;
    public bool isAming;
    Transform anchor;
    Transform hipState;
    Transform aimState;
    public Transform prefabContainer;
    public GameObject prefab;
    public ParticleSystem muzzleFlash;
    const int HEADSHOTMULTIPLIER = 2;
    LayerMask layerMask;
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        cameraHolder = GameObject.Find("/CameraHolder").transform;
        playerCam = GameObject.Find("/CameraHolder/CameraRecoil/MainCamera").GetComponent<Camera>();
        weaponCam = GameObject.Find("/CameraHolder/CameraRecoil/WeaponCamera").GetComponent<Camera>();
        weaponChanger = FindObjectOfType<ItemHolder>();
        audioSource = GetComponent<AudioSource>();
        recoilScript = FindObjectOfType<Recoil>();
        viusalRecoilScript = GetComponent<VisualRecoil>();
        layerMask =~(1<<7);
        anchor = transform.Find("Anchor");
        hipState = transform.Find("States/Hip");
        aimState = transform.Find("States/Aim");
        prefabContainer = transform.Find("Anchor/Design");
        GenerateWeapon();
        float damagePerSecond= weaponData.damage*rarityData.multiplier / weaponData.fireRate;
        prefab = GetComponentInChildren<Collider>().gameObject;
        gameManager.GenerateLabel(prefab.transform,prefab.transform.position+new Vector3(0,0.5f,0.5f),weaponData.weaponName,rarityData.rarity,rarityData.labelIcon,((int)damagePerSecond).ToString(),rarityData.color);
        muzzleFlash = GetComponentInChildren<ParticleSystem>();
       /* Transform[] weaponChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in weaponChildren)
        {
            if (child.name.Equals("MuzzleFlash")) muzzleFlash = child.GetComponent<ParticleSystem>();
        }*/
        animator = GetComponentInChildren<Animator>();

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
        return collection[0];//si hay algún error genera un arma común
    }
    private void OnEnable()
    {
        weaponChanger.OnItemRemoved += CutReload;
        weaponChanger.OnNewItemSwitched += CutReload;
        gameManager.hudCrosshair.sprite = weaponData.crosshair;
        animator.Play("WeaponUp");
    }
    private void OnDisable()
    {
        weaponChanger.OnItemRemoved -= CutReload;
        weaponChanger.OnOldItemSwitched -= CutReload;
        if (gameManager.hudCrosshair != null)
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
    bool CanReload()
    {
        return !isReloading && currentAmmo < weaponData.maxClipAmmo && totalAmmo > 0;//&& gameManager.IsSafeToReload; 
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
        return !weaponChanger.IsChanging && !isReloading && nextTimeToFire > weaponData.fireRate && currentAmmo > 0;
    }
    private void Shoot()
    {
        audioSource.pitch = Random.Range(weaponData.pitch - weaponData.pitchRand, weaponData.pitch + weaponData.pitchRand);
        audioSource.PlayOneShot(weaponData.shootSound, 0.3f);
        if (muzzleFlash) muzzleFlash.Play();

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

        currentAmmo--;
        nextTimeToFire = 0;
        RaycastHit hit;
        Physics.Raycast(weaponCam.transform.position, weaponCam.transform.forward, out hit, weaponData.range,layerMask);
        /*Decal
        if (Physics.Raycast(weaponCam.transform.position, weaponCam.transform.forward, out hit, weaponData.range))
        {
            GameObject decal = Instantiate(weaponData.bulletDecal, hit.point + (hit.normal * 0.025f), Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;//se instancia el decal
                                                                                                                                                               //Se rota el decal para adaptarse a la superficie
            decal.transform.parent = hit.transform;//el decal se "pega" al objeto con el que impacte
            Destroy(decal, 10f);//Se destruye el decal a los 10 segundos
        }*/
        if (hit.transform)
        {
            CheckEnemyHit(hit);
            //Destroy(hit.transform.gameObject);
        }

    }
    private bool CheckEnemyHit(RaycastHit hit)
    {
        var hitBox = hit.collider.GetComponent<EnemyHitBox>();
        if (hitBox)
        {
            hitBox.healthSystem.waslastHitHead = false;//reiniciamos la variable
            if (hitBox.CompareTag("Head"))//Headshot
            {
                hitBox.healthSystem.waslastHitHead = true;
                hitBox.OnHit(weaponData.damage * rarityData.multiplier * HEADSHOTMULTIPLIER);
                audioSource.PlayOneShot(gameManager.headshotSound, 0.2f);
            }
            else
            {
                hitBox.OnHit(weaponData.damage * rarityData.multiplier);
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
           // anchor.rotation = Quaternion.Slerp(anchor.rotation, aimState.rotation, Time.deltaTime * weaponData.aimSpeed);
            playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, weaponData.aimFOV, weaponData.aimSpeed * Time.deltaTime);
            gameManager.hudCrosshair.transform.localScale = new Vector3(weaponData.crosshairSizeAim, weaponData.crosshairSizeAim, weaponData.crosshairSizeAim);
        }
        else
        {

            anchor.position = Vector3.Lerp(anchor.position, hipState.position, Time.deltaTime * weaponData.aimSpeed);
          //  anchor.rotation = Quaternion.Slerp(anchor.rotation, hipState.rotation, Time.deltaTime * weaponData.aimSpeed);
            playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, weaponData.mainFOV, weaponData.aimSpeed * Time.deltaTime);
            gameManager.hudCrosshair.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

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
