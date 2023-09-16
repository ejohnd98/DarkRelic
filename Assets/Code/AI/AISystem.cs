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

            //Within range:
            if (entity.DistanceTo(target.Position) == 1){
                HealthComponent targetHealth = target.GetComponent<HealthComponent>();
                if (targetHealth.IsAlive()){
                    return new AttackAction(targetHealth, entity);
                }
            }

            //Not within range:
            
            if (!aiComponent.HasPath()){
                aiComponent.currentPath = Pathfinding.FindPath(gm.CurrentMap, entity.Position, target.Position);
            }
            if (aiComponent.HasPath()){
                Vector2Int nextPos = aiComponent.currentPath.AdvanceStep();
                return new MoveAction(entity, nextPos);
            }

        }

        //branch here:
        //if no target: return wait action (later, wander or move to last known target location)
        //if target:
        //  if within attack range, (create skeleton function for that) attack.
        //  otherwise, compute path (create pathfinding class that returns a result class)
        //  then move towards target

        //Temp:

        return new WaitAction(entity);
    }

    public static DR_Entity DetermineTarget(DR_GameManager gm, DR_Entity entity){
        //TODO: elaborate on this
        return MapHelpers.GetClosestEntity(gm, entity, 10);
    }
}
