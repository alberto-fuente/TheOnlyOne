using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryVisuals : MonoBehaviour
{
    private PlayerInventory itemHolder;
    private const float SCALEFACTOR = 1.3f;
    
    void Start()
    {
        itemHolder = FindObjectOfType<PlayerInventory>();
        itemHolder.OnItemAdded += InventoryScript_ItemAdded;
        itemHolder.OnItemRemoved += InventoryScript_ItemRemoved;
        itemHolder.OnNewItemSwitched+= InventoryScript_NewItemSwitched;
        itemHolder.OnOldItemSwitched += InventoryScript_OldItemSwitched;
    }
    private void OnDisable()
    {
        itemHolder.OnItemAdded -= InventoryScript_ItemAdded;
        itemHolder.OnItemRemoved -= InventoryScript_ItemRemoved;
        itemHolder.OnNewItemSwitched -= InventoryScript_NewItemSwitched;
        itemHolder.OnOldItemSwitched -= InventoryScript_OldItemSwitched;
    }
    //make new slot larger
    private void InventoryScript_NewItemSwitched(object sender, InventoryEventArgs _slot)
    {
        transform.GetChild(_slot.SlotId).localScale *= SCALEFACTOR;
    }
    //make previus slot default size
    private void InventoryScript_OldItemSwitched(object sender, InventoryEventArgs _slot)
    {
        transform.GetChild(_slot.SlotId).localScale /= SCALEFACTOR;
    }
    //update visual components when item is added
    private void InventoryScript_ItemAdded(object sender, InventoryEventArgs _slot)
    {
        Transform imageTransform = transform.GetChild(_slot.SlotId).GetChild(0).GetChild(0);
        Transform textTransform = transform.GetChild(_slot.SlotId).GetChild(0).GetChild(1);

        Image icon = imageTransform.GetComponent<Image>();
        TMP_Text txtCount = textTransform.GetComponent<TMP_Text>();
        //background color
        Color color = Color.white;//default
        if (_slot.Item.typeOfItem.Equals(GameUtils.TypeOfItem.GUN))
        {
            color = _slot.Item.gameObject.GetComponent<Weapon>().rarityData.color;
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
        txtCount.text = itemCount > 1?itemCount.ToString(): "";
    }
    //update visual components when item is removed
    private void InventoryScript_ItemRemoved(object sender, InventoryEventArgs _slot)
    {
        Transform imageTransform = transform.GetChild(_slot.SlotId).GetChild(0).GetChild(0);
        Transform textTransform = transform.GetChild(_slot.SlotId).GetChild(0).GetChild(1);

        Image image = imageTransform.GetComponent<Image>();
        TMP_Text txtCount = textTransform.GetComponent<TMP_Text>();
        //color background
        int itemCount = _slot.Item.Slot.Count();
        switch (itemCount){
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