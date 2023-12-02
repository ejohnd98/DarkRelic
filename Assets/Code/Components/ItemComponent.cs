using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemComponent : DR_Component
{
    [Copy]
    public bool requireFurtherInputOnUse = false;
    public bool UseItem(DR_GameManager gm, DR_Entity user, DR_Entity target){

        bool itemUsed = false;
        for (int i = 0; i < Entity.ComponentList.Count; i++){
            itemUsed |= Entity.ComponentList[i].Trigger(gm, user, target);
        }
        return itemUsed;
    }
}
