using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DR_Ability {

    public virtual bool CanUse(DR_GameManager gm, DR_Entity user){
        return false;
    }

    public virtual bool UseAbility(DR_GameManager gm, DR_Entity user){
        Debug.LogError("UseAbility not implemented!");
        return false;
    }
}