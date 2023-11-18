using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class DR_Component
{
    #if UNITY_EDITOR
    // Used for ContentEditor to display component names as element values
    [HideInInspector]
    public string key = "";
    #endif

    public DR_Component(){
        #if UNITY_EDITOR
        key = GetType().Name;
        #endif
    }

    public DR_Entity Entity;

    public virtual bool Trigger(DR_GameManager gm, DR_Entity user, DR_Entity target){
        return false;
    }

    public virtual void OnComponentRemoved(){
        
    }
}
