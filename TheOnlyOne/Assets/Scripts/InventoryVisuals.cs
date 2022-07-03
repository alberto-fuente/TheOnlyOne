using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryVisuals : MonoBehaviour
{
    private PlayerInventory playerInventory;
    private Vector3 defaultScale = new Vector3(1.6f, 1.392f, 10);
    private Vector3 selectedScale = new Vector3(2.08f, 1.81f, 10);

    [Header("Sounds")] 
    private AudioManager audioManager;
    [SerializeField] private AudioClip pickSound;
    [SerializeField] private AudioClip dropSound;
    [SerializeField] private AudioClip switchSound;

    void Start()
    {
        audioManager = AudioManager.Instance;
        playerInventory = FindObjectOfType<PlayerInventory>();
        playerInventory.OnItemAdded += InventoryScript_ItemAdded;
        playerInventory.OnItemRemoved += InventoryScript_ItemRemoved;
        playerInventory.OnNewItemSwitched += InventoryScript_NewItemSwitched;
        playerInventory.OnOldItemSwitched += InventoryScript_OldItemSwitched;
    }
    private void OnDisable()
    {
        playerInventory.OnItemAdded -= InventoryScript_ItemAdded;
        playerInventory.OnItemRemoved -= InventoryScript_ItemRemoved;
        playerInventory.OnNewItemSwitched -= InventoryScript_NewItemSwitched;
        playerInventory.OnOldItemSwitched -= InventoryScript_OldItemSwitched;
    }
    //make new slot larger
    private void InventoryScript_NewItemSwitched(object sender, InventoryEventArgs _slot)
    {
        transform.GetChild(_slot.SlotId).localScale = selectedScale;
    }
    //make previus slot default size
    private void InventoryScript_OldItemSwitched(object sender, InventoryEventArgs _slot)
    {
        audioManager.PlaySound(switchSound, 0.3f);
        transform.GetChild(_slot.SlotId).localScale = defaultScale;
    }
    //update visual components when item is added
    private void InventoryScript_ItemAdded(object sender, InventoryEventArgs _slot)
    {

        audioManager.PlaySound(pickSound);
        Transform imageTransform = transform.GetChild(_slot.SlotId).GetChild(0).GetChild(0);
        Transform textTransform = transform.GetChild(_slot.SlotId).GetChild(0).GetChild(1);

        Image icon = imageTransform.GetComponent<Image>();
        TMP_Text txtCount = textTransform.GetComponent<TMP_Text>();
        //background color
        Color color = Color.white;//default
        if (_slot.Item.typeOfItem.Equals(GameUtils.TypeOfItem.GUN))
        {
            color = _slot.Item.gameObject.GetComponent<Weapon>().RarityData.color;
            imageTransform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        }
        else if (_slot.Item.typeOfItem.Equals(GameUtils.TypeOfItem.THROWEABLE))
        {
            color = _slot.Item.gameObject.GetComponent<Granade>().GranadeData.color;
            imageTransform.localScale = new Vector3(.8f, .8f, .8f);//granade sprites are bigger
        }
        //set frame color according to item's rarity or typef
        transform.GetChild(_slot.SlotId).GetChild(0).GetComponent<Image>().color = color;

        icon.enabled = true;
        icon.sprite = _slot.Item.Icon;
        int itemCount = _slot.Item.Slot.Count();
        txtCount.text = itemCount > 1 ? itemCount.ToString() : "";
    }
    //update visual components when item is removed
    private void InventoryScript_ItemRemoved(object sender, InventoryEventArgs _slot)
    {
        audioManager.PlaySound(dropSound, 0.3f);
        Transform imageTransform = transform.GetChild(_slot.SlotId).GetChild(0).GetChild(0);
        Transform textTransform = transform.GetChild(_slot.SlotId).GetChild(0).GetChild(1);

        Image image = imageTransform.GetComponent<Image>();
        TMP_Text txtCount = textTransform.GetComponent<TMP_Text>();
        //color background
        int itemCount = _slot.Item.Slot.Count();
        switch (itemCount)
        {
            case 0://slot empty
                image.enabled = false;
                image.sprite = null;
                txtCount.text = "";
                transform.GetChild(_slot.SlotId).GetChild(0).GetComponent<Image>().color = Color.white;//Reset
                break;
            case 1:
                txtCount.text = "";
                break;
            default://more than one item
                txtCount.text = itemCount.ToString();
                break;
        }
    }
}