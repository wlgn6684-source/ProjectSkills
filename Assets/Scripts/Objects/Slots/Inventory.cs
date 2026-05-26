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
        slots = new ItemSlot[columns, rows];
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

    public void LockSlot(int wantRows, int wantColumn)
    { }

    public void UnLockSlot(int wantRows, int wantColumn)
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

    public ItemSlot FindItem(ItemContainer target)
    { return default; }
    public ItemSlot FindItem(ItemType wantType)
    { return default; }
    public ItemSlot FindItem(int wantRows, int wantColumn)
    { return default; }
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
