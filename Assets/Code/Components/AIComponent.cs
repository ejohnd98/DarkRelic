using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIComponent : DR_Component
{
    public DR_Entity target;

    public PathResult currentPath;

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

    public bool HasPath(){
        return currentPath != null && currentPath.validPath && currentPath.HasNextStep();
    }
}
