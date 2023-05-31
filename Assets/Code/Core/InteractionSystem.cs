using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionSystem
{
    public static List<DR_Action> GetPotentialActions(DR_Entity entity, DR_Map map, Vector2Int pos){
        List<DR_Action> actionList = new List<DR_Action>();

        DR_Cell targetCell = map.Cells[pos.y, pos.x];

        if (pos == entity.Position){
            actionList.Add(new WaitAction());
            return actionList;
        }

        if (targetCell.Actor != null){
            HealthComponent target = targetCell.Actor.GetComponent<HealthComponent>();
            if (target != null){
                actionList.Add(new AttackAction(target, entity));
            }
        }

        if (targetCell.IsTraversable()){
            actionList.Add(new MoveAction(pos));
        }

        return actionList;
    }
}
