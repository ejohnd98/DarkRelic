using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DR_EffectBase
{
    public enum AbilityType{
        Common,
        Rare,
        Unholy,
        Cursed
    }

    public AbilityType abilityType;
    public bool triggeredByPlayer = true;
    public string abilityName = "";
    public Sprite sprite;
    public List<ActionInput> actionInputs;
    public List<DR_Entity> relatedEntities = new();

    public int count = 1;
    public int baseBloodCost = 0;

    //TODO: reflect this in UI and ability SO
    public int cooldownLength = 0; //represents cooldown turns (1 would mean you can use every other turn at most
    private int cooldown = 0;

    public DR_Entity owner;
    public string contentGuid = "";

    public virtual void OnAdded(){
        // Owner is guaranteed to be set here
        // Should only be for init stuff and not every time this ability is picked up
    }

    public virtual void TickCooldown(){
        if (cooldown > 0){
            Debug.Log("Tick cooldown on " + owner.Name + ": " + abilityName + " (" + cooldown + "->"+ (cooldown-1) +")");
            cooldown--;
        }
    }

    public virtual bool CanBePerformed(){
        if (cooldown > 0){
            return false;
        }

        if (owner.GetComponent<AIComponent>() is AIComponent aiComp 
            && aiComp.ignoreAbilityBloodCost){
            return true;
        }
        int bloodCost = GetBloodCost();
        if (bloodCost > 0){
            if (!owner.HasComponent<InventoryComponent>()){
                Debug.LogError(owner.Name + " tried to use ability ("+ abilityName +") that requires blood, but has no inventory component!");
                return false;
            }
            return owner.GetComponent<InventoryComponent>().blood + owner.GetComponent<HealthComponent>().currentHealth >= bloodCost;
        }
        return true;
    }

    public void Trigger(DR_Event e){
        if (owner.GetComponent<AIComponent>() is AIComponent aiComp 
            && aiComp.ignoreAbilityBloodCost){

        }else{
            int bloodCost = GetBloodCost();
            if (bloodCost > 0){
                var inventory = owner.GetComponent<InventoryComponent>();
                int bloodToUse = Mathf.Min(inventory.blood, bloodCost);
                inventory.SpendBlood(bloodToUse);
                if (bloodCost - bloodToUse > 0){
                    owner.GetComponent<HealthComponent>().TakeDamage(bloodCost - bloodToUse);
                    if (owner.GetComponent<HealthComponent>().currentHealth <= 0){
                        Debug.Log("Player tried to use too much blood!");
                    }
                }
            }
        }
        
        if (cooldownLength != 0){
            cooldown = cooldownLength + 1;
        }
        OnTrigger(e);
    }

    public virtual void ApplyStatModifiers(StatsModifier statsModifier){
    }

    protected virtual void OnTrigger(DR_Event e){
    }

    public void ResetInputs(){
        actionInputs = new();
        relatedEntities = new();
        SetupInputs();
    }

    public virtual List<DR_Entity> GetRelatedEntities(){
        return relatedEntities;
    }

    protected virtual void SetupInputs(){
    }

    public virtual int GetBloodCost(){
        return baseBloodCost;
    }

    public virtual string GetFormattedDescription(){
        //TODO: get a list of things to insert?
        return "Description not filled out!";
    }

    //TODO: need to think about how this can be driven by scriptable objects as that's where the sprite, name, and description would be
    // Don't necessarily want to make some elaborate generic system though, but can maybe just specify those things, and then see
    // if a dropdown can be created with the ability child class
    // Then when creating the ability at runtime it will just assign the data from the scriptable object to the ability instance

    // Where do entity specific events live?
    // They shouldn't necessarily live on DR_Entity
    // Perhaps they are added to each individual component?
    // What about stuff not tied to components, such as door opening?
    
    // Possibly it does live on the DR_Entity, and then the components trigger them
    // Then things can subscribe to them and they can be triggered without worry.

    // CURRENT TODO:
    // Move ability to scriptable object (initially can be default type?)
    // Create an ability action for those which are player triggered. Can be pretty barebones, but will have the "waiting for input" stage if needed

    // Future:
    // Have relic grant ability
    // Add events for basic stuff on Entity and some way for abilities to subscribe (or just hardcode that)
}
