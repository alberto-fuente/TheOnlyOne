using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase;
using Firebase.Database;
public class GameManager : MonoBehaviour
{
    public float minutesToPlay = 15;
    public Image hudCrosshair;
    [SerializeField] private Sprite defaultCrosshair;
    public ItemHolder itemHolder;
    public GameObject ammoPanel;
    public float gameTimer = 0f;
    [SerializeField] private float safeRadius = 300;
    public float fpsRate;
    public TMP_Text fpsText;
    public TMP_Text currentAmmoText;
    public TMP_Text totalAmmoText;
    public AudioClip headshotSound;

    public ItemRarityBlueprint[] rarityDataPistols;
    public ItemRarityBlueprint[] rarityDataSubfusils;
    public ItemRarityBlueprint[] rarityDataRifles;
    public ItemRarityBlueprint[] rarityDataSnipers;
    public ItemRarityBlueprint[] rarityDataShotguns;

    public ItemRarityBlueprint[] rarityPackAmmo;
    public ItemRarityBlueprint[] rarityPackHealth;
    public ItemRarityBlueprint[] rarityPackArmor;

    public GranadeBlueprint[] granadeTypes;

    public EnemyBlueprint[] enemyTypes;

    public GameObject[] spawnableItems;//crates and enemies
    //Experience
    public TMP_Text beatenEnemiesField;
    public TMP_Text matchExpField;
    public TMP_Text enemiesExpField;
    public TMP_Text victoryExpField;
    public TMP_Text totalExpField;

    private int matchExp = 200;
    [HideInInspector] public int beatenEnemies = 0;
    public TMP_Text defeatedEnemies;
    private int expPerEnemy = 25;
    private int victoryExp = 0;
    private int totalExp = 0;
    
    public GameObject winCanvas;
    public GameObject loseCanvas;
    public GameObject expCanvas;

    private bool matchIsFinished;
    //Label
    public GameObject Label;

    public int entitiesLeft;
    public TMP_Text entitiesLeftText;

    DatabaseReference DBreference;
    //Scene currentScene;
    PhysicsScene currentPhysicsScene;

    //public Scene CurrentScene { get => currentScene; set => currentScene = value; }
    public PhysicsScene CurrentPhysicsScene { get => currentPhysicsScene; set => currentPhysicsScene = value; }
    public float SafeRadius { get => safeRadius; set => safeRadius = value; }

    // Start is called before the first frame update
    void Start()
    {
        hudCrosshair.enabled=false;
        itemHolder = FindObjectOfType<ItemHolder>();

        Physics.IgnoreLayerCollision(10, 11);//player and bullet
        Physics.IgnoreLayerCollision(7, 11);

        DBreference = FirebaseDatabase.DefaultInstance.RootReference;


    }
    
    private void FixedUpdate()
    {
       /* if (CurrentPhysicsScene.IsValid())
        {
            CurrentPhysicsScene.Simulate(Time.fixedDeltaTime);
        }*/
    }
    // Update is called once per frame
    void Update()
    {

        entitiesLeft = FindObjectsOfType<HealthSystem>().Length;
        entitiesLeftText.text = entitiesLeft.ToString();
        CheckMatchOver();
        if (hudCrosshair.sprite == null) hudCrosshair.sprite=defaultCrosshair;
        fpsRate = 1f / Time.deltaTime;
        fpsText.text = fpsRate.ToString("F2");
       // CurrentScene = SceneManager.GetActiveScene();
       // CurrentPhysicsScene = CurrentScene.GetPhysicsScene();

        if (itemHolder.GetCurrentItem()!=null&&itemHolder.GetCurrentItem().typeOfItem.Equals(GameUtils.TypeOfItem.GUN))
        {
            ammoPanel.SetActive(true);
            Weapon weapon = itemHolder.GetCurrentItem().GetComponent<Weapon>();
            int ammo = weapon.currentAmmo;
            int totalAmmo = weapon.totalAmmo;
            int maxClipAmmo = weapon.weaponData.maxClipAmmo;
            currentAmmoText.text = ammo.ToString();
            totalAmmoText.text = totalAmmo.ToString();
            if (ammo < maxClipAmmo / 3)
            {
                currentAmmoText.color = Color.red;
            }
            else
            {
                currentAmmoText.color = Color.white;
            }

        }
        else
        {
            ammoPanel.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene(0);
        }

    }
    public void GenerateLabel(Transform parent, Vector3 position, string name, string type, Sprite icon, string stat, Color color)
    {
        GameObject label= Instantiate(Label, position, Quaternion.identity,parent);
        Label labelInfo = label.GetComponent<Label>();
        labelInfo.itemName.text = name;
        labelInfo.itemType.text = type;
        labelInfo.icon.sprite = icon;
        labelInfo.stat.text = stat;
        labelInfo.color = color;
    }
   
