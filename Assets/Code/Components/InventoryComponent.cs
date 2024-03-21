using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryComponent : DR_Component
{
    [Copy]
    public int capacity = 0;
    [Copy]
    public int maxEquips = 0;
    
    public int equippedItems = 0;
    public List<DR_Entity> items = new List<DR_Entity>();
    
    public Dictionary<RelicType, int> RelicInventory = new ();

    public InventoryComponent(){}

    public InventoryComponent(int capacity, int maxEquips = 10){
        this.capacity = capacity;
        this.maxEquips = maxEquips;
    }

    public void RemoveItem(DR_Entity item){
        items.Remove(item);
        //TODO: only do this when the displayed UI matches this one
        UISystem.instance.RefreshInventoryUI();
    }

    public bool EquipItem(DR_Entity item){
        EquippableComponent equippable = item.GetComponent<EquippableComponent>();
        if (equippable != null && equippedItems < maxEquips){
            equippable.isEquipped = true;
            equippedItems++;
            UISystem.instance.RefreshInventoryUI();
            return true;
        }
        return false;
    }

    public bool UnequipItem(DR_Entity item){
        EquippableComponent equippable = item.GetComponent<EquippableComponent>();
        if (equippable != null){
            equippable.isEquipped = false;
            equippedItems--;
            UISystem.instance.RefreshInventoryUI();
            return true;
        }
        return false;
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
        if (item.GetComponent<RelicComponent>() is RelicComponent relicComponent)
        {
            RelicType relicType = relicComponent.relicType;
            if (RelicInventory.ContainsKey(relicType))
            {
                RelicInventory[relicType] += 1;
            }
            else
            {
                RelicInventory[relicType] = 1;
            }
            UISystem.instance.RefreshInventoryUI();
            return true;
        }
        
        
        if (items.Count + 1 < capacity){
            items.Add(item);

            //TODO: only do this when the displayed UI matches this one
            UISystem.instance.RefreshInventoryUI();
            
            return true;
        }
        return false;
    }
}
