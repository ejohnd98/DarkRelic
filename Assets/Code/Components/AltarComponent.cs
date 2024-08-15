using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum AltarType
{
    HEALTH,
    ITEM
}

public class AltarComponent : DR_Component
{
    [Copy]
    public AltarType altarType;

    [DoNotSerialize]
    public Content itemAltarContent;

    public int GetBloodCost(DR_Entity interactor = null){
        DR_Entity entity = interactor ?? DR_GameManager.instance.GetPlayer();
        if (entity == null){
            return -1;
        }

        switch (altarType){
            case AltarType.HEALTH:{
                HealthComponent healthComp = entity.GetComponent<HealthComponent>();
                InventoryComponent inventory = entity.GetComponent<InventoryComponent>();
                return Mathf.Min(healthComp.maxHealth - healthComp.currentHealth, inventory.blood);
            }
            case AltarType.ITEM:{
                // TODO: scale with depth and item multiplier (some relics should cost more)
                float scaledCost = 10 * Mathf.Pow(1.6f, DR_GameManager.instance.CurrentDungeon.mapIndex);
                return ((int)(scaledCost / 5)) * 5; //multiple of 5
            }
            default:{
                return -1;
            }
        }
    }

    public override string GetDetailsDescription()
    {
        switch (altarType){
            case AltarType.HEALTH:
                return "Fully replenishes health for an equal blood cost (" + GetBloodCost() + ").\n\nInsufficient blood will partially restore health.";
            
            case AltarType.ITEM:
                return "Grants " + itemAltarContent.contentName + " for a blood cost of " + GetBloodCost() + ".";
            default:
                return "unknown altar type!";
        }
    }
}
