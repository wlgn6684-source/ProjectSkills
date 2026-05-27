using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
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

    public void HealPotionPlus()
    {
       ItemContainer potion = DataManager.LoadDataFile<ItemContainer>("LesserHealPotion");
        AddItem(potion);
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
    public ItemSlot[] GetAllSlot()
    {   // X = 열의 갯수 * R + C

        ItemSlot[] result = new ItemSlot[slots.Length];

        int height = slots.GetLength(0);
        int width = slots.GetLength(1);

        for (int row = 0; row < height; row++)
        {
            for (int column = 0; column < width; column++)
            {
                result[width * row + column] = slots[row, column];
            }
        }
        return result;
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

    public ItemSlot FindFirstEmptySlot()
    { return default; }
    public ItemSlot FindLastEmptySlot()
    { return default; }
    public ItemSlot FindFirstItem(ItemContainer target)
    { return default; }
    public ItemSlot FindLastItem(ItemContainer target)
    { return default; }
    public int AddItem(ItemContainer wantItem, int amount = 1)
    {
        slots[0, 0].AddItem(wantItem, amount);
        return default;
    }
    public int AddItemOnExistSlots(ItemContainer wantItem, int amount)
    {
        return default;
    }

    public int AddItemOnEmptySlots(ItemContainer wantItem, int amount)
    {
        return default;
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
        return default;
    }
    public int RemoveItem(ItemContainer wantItem, int amount) 
    {
        return default;
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

    public bool MoveItem(int startRow, int startColumn, Inventory targetInventory, int targetRow, int targetColumn, int amount = -1)
    {
        return default;
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
