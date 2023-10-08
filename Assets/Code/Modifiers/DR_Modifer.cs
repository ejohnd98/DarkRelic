using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: refactor to DR_AttackModifier
// Then can have DR_StatModifiers which are added to LevelComponent (implement a GetStats function which returns the resulting stats)
public abstract class DR_Modifier
{
    //called upon attacking a target
    public virtual void OnAttack(DR_GameManager gm, DamageEvent damageEvent){
    }

    //called upon being hit
    public virtual void OnHit(DR_GameManager gm, DamageEvent damageEvent){
    }

    //called upon killing a target
    public virtual void OnKill(DR_GameManager gm, DamageEvent damageEvent){
    }

    //called upon being killed
    public virtual void OnKilled(DR_GameManager gm, DamageEvent damageEvent){
    }

    public virtual void ApplyAttackerDamageChanges(DR_GameManager gm, DamageEvent damageEvent){
    }

    public virtual void ApplyDefenderDamageChanges(DR_GameManager gm, DamageEvent damageEvent){
    }

    public virtual string GetDescription(){
        return "[modifier with no description!]";
    }
}

public class AttackMultiplierModifier : DR_Modifier
{
    float multiplier = 1.0f;

    public AttackMultiplierModifier(float multiplier){
        this.multiplier = multiplier;
    }

    public override void ApplyAttackerDamageChanges(DR_GameManager gm, DamageEvent damageEvent)
    {
        damageEvent.multiplier *= multiplier;
    }

    public override string GetDescription()
    {
        return "multiply attack damage by " + multiplier.ToString("0.0"); //TODO: want ability to color this number
    }
}
