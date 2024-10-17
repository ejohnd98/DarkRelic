using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum AltarType
{
    HEALTH_ALTAR,
    ITEM_ALTAR,
    CURSED_ALTAR,
    CHEST
}

public class AltarComponent : DR_Component
{
    [Copy]
    public AltarType altarType;

    [Copy]
    public DR_Ability.AbilityType chestType;

    [Copy]
    public Sprite unusedSprite, usedSprite;

    [DoNotSerialize]
    public AbilityContent altarAbilityContent;

    [DoNotSerialize]
    public bool interactable = true;

    public Action<DR_Event> OnAltarUsed;

    public int GetBloodCost(DR_Entity interactor = null){
        DR_Entity entity = interactor ?? DR_GameManager.instance.GetPlayer();
        if (entity == null){
            return -1;
        }

        switch (altarType){
            case AltarType.HEALTH_ALTAR:{
                HealthComponent healthComp = entity.GetComponent<HealthComponent>();
                InventoryComponent inventory = entity.GetComponent<InventoryComponent>();
                return Mathf.Min(healthComp.maxHealth - healthComp.currentHealth, inventory.blood);
            }
            case AltarType.ITEM_ALTAR:{
                // TODO: scale with depth and item multiplier (some relics should cost more)
                float scaledCost = 10 * Mathf.Pow(1.6f, DR_GameManager.instance.CurrentDungeon.mapIndex);
                return ((int)(scaledCost / 5)) * 5; //multiple of 5
            }
            case AltarType.CURSED_ALTAR:{
                return 0;
            }
            case AltarType.CHEST:{
                // TODO: scale with depth and item multiplier (some relics should cost more)
                float scaledCost = 10 * Mathf.Pow(1.6f, DR_GameManager.instance.CurrentDungeon.mapIndex);
                return ((int)(scaledCost / 5)) * 5; //multiple of 5
            }
            default:{
                return -1;
            }
        }
    }

    public bool Interact(DR_Entity other){

        if (!interactable){
            return false;
        }

        int bloodCost = GetBloodCost(other);
        HealthComponent healthComponent = other.GetComponent<HealthComponent>();
        InventoryComponent inventoryComponent = other.GetComponent<InventoryComponent>();
        AbilityComponent abilityComponent = other.GetComponent<AbilityComponent>();

        if (inventoryComponent.blood < bloodCost){
            return false;
        }
        
        switch (altarType){
            case AltarType.HEALTH_ALTAR:
            {
                var healthRestored = healthComponent.Heal(bloodCost);
                inventoryComponent.SpendBlood(healthRestored);
                if (healthRestored == 0){
                    return false;
                }
                SoundSystem.instance.PlaySound("altar");
                break;
            }
            
            case AltarType.ITEM_ALTAR:
            {
                inventoryComponent.SpendBlood(bloodCost);
                abilityComponent.AddAbilityFromContent(altarAbilityContent);
                interactable = false;
                SoundSystem.instance.PlaySound("relic");
                break;
            }
            case AltarType.CURSED_ALTAR:
            {
                abilityComponent.AddAbilityFromContent(altarAbilityContent);
                interactable = false;
                SoundSystem.instance.PlaySound("cursedRelic");
                break;
            }
            case AltarType.CHEST:
            {
                inventoryComponent.SpendBlood(bloodCost);
                abilityComponent.AddAbilityFromContent(altarAbilityContent);

                interactable = false;
                var spriteComp = Entity.GetComponent<SpriteComponent>();
                spriteComp.Sprite = usedSprite;
                SoundSystem.instance.PlaySound("relic");
                break;
            }
            default:
            break;
        }
        OnAltarUsed?.Invoke(new(){owner = other});

        return true;
    }

    public override string GetDetailsDescription()
    {
        switch (altarType){
            case AltarType.HEALTH_ALTAR:
                return "Fully replenishes health for an equal blood cost (" + GetBloodCost() + ").\n\nInsufficient blood will partially restore health.";
            case AltarType.ITEM_ALTAR:
                return "Grants " + altarAbilityContent.contentName + " for a blood cost of " + GetBloodCost() + ".";
            case AltarType.CURSED_ALTAR:
                return "Grants " + altarAbilityContent.contentName;
            case AltarType.CHEST:
                return "Spend " + GetBloodCost() + " blood to open!";
            default:
                return "unknown altar type!";
        }
    }
}
