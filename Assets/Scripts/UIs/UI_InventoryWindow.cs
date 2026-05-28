using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_InventoryWindow : OpenableUIBase
{
    [SerializeField] Inventory targetInventory;
    [SerializeField] LayoutGroup layout;
    [SerializeField] string itemSlotPrefabName;

    public override void Registration(UIManager manager)
    {
        base.Registration(manager);
        targetInventory?.Initialize();
        ConnectInventory(targetInventory);
    }

    private void ConnectInventory(Inventory newInventory)
    {
        if(!newInventory) return;
        targetInventory = newInventory;

        if (!layout) return;

        if (layout is GridLayoutGroup asGridLayout)
        {
            asGridLayout.constraintCount = targetInventory.columns;
        }


        foreach(ItemSlot currentSlot in newInventory.GetAllSlot())
        {
            if (currentSlot is null) continue;
            GameObject instance = ObjectManager.CreateObject(itemSlotPrefabName, layout.transform);
            if (!instance) continue;
            if (instance.TryGetComponent(out UI_ItemSlotInfo createSlot))
            {
                createSlot.ConnectSlot(currentSlot);
            }
        }
    }

    public override void UnRegistration(UIManager manager)
    {
        base.UnRegistration(manager);
        DisconnectInventory();
    }

    public void DisconnectInventory()
    {
        if (!layout) return;
        // foreach for while
        while (layout.transform.childCount > 0)
        { 
            Transform targetChild = layout.transform.GetChild(0);
            targetChild.SetParent(null);
            ObjectManager.DestroyObject(targetChild.gameObject);
        }
    }
}
