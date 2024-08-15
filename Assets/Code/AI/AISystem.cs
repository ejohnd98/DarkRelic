using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISystem
{
    public static DR_Action DetermineAIAction(DR_GameManager gm, DR_Entity entity){

        AIComponent aiComponent = entity.GetComponent<AIComponent>();

        //TODO: should have a separate function to update AIComponent states/targets/etc (ai may lose target during this if too far away)
        if(!aiComponent.HasTarget()){
            aiComponent.target = DetermineTarget(gm, entity);
        }

        if(aiComponent.HasTarget()){
            DR_Entity target = aiComponent.target;

            //Within melee range:
            if (entity.DistanceTo(target.Position) == 1){
                HealthComponent targetHealth = target.GetComponent<HealthComponent>();
                if (targetHealth.IsAlive()){
                    return new AttackAction(target, entity);
                }
            }

            //Can use ability
            if (entity.GetComponent<AbilityComponent>() is AbilityComponent abilityComponent){
                foreach (DR_Ability ability in abilityComponent.abilities){
                    if (ability.CanBePerformed() && ability.triggeredByPlayer){
                        var abilityAction = new AbilityAction(ability, entity);
                        ActionInput actionInput = abilityAction.GetNextNeededInput();
                        if (actionInput.GiveInput(target.Position) && !abilityAction.RequiresInput()){
                            //ability accepts single position as input, and input is valid
                            return abilityAction;
                        }
                    }
                }
            }

            //TODO: should optimize this to not throw away whole path each time
            //Not within range:

            aiComponent.currentPath = Pathfinding.FindPath(gm.CurrentMap, entity.Position, target.Position);

            if (aiComponent.HasPath()){
                Vector2Int nextPos = aiComponent.currentPath.AdvanceStep();
                if (gm.CurrentMap.CanMoveActor(entity, nextPos)){
                    return new MoveAction(entity, nextPos);
                }
            }

        }

        return new WaitAction(entity);
    }

    public static DR_Entity DetermineTarget(DR_GameManager gm, DR_Entity entity){
        //TODO: elaborate on this
        return MapHelpers.GetClosestEntity(gm, entity, 10);
    }
}
