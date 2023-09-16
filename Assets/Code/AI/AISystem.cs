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

        //branch here:
        //if no target: return wait action (later, wander or move to last known target location)
        //if target:
        //  if within attack range, (create skeleton function for that) attack.
        //  otherwise, compute path (create pathfinding class that returns a result class)
        //  then move towards target

        //Temp:
        DR_Entity Target = aiComponent.target;
        if (Target != null && Target.GetComponent<HealthComponent>().IsAlive()){
            Vector2Int playerPosDiff = Target.Position - entity.Position;
            if (Mathf.Abs(playerPosDiff.x) + Mathf.Abs(playerPosDiff.y) == 1){
                return new AttackAction(Target.GetComponent<HealthComponent>(), entity);
            }
        }

        return new WaitAction(entity);
    }

    public static DR_Entity DetermineTarget(DR_GameManager gm, DR_Entity entity){
        //TODO: elaborate on this
        return MapHelpers.GetClosestEntity(gm, entity, 10);
    }
}
