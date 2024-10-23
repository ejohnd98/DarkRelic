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

    //map gen debugging
    public MapGenRoom associatedRoom = null;

    public bool neverRender = false;

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

    public void AddBlood(int blood){
        this.blood += blood;
        bloodStained = true;
    }

    public void ClearBlood(){
        blood = 0;
        bloodStained = false;
    }

    public void SetBlood(int finalAmount){
        blood = finalAmount;
        bloodStained = true;
    }

    public void CollectBlood(DR_Entity collector){
        if (blood > 0){
            if (collector.GetComponent<InventoryComponent>() is InventoryComponent inventory && inventory.canCollectBlood){
                //TODO: later have a handler for this as relics will affect stuff here when getting blood

                inventory.AddBlood(blood);
                UISystem.instance.RefreshInventoryUI();
                blood = 0;
            }
        }
    }
}
