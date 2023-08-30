using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DR_Component
{
    public DR_Entity Entity;

    public virtual bool Trigger(DR_GameManager gm, DR_Entity user, DR_Entity target){
        return false;
    }

    public virtual void OnComponentRemoved(){
        
    }
}
