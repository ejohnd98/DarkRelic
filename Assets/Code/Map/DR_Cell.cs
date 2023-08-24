using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DR_Cell
{
    public bool bBlocksMovement = false;
    public DR_Entity Actor;
    public DR_Entity Prop;
    public DR_Entity Item;

    public bool BlocksMovement(){
        if(bBlocksMovement || Actor != null){
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
