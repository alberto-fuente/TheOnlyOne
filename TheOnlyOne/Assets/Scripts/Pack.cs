using UnityEngine;

public class Pack : MonoBehaviour
{
    public GameUtils.TypeOfPack type;
    private GameManager gameManager;
    private ItemRarityBlueprint rarityData;
    private HealthSystem healthSystem;
    public Label label;
    private GameObject prefab;
    [SerializeField] private AudioClip pickPack;
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        GenerateAmmoPack();
        //label
        string statText = "";
        switch (type)
        {
            case GameUtils.TypeOfPack.AMMO:
                statText = "x" + rarityData.multiplier;
                break;
            case GameUtils.TypeOfPack.HEALTH:
                statText = "+" + GameUtils.MAXHEALTH / GameUtils.numberOfRarities * rarityData.multiplier;
                break;
            case GameUtils.TypeOfPack.ARMOR:
                statText = "+" + GameUtils.MAXSHIELD / GameUtils.numberOfRarities * rarityData.multiplier;
                break;
        }
        prefab = GetComponentInChildren<Collider>().gameObject;
        gameManager.GenerateLabel(prefab.transform,prefab.transform.position+new Vector3(0,0.3f,0), rarityData.name, rarityData.rarity, rarityData.labelIcon, statText, rarityData.color);
    }
    private void Start()
    {
        label = GetComponentInChildren<Label>();
    }
    private void GenerateAmmoPack()
    {
        switch (type)
        {
            case GameUtils.TypeOfPack.AMMO:
                rarityData = GetRarityPack(gameManager.rarityPackAmmo);
                break;
            case GameUtils.TypeOfPack.HEALTH:
                rarityData = GetRarityPack(gameManager.rarityPackHealth);
                break;
            case GameUtils.TypeOfPack.ARMOR:
                rarityData = GetRarityPack(gameManager.rarityPackArmor);
                break;
        }

        Instantiate(rarityData.prefab, transform);
    }
    ItemRarityBlueprint GetRarityPack(ItemRarityBlueprint[] collection)
    {
        int i = Random.Range(0, 100);
        for (int j = 0; j < collection.Length; j++)
        {
            if (i >= collection[j].Minprobabilty && i <= collection[j].Maxprobabilty)
            {
                return collection[j];
            }
        }
        return collection[0];//si hay algún error genera un paquete de munición común
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect(other, type);
        }
    }
    public void Collect(Collider collector, GameUtils.TypeOfPack type)
    {
        FindObjectOfType<GameManager>().GetComponent<AudioSource>().PlayOneShot(pickPack);
        switch (type)
        {
            case GameUtils.TypeOfPack.AMMO:
                ItemHolder itemHolder = collector.gameObject.GetComponentInParent<PlayerController>().itemHolder;
                foreach (InventorySlot slot in itemHolder.inventory)
                {
                    if (slot.FirstItem() != null && slot.FirstItem().typeOfItem == GameUtils.TypeOfItem.GUN)
                    {
                        Weapon weapon = slot.FirstItem().gameObject.GetComponent<Weapon>();
                        weapon.totalAmmo += weapon.weaponData.maxClipAmmo * (int)rarityData.multiplier;
                    }
                }
                break;
            case GameUtils.TypeOfPack.HEALTH:
                healthSystem = collector.gameObject.GetComponentInParent<HealthSystem>();
                /*
                    * Common: +25
                    * Rare: +50
                    * Epic: +75
                    * Legendary: +100
                    */
                healthSystem.HealHealth(healthSystem.MaxHealth / 4 * (int)rarityData.multiplier);
                break;
            case GameUtils.TypeOfPack.ARMOR:
                healthSystem = collector.gameObject.GetComponentInParent<HealthSystem>();
                /*
                    * Common: +25
                    * Rare: +50
                    * Epic: +75
                    * Legendary: +100
                    */
                healthSystem.HealShield(healthSystem.MaxShield / 4 * rarityData.multiplier);
                break;
        }

        Destroy(gameObject);
    }
}
