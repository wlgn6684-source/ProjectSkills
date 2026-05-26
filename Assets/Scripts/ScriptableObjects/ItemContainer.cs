using UnityEngine;

public enum ItemType
{ 
    Equipement, Consumable, Material, Miscellaneous, Quest, Important,
    Length
}

[CreateAssetMenu(fileName = "ItemContainer", menuName = "Item/ItemBase")]
public class ItemContainer : InfoContainer
{
    public ItemType Type;
    public int maxStack;
    public float weight;
}
