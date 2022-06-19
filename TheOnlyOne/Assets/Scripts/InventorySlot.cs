using System.Collections.Generic;

public class InventorySlot
{
    //Each slot is structured as a stack of items
    private Stack<GrabbableItem> itemStack = new Stack<GrabbableItem>();
    private int id = 0;
    public int Id { get => id; }
    public InventorySlot(int id)
    {
        this.id = id;
    }
    public int Count()
    {
        return itemStack.Count;
    }
    public bool IsEmpty()
    {
        return Count() <= 0;
    }
    public GrabbableItem FirstItem()
    {
        if (IsEmpty()) return null;
        return itemStack.Peek();
    }
    public bool IsStackable(GrabbableItem _item)
    {
        if (IsEmpty()) return false;
        return (FirstItem().IsStackable && _item.ItemID.Equals(FirstItem().ItemID));
    }
    public GrabbableItem GetItemAt(int _index)
    {
        if (IsEmpty()) return null;
        GrabbableItem[] array = itemStack.ToArray();
        return array[_index];
    }
    public void AddItem(GrabbableItem _item)
    {
        _item.Slot = this;
        itemStack.Push(_item);
    }
    public bool RemoveItem()
    {
        if (IsEmpty()) return false;
        itemStack.Pop();
        return true;
    }
}
