using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot
{
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

    public bool IsStackable(GrabbableItem item)
    {
        if (IsEmpty()) return false;
        if (FirstItem().isStackable && item.itemID.Equals(FirstItem().itemID)) return true;
        return false;
    }
    public GrabbableItem GetItemAt(int index)
    {
        if (IsEmpty()) return null;
        GrabbableItem[] array = itemStack.ToArray();
        return array[index];
    }
    public void AddItem(GrabbableItem item)
    {
        item.Slot = this;
        itemStack.Push(item);
    }

    public bool RemoveItem(GrabbableItem item)
    {
        if (IsEmpty()) return false;
        itemStack.Pop();
        return true;

    }
}
