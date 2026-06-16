using UnityEngine;

public enum ItemType
{ 
    Miscellaneous = 50, 
    Quest = 55, 
    Consumable = 100, 
    Material = 150, 
    Equipement = 500, 
    Important = 600,
    Length
}

[CreateAssetMenu(fileName = "ItemContainer", menuName = "Item/ItemBase")]
public class ItemContainer : InfoContainer
{
    [Header("Item Base Info")]
    public int id;
    [Space]
    [Header("Item Detail")]
    public ItemType Type;
    public int maxStack;

    public virtual int CompareByType(ItemContainer other)
    {
        if (other == null) return 1;
        int result = Type - other.Type;
        if (result != 0) return result;
        return id - other.id;
    }
}
