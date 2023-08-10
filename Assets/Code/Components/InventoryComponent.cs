using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryComponent : DR_Component
{
    public int capacity = 0;
    public List<DR_Item> items;

    public InventoryComponent(int capacity){
        items = new List<DR_Item>();
        this.capacity = capacity;
    }

    public void RemoveItem(DR_Item item){
        items.Remove(item);
        //TODO: only do this when the displayed UI matches this one
        UISystem.instance.RefreshInventoryUI();
    }

    public void DropItem(DR_GameManager gm, DR_Item item){
        items.Remove(item);

        if (gm.CurrentMap.GetItemAtPosition(Entity.Position) != null){
            Debug.LogError("InventoryComponent: can't drop item as there is already an item at " + Entity.Position);
            return;
        }

        gm.CurrentMap.AddItem(item, Entity.Position);

        //TODO: only do this when the displayed UI matches this one
        UISystem.instance.RefreshInventoryUI();
    }

    public DR_Item GetItem(int index){
        if (index < items.Count){
            return items[index];
        }
        return null;
    }

    public bool AddItem(DR_Item item){
        if (items.Count + 1 < capacity){
            items.Add(item);

            //TODO: only do this when the displayed UI matches this one
            UISystem.instance.RefreshInventoryUI();
            
            return true;
        }
        return false;
    }
}
