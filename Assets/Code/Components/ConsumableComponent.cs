using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConsumableComponent : DR_Component {

    public virtual bool Consume(DR_GameManager gm, DR_Entity user, DR_Entity target){
        Debug.LogError("Consume not implemented!");
        return false;
    }

    public DR_Action GetAction(DR_Entity user, DR_Entity target){
        DR_Item OwningItem = (DR_Item)Entity;
        if (OwningItem == null){
            Debug.LogException(new System.Exception("ConsumableComponent on non-item entity"));
        }
        return new ItemAction(OwningItem, user, target);
    }
}

public class HealingConsumableComponent : ConsumableComponent {

    public override bool Consume(DR_GameManager gm, DR_Entity user, DR_Entity target)
    {
        return true;
    }
}