using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISystem
{
    public static DR_Action DetermineAIAction(DR_Entity entity, DR_Map map){

        // get list of all actions, then prioritize (similar to what happens for player)
        // but first, figure out the current state (can see target, has target at all, in attack range, etc)

        // will need to do pathfinding first (and create class to store path?)

        //Temp:
        DR_Entity Target = DR_GameManager.instance.GetPlayer();
        if (Target != null && Target.GetComponent<HealthComponent>().IsAlive()){
            Vector2Int playerPosDiff = Target.Position - entity.Position;
            if (Mathf.Abs(playerPosDiff.x) + Mathf.Abs(playerPosDiff.y) == 1){
                return new AttackAction(Target.GetComponent<HealthComponent>(), entity);
            }
        }

        return new WaitAction(entity);
    }
}
