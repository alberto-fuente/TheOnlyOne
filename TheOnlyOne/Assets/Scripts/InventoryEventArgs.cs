using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryEventArgs : EventArgs
{
    public GrabbableItem Item;

    public int SlotId;
    public InventoryEventArgs(int slotId)
    {
        SlotId = slotId;
    }
    public InventoryEventArgs(GrabbableItem item,int slotId)
    {
        Item = item;
        SlotId = slotId;
    }

}