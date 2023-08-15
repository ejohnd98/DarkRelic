using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConsumableComponent : DR_Component {

    public bool RemoveAfterUse = true;

    public override bool Trigger(DR_GameManager gm, DR_Entity user, DR_Entity target){
        bool wasUsed = Consume(gm, user, target);

        InventoryComponent inventory = user.GetComponent<InventoryComponent>();
        ItemComponent itemComp = Entity.GetComponent<ItemComponent>();
        if (wasUsed && inventory != null){
            inventory.RemoveItem(itemComp.ownerItem);
            return true;
        }

        return false;
    }

    public virtual bool Consume(DR_GameManager gm, DR_Entity user, DR_Entity target){
        Debug.LogError("Consume not implemented!");
        return false;
    }

    public DR_Action GetAction(DR_Entity user, DR_Entity target){
        DR_Item OwningItem = (DR_Item)Entity;
        if (OwningItem == null){
            Debug.LogException(new System.Exception("ConsumableComponent on non-item entity"));
        }
        return new ItemAction(OwningItem, user, target);
    }
}

public class HealingConsumableComponent : ConsumableComponent {

    public int healAmount = 4;

    public HealingConsumableComponent(int amount){
        healAmount = amount;
    }

    public override bool Consume(DR_GameManager gm, DR_Entity user, DR_Entity target)
    {
        HealthComponent health= target.GetComponent<HealthComponent>();
        if (health != null){
            return (health.Heal(healAmount) != 0);
        }
        return false;
    }
}

public class MagicConsumableComponent : ConsumableComponent {

    public int damageAmount = 5;
    public int maxRange = 6;

    public MagicConsumableComponent(int amount){
        damageAmount = amount;
    }

    public override bool Consume(DR_GameManager gm, DR_Entity user, DR_Entity target)
    {
        //Picks closest entity as target
        DR_Entity closestTarget = null;
        AlignmentComponent userAlignment = user.GetComponent<AlignmentComponent>();
        if (userAlignment == null){
            Debug.LogError("MagicDamageComponent.Consume: user (" + user.Name + ") alignment component is NULL!");
            return false;
        }

        int closestDist = -1;
        foreach (DR_Entity entity in gm.CurrentMap.Entities){

            AlignmentComponent alignment = entity.GetComponent<AlignmentComponent>();
            if (alignment != null && !alignment.IsFriendly(userAlignment)){
                
                int dist = entity.DistanceTo(user.Position);
                if (dist > maxRange){
                    continue;
                }

                if (!gm.CurrentMap.IsVisible[entity.Position.y, entity.Position.x]){
                    continue;
                }

                if (closestTarget == null || dist < closestDist){
                    closestDist = dist;
                    closestTarget = entity;
                }
            }
        }

        if (closestTarget != null){
            HealthComponent health = closestTarget.GetComponent<HealthComponent>();
            if (health != null){
                DamageSystem.HandleAttack(gm, health, damageAmount);
                return true;
            }
        }
        return false;
    }
}