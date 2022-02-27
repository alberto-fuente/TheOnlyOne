using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot
{
    private Stack<PickableItem> itemStack = new Stack<PickableItem>();
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

    public PickableItem FirstItem()
    {
        if (IsEmpty()) return null;
        return itemStack.Peek();
    }

    public bool IsStackable(PickableItem item)
    {
        if (IsEmpty()) return false;
        if (FirstItem().isStackable && item.itemID.Equals(FirstItem().itemID)) return true;
        return false;
    }

    public void AddItem(PickableItem item)
    {
        item.Slot = this;
        itemStack.Push(item);
    }

    public bool RemoveItem(PickableItem item)
    {
        if (IsEmpty()) return false;
        itemStack.Pop();
        return true;

    }
}
