using System;
using UnityEngine;
using UnityEngine.UIElements;

public delegate void ItemSlotChangeEvent(ItemSlot changeSlot);

public class ItemSlot
{   
    //칸에 들어 있는 아이템의 정보
    [SerializeField] ItemContainer item;
    // 이 칸 만의 정보
    
    [SerializeField] int currentStack;

    public event ItemSlotChangeEvent OnItemSlotChanged;
    public void NoticeChanged() => OnItemSlotChanged?.Invoke(this);
    public virtual bool Containable(ItemContainer wantItem)
    {
        if (wantItem is null) return false;
        if (item is not null && item != wantItem) return false;
        if (GetIsMax()) return false;
        return true;
    }
    public ItemContainer GetItem() => item;
    public int GetStack()          => currentStack;
    public bool GetIsMax()         => item ? currentStack >= item.maxStack : false;

    public bool GetIsEmpty()       => item is null || currentStack <= 0;

    public int Clear()
    {
        item = null;
        int removed = currentStack;
        currentStack = 0;
        return removed;
    }
    internal int AddItem(ItemContainer wantItem, int amount)
    {
        
        if (amount <= 0) return 0;
        if (!Containable(wantItem)) return amount;
        item =wantItem;
        int stackable = Mathf.Max(item.maxStack - currentStack, amount);
        currentStack += amount;
        return amount - stackable;
    }

    public int RemoveItem(ItemContainer wantItem)
    { 
        if(!wantItem) return 0;
        if(GetIsEmpty()) return 0;
        if(item != wantItem) return 0;
        return Clear();
        
    }
    public int RemoveItem(ItemContainer wantItem, int amount)
    {
        if (!wantItem) return amount;
        if (GetIsEmpty()) return amount;
        if (item != wantItem) return amount;
        if(amount >= currentStack) return amount - Clear();
        currentStack -= amount;
        return 0;
    }

    public void ExChangeItem(ItemSlot wantSlot)
    {
        if (wantSlot is null) return;
        ItemContainer wasItem = item;
        int wasStack = currentStack;
        item = wantSlot.item;
        currentStack = wantSlot.currentStack;
        wantSlot.item = wasItem;
        wantSlot.currentStack = wasStack;
    }

    internal void LeftClick(ItemSlot wantSlot)
    {
        if(wantSlot is null) return;
        ExChangeItem(wantSlot);
        NoticeChanged();
        wantSlot?.NoticeChanged();
    }
}
