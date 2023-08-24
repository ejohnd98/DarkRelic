using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryComponent : DR_Component
{
    public int capacity = 0;
    public List<DR_Entity> items;

    public InventoryComponent(int capacity){
        items = new List<DR_Entity>();
        this.capacity = capacity;
    }

    public void RemoveItem(DR_Entity item){
        items.Remove(item);
        //TODO: only do this when the displayed UI matches this one
        UISystem.instance.RefreshInventoryUI();
    }

    public void DropItem(DR_GameManager gm, DR_Entity item){
        items.Remove(item);

        if (gm.CurrentMap.GetItemAtPosition(Entity.Position) != null){
            Debug.LogError("InventoryComponent: can't drop item as there is already an item at " + Entity.Position);
            return;
        }

        gm.CurrentMap.AddItem(item, Entity.Position);

        //TODO: only do this when the displayed UI matches this one
        UISystem.instance.RefreshInventoryUI();
    }

    public DR_Entity GetItem(int index){
        if (index < items.Count){
            return items[index];
        }
        return null;
    }

    public bool AddItem(DR_Entity item){
        if (items.Count + 1 < capacity){
            items.Add(item);

            //TODO: only do this when the displayed UI matches this one
            UISystem.instance.RefreshInventoryUI();
            
            return true;
        }
        return false;
    }
}
