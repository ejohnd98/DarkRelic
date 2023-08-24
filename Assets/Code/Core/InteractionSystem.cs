using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionSystem
{
    public static List<DR_Action> GetPotentialActions(DR_Entity entity, DR_Map map, Vector2Int pos, KeyCode key){
        List<DR_Action> actionList = new List<DR_Action>();

        DR_Cell targetCell = map.Cells[pos.y, pos.x];

        if (key == KeyCode.Space){
            actionList.Add(new WaitAction(entity, true));
            return actionList;
        }

        if (key == KeyCode.G){
            if (targetCell.Item != null){
                actionList.Add(new PickupAction(targetCell.Item, entity));
            }
            return actionList;
        }

        bool pressedNumKey = false;
        for(int i = 0; i < DR_GameManager.NumberKeys.Length; i++){
            if (key == DR_GameManager.NumberKeys[i]){
                InventoryComponent inventory = entity.GetComponent<InventoryComponent>();
                if (inventory != null){
                    DR_Entity item = inventory.GetItem(i);
                    if (item != null){
                        actionList.Add(new ItemAction(item, entity, entity));
                        return actionList;
                    }
                }
                pressedNumKey = true;
            }
        }
        if (pressedNumKey){
            return actionList;
        }

        if (targetCell.Actor != null){
            HealthComponent target = targetCell.Actor.GetComponent<HealthComponent>();
            if (target != null){
                actionList.Add(new AttackAction(target, entity));
            }
        }

        if (!targetCell.BlocksMovement()){
            actionList.Add(new MoveAction(entity, pos));
        }

        if (targetCell.Prop != null){
            DoorComponent door = targetCell.Prop.GetComponent<DoorComponent>();
            if (door != null){
                actionList.Add(new DoorAction(door, entity));
            }

            StairComponent stairs = targetCell.Prop.GetComponent<StairComponent>();
            if (stairs != null){
                actionList.Add(new StairAction(entity, stairs));
            }
        }

        return actionList;
    }
}
