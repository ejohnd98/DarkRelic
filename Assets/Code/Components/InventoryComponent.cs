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
    public Action<DR_Event> OnPickedUpBlood;
    public Action<DR_Event> OnSpentBlood;
    
    [Copy]
    public bool canCollectBlood;
    [Copy]
    public int blood;
    
    public int equippedItems = 0;
    public List<DR_Entity> items = new List<DR_Entity>();

    public InventoryComponent(){}

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

            // TODO: handle unequip too
            if (Entity.GetComponent<AbilityComponent>() is AbilityComponent abilityComponent){
                foreach(var ability in relicComponent.grantedAbilities){
                    abilityComponent.AddAbilityFromContent(ability);
                }
            }

            Entity.GetComponent<LevelComponent>().UpdateStats();
        }
        
        bool addedItem = false;
        foreach (var existingItem in items){
            if (existingItem.contentGuid == item.contentGuid){
                existingItem.GetComponent<ItemComponent>().count++;
                addedItem = true;
            }
        }

        if (!addedItem){
            items.Add(item);
        }

        //TODO: only do this when the displayed UI matches this one?
        UISystem.instance.RefreshInventoryUI();
        return true;
    }

    public void AddBlood(int amount)
    {
        BloodChangeEvent bloodEvent = new();
        bloodEvent.oldBlood = blood;

        blood += amount;
        SoundSystem.instance.PlaySound("addBlood");
        UISystem.instance.RefreshInventoryUI();

        bloodEvent.newBlood = blood;
        bloodEvent.bloodDelta = amount;
        OnPickedUpBlood?.Invoke(bloodEvent);
    }

    public void SpendBlood(int amount)
    {
        BloodChangeEvent bloodEvent = new();
        bloodEvent.oldBlood = blood;

        blood -= amount;
        UISystem.instance.RefreshInventoryUI();

        bloodEvent.newBlood = blood;
        bloodEvent.bloodDelta = amount;
        OnSpentBlood?.Invoke(bloodEvent);
    }
}