    private void CheckMatchOver()
    {

        if (!matchIsFinished&&entitiesLeft == 1 && itemHolder.isActiveAndEnabled)
        {
            matchIsFinished = true;
            Win();
        }
        if (!matchIsFinished && !itemHolder.isActiveAndEnabled)
        {
            matchIsFinished = true;
            Lose();
        }
    }
    private void SetExpValues(bool hasWon)
    {
        int enemiesExp = beatenEnemies * expPerEnemy;
        victoryExp = hasWon ? 300 : 0;
        totalExp = matchExp + enemiesExp + victoryExp;

        matchExpField.text = "+" + matchExp.ToString();
        beatenEnemiesField.text = beatenEnemies.ToString();
        enemiesExpField.text = "+" + enemiesExp.ToString();
        victoryExpField.text = "+" + victoryExp.ToString();
        totalExpField.text = totalExp.ToString();
        Debug.Log("match values updated");
        //Update database
        StartCoroutine(UpdateUserExp(totalExp));
    }
    private void Win()
    {
        StartCoroutine(WinCorroutine());
    }
    public void Lose()
    {
        StartCoroutine(LoseCorroutine());
    }
    
    private IEnumerator WinCorroutine()
    {
        Debug.Log("Winner");
        Time.timeScale = 0f;
        ShowCursor();
        winCanvas.SetActive(true);
        SetExpValues(true);
        yield return new WaitForSecondsRealtime(1);
        expCanvas.SetActive(true);
        yield return null;
    }
    public IEnumerator LoseCorroutine()
    {
        Debug.Log("Loser");
        Time.timeScale = 0f;
        ShowCursor();
        loseCanvas.SetActive(true);
        SetExpValues(false);
        yield return new WaitForSecondsRealtime(1);
        expCanvas.SetActive(true);
        yield return null;
    }
    private IEnumerator UpdateUserExp(int xp)
    {
        var authentic = FirebaseManager.auth;
        var user = FirebaseManager.User;
        int userXP=0;
        int newTotalXP = 0;
        Debug.Log("Actualizando exp");
        var DBGetTask = FirebaseManager.DBReference.Child("players").Child(user.UserId).Child("xp").GetValueAsync();
        yield return new WaitUntil(predicate: () => DBGetTask.IsCompleted);
        if (DBGetTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to get player's xp with{DBGetTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBGetTask.Result;

            Debug.Log("la experiencia total es "+ (int)snapshot.Value);
            userXP = (int)snapshot.Value;//experiencia existente
        }

        newTotalXP = userXP + xp;
        var DBSetTask = DBreference.Child("players").Child(user.UserId).Child("xp").SetValueAsync(newTotalXP);
        yield return new WaitUntil(predicate: ()=>DBSetTask.IsCompleted);
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
    public void LoadScene(int sceneIndex)
    {
        Time.timeScale = 1;
        SceneDirector.instance.LoadScene(sceneIndex);
    }
}
