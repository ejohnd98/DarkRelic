using System.Collections;
using System.Collections.Generic;

public class DR_Item : DR_Entity
{
    public bool UseItem(DR_GameManager gm, DR_Entity user, DR_Entity target){

        bool itemUsed = false;
        for (int i = 0; i < ComponentList.Count; i++){
            itemUsed |= ComponentList[i].Trigger(gm, user, target);
        }
        return itemUsed;
    }
}
