using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapHelpers
{
    public static DR_Entity GetClosestEntity(DR_GameManager gm, DR_Entity sourceEntity, int range = 10){
        DR_Map map = gm.CurrentMap;

        DR_Entity chosenTarget = null;

        //Picks closest entity if target is null
        AlignmentComponent userAlignment = sourceEntity.GetComponent<AlignmentComponent>();
        if (userAlignment == null){
            Debug.LogError("MapHelpers.GetClosestEntity: sourceEntity (" + sourceEntity.Name + ") alignment component is NULL!");
            return null;
        }

        int closestDist = -1;
        foreach (DR_Entity entity in gm.CurrentMap.Entities){
            AlignmentComponent alignment = entity.GetComponent<AlignmentComponent>();
            if (alignment != null && !alignment.IsFriendly(userAlignment)){
                
                int dist = entity.DistanceTo(sourceEntity.Position);
                if (dist > range){
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

        return chosenTarget;
    }
}
