using UnityEngine;

public class Pack : MonoBehaviour
{
    [Header("References")]
    private ItemRarityBlueprint rarityData;
    private HealthSystem healthSystem;
    private PlayerInventory itemHolder;
    private GameManager gameManager;
    [Header("Components")]
    public GameUtils.TypeOfPack type;
    private string statText = "";
    private Label label;
    private GameObject prefab;
    [SerializeField] private GameObject pickVFX;
    [SerializeField] private AudioClip pickPack;

    public ItemRarityBlueprint RarityData { get => rarityData; set => rarityData = value; }
    public Label Label { get => label; set => label = value; }

    private void Start()
    {
        gameManager = GameManager.Instance;
        itemHolder = FindObjectOfType<PlayerInventory>();
        GeneratePack();
        prefab = GetComponentInChildren<Collider>().gameObject;
        gameManager.GenerateLabel(prefab.transform, prefab.transform.position + new Vector3(0, 0.3f, 0), RarityData.itemName, RarityData.rarity, RarityData.labelIcon, statText, RarityData.color);
        Label = GetComponentInChildren<Label>();
    }
    private void GeneratePack()
    {
        switch (type)
        {
            case GameUtils.TypeOfPack.AMMO:
                RarityData = GetRarityPack(gameManager.rarityPacksAmmo);
                statText = "x" + RarityData.multiplier;
                break;
            case GameUtils.TypeOfPack.HEALTH:
                RarityData = GetRarityPack(gameManager.rarityPacksHealth);
                statText = "+" + GameUtils.MAXHEALTH / GameUtils.numberOfRarities * RarityData.multiplier;
                break;
            case GameUtils.TypeOfPack.ARMOR:
                RarityData = GetRarityPack(gameManager.rarityPacksArmor);
                statText = "+" + GameUtils.MAXSHIELD / GameUtils.numberOfRarities * RarityData.multiplier;
                break;
        }
        Instantiate(RarityData.prefab, transform);
    }
    //Choose rarity of the pack according to its probability
    private ItemRarityBlueprint GetRarityPack(ItemRarityBlueprint[] collection)
    {
        int i = Random.Range(0, 100);
        for (int j = 0; j < collection.Length; j++)
        {
            if (i >= collection[j].Minprobabilty && i <= collection[j].Maxprobabilty)
            {
                return collection[j];
            }
        }
        return collection[0];//si hay alg�n error genera un paquete de munici�n com�n
    }
    public void Collect(Collider collector, GameUtils.TypeOfPack type)
    {
        AudioManager.Instance.PlaySound(pickPack);
        switch (type)
        {
            //give each equiped weapon an amount of full magazines according to the rarity of the pack
            case GameUtils.TypeOfPack.AMMO:
                foreach (InventorySlot slot in itemHolder.inventory)
                {
                    if (slot.Peek() != null && slot.Peek().typeOfItem.Equals(GameUtils.TypeOfItem.GUN))
                    {
                        Weapon weapon = slot.Peek().gameObject.GetComponent<Weapon>();
                        weapon.TotalAmmo += weapon.weaponData.maxClipAmmo * RarityData.multiplier;
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
                healthSystem.HealHealth(healthSystem.MaxHealth / GameUtils.numberOfRarities * RarityData.multiplier);
                break;
            case GameUtils.TypeOfPack.ARMOR:
                healthSystem = collector.gameObject.GetComponentInParent<HealthSystem>();
                /*
                    * Common: +25
                    * Rare: +50
                    * Epic: +75
                    * Legendary: +100
                */
                healthSystem.HealShield(healthSystem.MaxArmor / GameUtils.numberOfRarities * RarityData.multiplier);
                break;
        }
        GameObject vfx = Instantiate(pickVFX, gameObject.transform.position, Quaternion.identity);
        Destroy(vfx, 2);
        Destroy(gameObject);
    }
}
