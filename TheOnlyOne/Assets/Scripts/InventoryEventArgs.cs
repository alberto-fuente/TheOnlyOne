using System;
public class InventoryEventArgs : EventArgs
{
    public GrabbableItem Item;
    public int SlotId;
    public InventoryEventArgs(int _slotId)
    {
        SlotId = _slotId;
    }
    public InventoryEventArgs(GrabbableItem _item,int _slotId)
    {
        Item = _item;
        SlotId = _slotId;
    }

}