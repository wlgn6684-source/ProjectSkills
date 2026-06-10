using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Inventory : MonoBehaviour
{
    public static ItemSlot cursorSlot = new ItemSlot();
    // Columns Rows
    //   열     행
    public int columns;
    public int rows;

    ItemSlot[,] slots;

    public void Initialize()
    { 
        slots = new ItemSlot[rows, columns];

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                slots[row, column] = new ItemSlot();
            }
        }
    }

    public void HealPotionPlus(int amount)
    {
       ItemContainer potion = DataManager.LoadDataFile<ItemContainer>("LesserHealPotion");
        AddItem(potion, amount);
    }
    public void HealPotionMinus(int amount)
    {
       ItemContainer potion = DataManager.LoadDataFile<ItemContainer>("LesserHealPotion");
        RemoveItem(potion, amount);
    }

    public void Sort(System.Comparison<ItemContainer> Method)
    { 
        
    }
    public void AutoQuickInsert(Inventory other)
    { }
    public void AutoQuickInsert(Inventory[] other)
    { }

    public bool InsertAll(Inventory other)
    {
        return default;
    }
    public bool InsertAll(Inventory other, ItemContainer target)
    { return default; }

    public void LockSlot(int wantRow, int wantColumn)
    { }

    public void UnLockSlot(int wantRow, int wantColumn)
    { }

    public int CountItem(ItemContainer wantItem)
    {
        return default;
    }
    public int CountItem(ItemContainer wantItem, out List<ItemSlot> returnSlots)
    {   
        returnSlots = default;
        return default;
    }
    public IEnumerable<ItemSlot> GetAllSlot()
    {   // X = 열의 갯수 * R + C

        //ItemSlot[] result = new ItemSlot[slots.Length];

        int height = slots.GetLength(0);
        int width = slots.GetLength(1);

        for (int row = 0; row < height; row++)
        {
            for (int column = 0; column < width; column++)
            {
                if (slots[row, column] is null) continue;
                yield return slots[row, column];
            }
        }
      
    }
    public IEnumerable<ItemSlot> GetAllSlotReverse()
    {
        int height = slots.GetLength(0);
        int width = slots.GetLength(1);

        for (int row = height - 1; row >= 0; row--)
        {
            for (int column = width -1; column >= 0; column--)
            {
                if (slots[row, column] is null) continue;
                yield return slots[row, column];
            }
        }
      
    }

    public ItemSlot FindItem(ItemContainer target)
    { return default; }
    public ItemSlot FindItem(ItemType wantType)
    { return default; }
    public ItemSlot FindItem(int wantRow, int wantColumn)
    {
        if (wantRow < 0 || wantColumn < 0)      return null;
        if (wantRow >= slots.GetLength(0))      return null;
        if (wantColumn >= slots.GetLength(1))   return null;
        return slots[wantRow, wantColumn]; 
    }
    public ItemSlot FindItem(string containWord)
    { return default; }

    public IEnumerable<ItemSlot> FindFirstEmptySlot()
    {
        foreach (ItemSlot currentSlot in GetAllSlot())
        { 
                if(currentSlot.GetIsEmpty()) yield return currentSlot;
        }
    }
    public IEnumerable<ItemSlot> FindLastEmptySlot()
    {
        foreach (ItemSlot currentSlot in GetAllSlotReverse())
        {
            if (currentSlot.GetIsEmpty()) yield return currentSlot;
        }
    }
    public IEnumerable<ItemSlot>FindFirstItem(ItemContainer target)
    {
        foreach (ItemSlot currentSlot in GetAllSlot())
            if (currentSlot.GetItem() == target) yield return currentSlot;
    }
    public IEnumerable<ItemSlot>FindLastItem(ItemContainer target)
    {
        foreach (ItemSlot currentSlot in GetAllSlotReverse())
        {
            if (currentSlot.GetItem() == target) yield return currentSlot;
        }
    }
    public int AddItem(ItemContainer wantItem, int amount = 1)
    {
        
        amount = AddItemOnExistSlots(wantItem, amount);
        if (amount <= 0) return 0;
        return AddItemOnEmptySlots(wantItem, amount);
    
    }
    public int AddItemOnExistSlots(ItemContainer wantItem, int amount)
    {
        foreach (ItemSlot currentSlot in FindFirstItem(wantItem))
        {
            if (amount <= 0) return 0;
            amount = currentSlot.AddItem(wantItem, amount);
            currentSlot.NoticeChanged();
        }
        return amount;
    }

    public int AddItemOnEmptySlots(ItemContainer wantItem, int amount)
    {
        foreach (ItemSlot currentSlot in FindFirstEmptySlot())
        {
            if (amount <= 0) return 0;
            amount = currentSlot.AddItem(wantItem, amount);
            currentSlot.NoticeChanged();
        }
        return amount;
    }

    public int AddItemToLocation(ItemContainer wantItem, int amount, int row, int column)
    {
        return default;
    }

    public ItemSlot[,] Clear() 
    {
        ItemSlot[,] origin = slots;
        Initialize();
        return origin;
    }
    public int RemoveItem(System.Predicate<ItemContainer> condition) 
    {
        return default;
    }
    public int RemoveItem(ItemContainer wantItem) 
    {
        int result = 0;
        foreach (ItemSlot currentSlot in FindFirstItem(wantItem))
        {
            result+= currentSlot.RemoveItem(wantItem);
            currentSlot.NoticeChanged();
        }
        return result;
    }
    public int RemoveItem(ItemContainer wantItem, int amount) 
    {
        foreach (ItemSlot currentSlot in FindFirstItem(wantItem))
        {
            if (amount <= 0) return 0;
            amount = currentSlot.RemoveItem(wantItem, amount);
            currentSlot.NoticeChanged();
        }
        return amount;
    }
    public int RemoveItemOnExistSlots(ItemContainer wantItem, int amount)
    {
        return default;
    }
    public int RemoveItemFromLocation(int row, int column)
    {
        return default;
    }
    public int RemoveItemFromLocation(int row, int column, int amount)
    {
        return default;
    }

    public void MoveItem(int startRow, int startColumn, Inventory targetInventory, int targetRow, int targetColumn, int amount = -1)
    { 
    
    }
    public void ExChangeItem(int startRow, int startColumn, int targetRow, int targetColumn)
    { 
        ExChangeItem(startRow, startColumn, this, targetRow, targetColumn);

    }
    public void ExChangeItem(int startRow, int startColumn, ItemSlot targetSlot)
    {
        if (targetSlot is null) return;
        ItemSlot first = FindItem(startRow, startColumn);
        if (first is null) return;
        first.ExChangeItem(targetSlot);
        first.NoticeChanged();
        targetSlot.NoticeChanged();
    }
    public void ExChangeItem(int startRow, int startColumn, Inventory targetInventory, int targetRow, int targetColumn)
    {
        ItemSlot first = FindItem(startRow, startColumn);
        if(first is null) return;
        if (!targetInventory) return;
        ItemSlot second = targetInventory.FindItem(targetRow, targetColumn);
        if(second is null) return;

        first.ExChangeItem(second);
        first.NoticeChanged();
        second.NoticeChanged();

    }
    public bool UseItem(ItemContainer target)
    {
        return default;
    }
    public bool UseItem(int row, int column)
    {
        return default;
    }


}
