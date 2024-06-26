using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: something other than this
public class HeldRelic
{
    public DR_Entity relicEntity = null;
    public int count = 0;
}

public class InventoryComponent : DR_Component
{
    [Copy]
    public int capacity = 0;
    [Copy]
    public int maxEquips = 0;
    [Copy]
    public bool canCollectBlood = false;
    [Copy]
    public int blood = 0;
    
    public int equippedItems = 0;
    public List<DR_Entity> items = new List<DR_Entity>();
    
    public Dictionary<RelicType, HeldRelic> RelicInventory = new ();

    public InventoryComponent(){}

    public InventoryComponent(int capacity, int maxEquips = 10){
        this.capacity = 999;//capacity;
        this.maxEquips = maxEquips;
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

    public bool AddItemFromContent(Content content){
        DR_Entity newItem = EntityFactory.CreateEntityFromContent(content);
        return AddItem(newItem);
    }

    public bool AddItem(DR_Entity item){
        if (item.GetComponent<RelicComponent>() is RelicComponent relicComponent)
        {
            SoundSystem.instance.PlaySound("relic");
            RelicType relicType = relicComponent.relicType;
            if (RelicInventory.ContainsKey(relicType))
            {
                RelicInventory[relicType].count += 1;
            }
            else
            {
                RelicInventory[relicType] = new HeldRelic(){
                    relicEntity = item,
                    count = 1
                };
            }

            // TODO: handle unequip too
            if (Entity.GetComponent<AbilityComponent>() is AbilityComponent abilityComponent){
                foreach(var ability in relicComponent.grantedAbilities){
                    abilityComponent.AddAbilityFromContent(ability);
                }
            }

            Entity.GetComponent<LevelComponent>().UpdateStats();
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

    public void AddBlood(int amount)
    {
        blood += amount;
        SoundSystem.instance.PlaySound("addBlood");
        UISystem.instance.RefreshInventoryUI();
    }

    public void SpendBlood(int amount)
    {
        blood -= amount;
        UISystem.instance.RefreshInventoryUI();
    }
}
