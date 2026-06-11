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
    public int GetStackable(ItemContainer wantItem) => Containable(wantItem) ? wantItem.maxStack - currentStack : 0;
    public int GetStackable()      => GetStackable(item);
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
        int stackable = Mathf.Min(item.maxStack - currentStack, amount);
        currentStack += stackable;
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

    public int GiveItem(ItemSlot wantSlot) => GiveItem(wantSlot, currentStack);
    
    public int GiveItem(ItemSlot wantSlot, int amount)
    {
        if (wantSlot is null) return amount;
        if(!item) return amount;
        if(currentStack <= 0 || amount <= 0) return amount;

        ItemContainer targetItem = item;
        amount = Mathf.Min(amount, wantSlot.GetStackable(targetItem));
        amount -= RemoveItem(targetItem, amount);
        amount = wantSlot.AddItem(targetItem, amount);

        return amount;
    }

    public void LeftClick(ItemSlot wantSlot)
    {
        if (wantSlot is null) return;
        if (InputManager.IsShift)
        {
            if (wantSlot.GetIsEmpty())
            {
                if (GetIsEmpty()) return;

                else if (wantSlot.Containable(item))
                {
                    GiveItem(wantSlot, Mathf.CeilToInt(currentStack * 0.5f));
                }
            }
            else if (Containable(wantSlot.item))
            {
                wantSlot.GiveItem(this, Mathf.CeilToInt(wantSlot.currentStack * 0.5f));
            }
        }
        else
        {
            if (wantSlot.Containable(item))
            {
                GiveItem(wantSlot);
            }
            else
            {
                ExChangeItem(wantSlot);
            }

        }
        NoticeChanged();
        wantSlot?.NoticeChanged();
    }

    public void RightClick(ItemSlot wantSlot)
    {
        //if (wantSlot == null) return;
        //if (GetIsEmpty()) return;
        //if (!wantSlot.Containable(item)) return;
        //GiveItem(wantSlot, 1);

        if (wantSlot is null) return;
        if (InputManager.IsShift || wantSlot.GetIsEmpty())
        { 
            if(GetIsEmpty()) return;
            if(wantSlot.Containable(item)) GiveItem(wantSlot, 1);
            else return;
        }
        else
        {
            if (Containable(wantSlot.item)) wantSlot.GiveItem(this, 1);
            else return;
        }
        NoticeChanged();
        wantSlot.NoticeChanged();
    }
}

