using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DR_Cell
{
    public bool bBlocksMovement = false;
    public DR_Entity Actor;
    public DR_Entity Prop;
    public DR_Entity Item;
    public int blood = 0;
    public bool bloodStained = false;

    public bool BlocksMovement(bool ignoreActor = false){
        if(!ignoreActor && Actor != null){
            return true;
        }

        if(Prop != null && Prop.GetComponent<PropComponent>().blocksMovement){
            return true;
        }

        return bBlocksMovement;
    }

    public bool BlocksSight(){
        if(Prop != null && Prop.GetComponent<PropComponent>().blocksSight){
            return true;
        }

        return bBlocksMovement;
    }
}
