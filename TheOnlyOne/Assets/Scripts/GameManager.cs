using Firebase.Database;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [Header("Crosshair")]
    public Image HUDCrosshair;
    [SerializeField] private Sprite defaultCrosshair;

    [Header("Finish Screens")]
    public GameObject winCanvas;
    public GameObject loseCanvas;
    public GameObject expCanvas;

    [Header("Properties")]
    private bool matchIsFinished;
    [SerializeField] private float safeRadius = 300;

    [Header("Entities")]
    public TMP_Text entitiesLeftText;
    public TMP_Text defeatedEnemies;
    [HideInInspector] public int beatenEnemies = 0;
    private int entitiesLeft;

    [Header("References")]
    public PlayerInventory playerInventory;
    public GameObject ammoPanel;
    public GameObject emptyLabel;
    public AudioClip headshotSound;
    private DatabaseReference DBreference;

    [Header("Ammo UI")]
    public TMP_Text currentAmmoText;
    public TMP_Text totalAmmoText;

    [Header("Weapons' rarity")]
    public ItemRarityBlueprint[] rarityDataPistols;
    public ItemRarityBlueprint[] rarityDataSubfusils;
    public ItemRarityBlueprint[] rarityDataRifles;
    public ItemRarityBlueprint[] rarityDataSnipers;
    public ItemRarityBlueprint[] rarityDataShotguns;

    [Header("Packs' rarity")]
    public ItemRarityBlueprint[] rarityPacksAmmo;
    public ItemRarityBlueprint[] rarityPacksHealth;
    public ItemRarityBlueprint[] rarityPacksArmor;

    [Header("Granades")]
    public GranadeBlueprint[] granadeTypes;

    [Header("Enemies")]
    public EnemyBlueprint[] enemyTypes;

    [Header("Spawnable items")]
    public GameObject[] spawnableItems;//can be spawned in crates and when enemy dies

    [Header("Player XP UI")]
    public TMP_Text playerRank;
    public TMP_Text beatenEnemiesField;
    public TMP_Text matchExpField;
    public TMP_Text enemiesExpField;
    public TMP_Text victoryExpField;
    public TMP_Text totalExpField;

    private int matchExp = 200;
    private int expPerEnemy = 25;
    private int victoryExp = 300;
    private int totalExp = 0;

    public static GameManager Instance { get => instance; private set => instance = value; }
    public float SafeRadius { get => safeRadius; set => safeRadius = value; }
    public int EntitiesLeft { get => entitiesLeft; set => entitiesLeft = value; }

    void Awake()
    {
        if (GameManager.Instance == null)
        {
            GameManager.Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        playerInventory = FindObjectOfType<PlayerInventory>();
        EntitiesLeft = FindObjectsOfType<HealthSystem>().Length;
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Weapon"));
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Bullet"));
    }

    void Update()
    {
        entitiesLeftText.text = EntitiesLeft.ToString();
        if (HUDCrosshair.sprite == null) HUDCrosshair.sprite = defaultCrosshair;
        CheckMatchOver();
        CheckWeaponEquiped();
    }
    private void CheckWeaponEquiped()
    {
        if (playerInventory.GetCurrentItem() != null && playerInventory.GetCurrentItem().typeOfItem.Equals(GameUtils.TypeOfItem.GUN))
        {
            ammoPanel.SetActive(true);
            Weapon weapon = playerInventory.GetCurrentItem().GetComponent<Weapon>();
            int currentAmmo = weapon.CurrentAmmo;
            int totalAmmo = weapon.TotalAmmo;
            int maxClipAmmo = weapon.weaponData.maxClipAmmo;
            currentAmmoText.text = currentAmmo.ToString();
            totalAmmoText.text = totalAmmo.ToString();
            currentAmmoText.color = currentAmmo < maxClipAmmo / 3 ? Color.red : Color.white;

        }
        else
        {
            if (ammoPanel != null) ammoPanel.SetActive(false);
        }
    }
    public void GenerateLabel(Transform _parent, Vector3 _position, string _name, string _type, Sprite icon, string _stat, Color _color)
    {
        GameObject label = Instantiate(emptyLabel, _position, Quaternion.identity, _parent);
        Label labelInfo = label.GetComponent<Label>();
        labelInfo.itemName.text = _name;
        labelInfo.itemType.text = _type;
        labelInfo.icon.sprite = icon;
        labelInfo.stat.text = _stat;
        labelInfo.color = _color;
    }

    private void CheckMatchOver()
    {
        if (!matchIsFinished && EntitiesLeft == 1 && playerInventory.isActiveAndEnabled)//player wins
        {
            matchIsFinished = true;
            Win();
        }
        if (!matchIsFinished && !playerInventory.isActiveAndEnabled)//player loses
        {
            matchIsFinished = true;
            Lose();
        }
    }
    private void SetExpValues(bool _hasWon)
    {
        int enemiesExp = beatenEnemies * expPerEnemy;
        victoryExp = _hasWon ? 300 : 0;
        totalExp = matchExp + enemiesExp + victoryExp;

        matchExpField.text = "+" + matchExp.ToString();
        beatenEnemiesField.text = beatenEnemies.ToString();
        enemiesExpField.text = "+" + enemiesExp.ToString();
        victoryExpField.text = "+" + victoryExp.ToString();
        totalExpField.text = totalExp.ToString();
        //Update database
        StartCoroutine(UpdateUserExpDB(totalExp));
    }
    private void Win()
    {
        StartCoroutine(EndMatchCorroutine(true));
    }
    public void Lose()
    {
        playerRank.text = "#" + (EntitiesLeft + 1).ToString();//final rank
        StartCoroutine(EndMatchCorroutine(false));
    }

    private IEnumerator EndMatchCorroutine(bool _hasWon)
    {
        ShowCursor();
        if (_hasWon)
        {
            winCanvas.SetActive(true);
        }
        else
        {
            loseCanvas.SetActive(true);
        }

        SetExpValues(_hasWon);
        yield return new WaitForSecondsRealtime(1);
        expCanvas.transform.localScale = new Vector3(0, 0, 0);
        expCanvas.SetActive(true);
        LeanTween.scale(expCanvas, new Vector3(1, 1, 1), 0.5f).setOnComplete(slowTime);
        yield return null;
    }
    private void slowTime()
    {
        Time.timeScale = 0.1f;
    }
    private IEnumerator UpdateUserExpDB(int xp)
    {
        var user = FirebaseManager.User;
        int userXP = 0;
        int newTotalXP = 0;
        var DBGetTask = DBreference.Child("players").Child(user.UserId).Child("xp").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBGetTask.IsCompleted);
        if (DBGetTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to get player's xp with{DBGetTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBGetTask.Result;
            userXP = Convert.ToInt32(snapshot.Value);//saved xp
        }

        newTotalXP = userXP + xp;
        var DBSetTask = DBreference.Child("players").Child(user.UserId).Child("xp").SetValueAsync(newTotalXP);
        yield return new WaitUntil(predicate: () => DBSetTask.IsCompleted);
        if (DBSetTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to update player's xp with{DBSetTask.Exception}");
        }
    }

    private void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
