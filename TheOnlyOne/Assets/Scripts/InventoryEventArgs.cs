using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryEventArgs : EventArgs
{
    public PickableItem Item;

    public int SlotId;
    public InventoryEventArgs(int slotId)
    {
        SlotId = slotId;
    }
    public InventoryEventArgs(PickableItem item,int slotId)
    {
        Item = item;
        SlotId = slotId;
    }

}