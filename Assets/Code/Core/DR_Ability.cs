using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Abilities are meant to be used like an attack or consumable. For passive effects and action reactions use DR_Modifier
// UI buttons should be given a reference to this? then when clicked will tell the action system that it wants to use this ability.
// will need to create ability action maybe (potentially a wrapper for another action?)
// when AI use abilities
public abstract class DR_Ability {

    int cooldown = 1;
    int cooldownCounter = 0;

    //todo: figure out how to handle this. decrease using params from TurnComponent probably
    public virtual void AdvanceCooldown(int amount = 1){
        cooldownCounter = Mathf.Clamp(cooldownCounter - amount, 0, cooldown);
    }

    public virtual bool CanUse(DR_GameManager gm, DR_Entity user){
        return cooldownCounter == 0;
    }

    public virtual bool UseAbility(DR_GameManager gm, DR_Entity user){
        cooldownCounter = cooldown;
        Debug.LogError("UseAbility not implemented!");
        return false;
    }
}