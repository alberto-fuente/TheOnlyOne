using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Image hudCrosshair;
    [SerializeField] private Sprite defaultCrosshair;
    public int totalTargets;
    public int remainingTargets;
    public ItemHolder weaponHolder;
    public GameObject ammoPanel;
    public float gameTimer = 0f;
    public float fpsRate;
    public Text timerText;
    public Text fpsText;
    public TMP_Text currentAmmoText;
    public TMP_Text totalAmmoText;

    public ItemRarityBlueprint[] rarityDataPistols;
    public ItemRarityBlueprint[] rarityDataSubfusils;
    public ItemRarityBlueprint[] rarityDataRifles;

    public ItemRarityBlueprint[] rarityPackAmmo;
    public ItemRarityBlueprint[] rarityPackHealth;
    public ItemRarityBlueprint[] rarityPackArmor;

    public GranadeBlueprint[] granadeTypes;

    public int playersLeft;
    public TMP_Text playersLeftText;
    //Scene currentScene;
    PhysicsScene currentPhysicsScene;

    //public Scene CurrentScene { get => currentScene; set => currentScene = value; }
    public PhysicsScene CurrentPhysicsScene { get => currentPhysicsScene; set => currentPhysicsScene = value; }
    
    // Start is called before the first frame update
    void Start()
    {
        hudCrosshair.enabled=false;
        weaponHolder = FindObjectOfType<ItemHolder>();
        totalTargets = GameObject.FindGameObjectsWithTag("Target").Length;
        remainingTargets = totalTargets;
        Physics.IgnoreLayerCollision(10, 11);//player and bullet
        Physics.IgnoreLayerCollision(7, 11);
    }
    
    private void FixedUpdate()
    {
        if (CurrentPhysicsScene.IsValid())
        {
            CurrentPhysicsScene.Simulate(Time.fixedDeltaTime);
        }
    }
    // Update is called once per frame
    void Update()
    {
        playersLeft = FindObjectsOfType<HealthSystem>().Length;
        playersLeftText.text = playersLeft.ToString();
        if (hudCrosshair.sprite == null) hudCrosshair.sprite=defaultCrosshair;
        fpsRate = 1f / Time.deltaTime;
        fpsText.text = fpsRate.ToString("F2");
       // CurrentScene = SceneManager.GetActiveScene();
       // CurrentPhysicsScene = CurrentScene.GetPhysicsScene();

        timerText.text = gameTimer.ToString("F2");

        if (remainingTargets < totalTargets && remainingTargets!=0)
        {
            gameTimer += Time.deltaTime;
        }

        if (weaponHolder.GetCurrentItem()!=null&&weaponHolder.GetCurrentItem().typeOfItem.Equals(GameUtils.TypeOfItem.GUN))
        {
            ammoPanel.SetActive(true);
            currentAmmoText.text = weaponHolder.GetCurrentItem().GetComponent<Weapon>().currentAmmo.ToString();
            totalAmmoText.text = weaponHolder.GetCurrentItem().GetComponent<Weapon>().totalAmmo.ToString();
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
}
