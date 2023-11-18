using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConsumableComponent : DR_Component {

    public bool RemoveAfterUse = true;

    public override bool Trigger(DR_GameManager gm, DR_Entity user, DR_Entity target){
        bool wasUsed = Consume(gm, user, target);

        InventoryComponent inventory = user.GetComponent<InventoryComponent>();
        if (wasUsed && inventory != null){
            inventory.RemoveItem(Entity);
            return true;
        }

        return false;
    }

    public virtual bool Consume(DR_GameManager gm, DR_Entity user, DR_Entity target){
        Debug.LogError("Consume not implemented!");
        return false;
    }

    public DR_Action GetAction(DR_Entity user, DR_Entity target){
        DR_Entity OwningItem = (DR_Entity)Entity;
        if (OwningItem == null){
            Debug.LogException(new System.Exception("ConsumableComponent on non-item entity"));
        }
        return new ItemAction(OwningItem, user, target);
    }

    public virtual string GetDescription(){
        return "";
    }
}

public class HealingConsumableComponent : ConsumableComponent {

    public int healAmount = 4;

    public HealingConsumableComponent(){}

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

    public override string GetDescription(){
        return "Restores " + healAmount + " health";
    }
}

public class MagicConsumableComponent : ConsumableComponent {

    public int damageAmount = 5;
    public int maxRange = 6;

    //todo: figure out better spot for these
    public Color color = Color.white;
    public Sprite projectileSprite;

    //todo: make enum of targeting types?
    public bool targetClosest = false;

    public MagicConsumableComponent(){}

    public MagicConsumableComponent(int amount){
        damageAmount = amount;
    }

    public Vector2Int GetTargetPosition(DR_GameManager gm, DR_Entity user, DR_Entity target){
        DR_Entity chosenTarget = null;
        
        if (targetClosest){
            AlignmentComponent userAlignment = user.GetComponent<AlignmentComponent>();
            if (userAlignment == null){
                Debug.LogError("MagicDamageComponent.Consume: user (" + user.Name + ") alignment component is NULL!");
                return Vector2Int.zero;
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

                    if (chosenTarget == null || dist < closestDist){
                        closestDist = dist;
                        chosenTarget = entity;
                    }
                }
            }
        }else{
            chosenTarget = target;
        }

        if (chosenTarget != null){
            return chosenTarget.Position;
        }
        return Vector2Int.zero;
    }

    public override bool Consume(DR_GameManager gm, DR_Entity user, DR_Entity target)
    { 
        DR_Entity chosenTarget = null;

        //Picks closest entity if target is null
        if (targetClosest){
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

                    if (chosenTarget == null || dist < closestDist){
                        closestDist = dist;
                        chosenTarget = entity;
                    }
                }
            }
        }else{
            chosenTarget = target;
        }

        if (chosenTarget != null){
            HealthComponent health = chosenTarget.GetComponent<HealthComponent>();
            if (health != null){
                DamageSystem.HandleAttack(gm, user, health, damageAmount);
                return true;
            }
        }
        return false;
    }

    public override string GetDescription(){
        return "Targets " + (targetClosest? "closest enemy" : "selected enemy") + "\nDeals " + damageAmount + " damage";
    }
}