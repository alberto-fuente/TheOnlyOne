using UnityEngine;

public class Pack : MonoBehaviour
{
    public string packName;//cambiarlo por un enum con los tipos de packs que hay
    private GameManager gameManager;
    private ItemRarityBlueprint packData;
    private HealthSystem healthSystem;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        GenerateAmmoPack();
    }
    private void GenerateAmmoPack()
    {
        switch (packName)
        {
            case "Ammo":
                packData = GetRarityPack(gameManager.rarityPackAmmo);
                break;
            case "Health":
                packData = GetRarityPack(gameManager.rarityPackHealth);
                break;
            case "Armor":
                packData = GetRarityPack(gameManager.rarityPackArmor);
                break;
        }

        Instantiate(packData.prefab, transform);
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
            Collect(other, packName);
        }
    }
    public void Collect(Collider collector, string packName)
    {
        switch (packName)
        {
            case "Ammo":

                ItemHolder itemHolder = collector.gameObject.GetComponentInParent<PlayerController>().itemHolder;
                foreach (InventorySlot slot in itemHolder.inventory)
                {
                    if (slot.FirstItem() != null && slot.FirstItem().typeOfItem == GameUtils.TypeOfItem.GUN)
                    {
                        Weapon weapon = slot.FirstItem().gameObject.GetComponent<Weapon>();
                        weapon.totalAmmo += weapon.weaponData.maxClipAmmo * (int)packData.multiplier;
                    }
                }
                break;
            case "Health":
                healthSystem = collector.gameObject.GetComponentInParent<HealthSystem>();
                /*
                    * Common: +25
                    * Rare: +50
                    * Common: +75
                    * Common: +100
                    */
                healthSystem.HealHealth(healthSystem.MaxHealth / 4 * (int)packData.multiplier);
                Debug.Log(healthSystem.MaxHealth / 4 * (int)packData.multiplier);
                break;
            case "Armor":
                healthSystem = collector.gameObject.GetComponentInParent<HealthSystem>();
                /*
                    * Common: +15
                    * Rare: +30
                    * Common: +45
                    * Common: +60
                    */
                healthSystem.HealShield(healthSystem.MaxShield / 4 * packData.multiplier);
                break;
        }

        Destroy(gameObject);
    }
}
