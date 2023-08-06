using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISystem
{
    public static DR_Action DetermineAIAction(DR_Entity entity, DR_Map map){

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
