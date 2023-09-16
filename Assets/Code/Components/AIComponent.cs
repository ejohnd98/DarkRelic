using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIComponent : DR_Component
{
    public DR_Entity target; 

    public bool HasTarget(){
        if (target == null){
            return false;
        }
        if (Entity.noLongerValid){
            return false;
        }
        if (!target.GetComponent<HealthComponent>().IsAlive()){
            return false;
        }
        
        return true;
    }
}
