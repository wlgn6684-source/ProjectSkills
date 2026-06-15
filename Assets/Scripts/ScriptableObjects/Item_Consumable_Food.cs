using UnityEngine;

[CreateAssetMenu(fileName = "Item_Consumable_Food", menuName = "Item/Consumable/Food")]
public class Item_Consumable_Food : Item_Consumable
{
    public float hungerChange = 10f;
    public float ThirstyChange = -5.0f;

    public override void OnUse(CharacterBase from, CharacterBase to)
    {
        base.OnUse(from, to);
    }
}
