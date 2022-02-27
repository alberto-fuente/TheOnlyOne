using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    public ItemHolder itemHolder;
    
    // Start is called before the first frame update
    void Start()
    {
        itemHolder.OnItemAdded += InventoryScript_ItemAdded;
        itemHolder.OnItemRemoved += InventoryScript_ItemRemoved;
        itemHolder.OnNewItemSwitched+= InventoryScript_NewItemSwitched;
        itemHolder.OnOldItemSwitched += InventoryScript_OldItemSwitched;
    }

    private void InventoryScript_NewItemSwitched(object sender, InventoryEventArgs e)
    {
        Transform inventoryPanel = transform.Find("InventoryPanel");

        inventoryPanel.GetChild(e.SlotId).localScale = new Vector3(1.8f, 1.15f, 1.15f);
        inventoryPanel.GetChild(e.SlotId).GetChild(0).GetComponent<Image>().color = Color.blue;
    }
    private void InventoryScript_OldItemSwitched(object sender, InventoryEventArgs e)
    {
        Transform inventoryPanel = transform.Find("InventoryPanel");

        inventoryPanel.GetChild(e.SlotId).localScale = new Vector3(1.568628f, 1f, 1f);
        inventoryPanel.GetChild(e.SlotId).GetChild(0).GetComponent<Image>().color = Color.white;
    }

    private void InventoryScript_ItemAdded(object sender, InventoryEventArgs e)
    {
        Transform inventoryPanel = transform.Find("InventoryPanel");

        Transform imageTransform = inventoryPanel.GetChild(e.SlotId).GetChild(0).GetChild(0);
        Transform textTransform = inventoryPanel.GetChild(e.SlotId).GetChild(0).GetChild(1);
        Image image = imageTransform.GetComponent<Image>();
        TMP_Text txtCount = textTransform.GetComponent<TMP_Text>();

        image.enabled = true;
        image.sprite = e.Item.Icon;

        int itemCount = e.Item.Slot.Count();
        if (itemCount > 1)
            txtCount.text = itemCount.ToString();
        else
            txtCount.text = "";
    }
    private void InventoryScript_ItemRemoved(object sender, InventoryEventArgs e)
    {
        Transform inventoryPanel = transform.Find("InventoryPanel");

        Transform imageTransform = inventoryPanel.GetChild(e.SlotId).GetChild(0).GetChild(0);
        Transform textTransform = inventoryPanel.GetChild(e.SlotId).GetChild(0).GetChild(1);

        Image image = imageTransform.GetComponent<Image>();
        TMP_Text txtCount = textTransform.GetComponent<TMP_Text>();

        int itemCount = e.Item.Slot.Count();
        switch (itemCount){
            case 0:
                image.enabled = false;
                image.sprite = null;
                txtCount.text = "";
                break;
            case 1:
                txtCount.text = "";
                break;
            default:
                txtCount.text = itemCount.ToString();
                break;
        }
     }
    
}