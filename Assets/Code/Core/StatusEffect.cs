using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: very similar to abilities, create a shared base class
public abstract class StatusEffect : DR_EffectBase
{
    public float tickRate = 4.0f;
    public float counter = 0.0f;

    // Possible types of statuses:
    // on added/removed (does not tick)
    // on tickRate
    // on some event (moved, attacked, attack, opened door, ability used, etc)


    public void Tick(float amount){
        counter += amount;
        if (counter >= tickRate){
            counter -= tickRate;
            OnTick();
        }
    }

    public virtual void OnTick(){
        Debug.Log("Status OnTick: " + this.GetType() + " on " + owner.Name);
    }

    //TODO: figure out how to support animations for status effects. Currently they are ticked outside of any action
    //
    // Option 1: Create a StatusEffect action when it gets triggered. Doesn't require any new work with the renderer
    //
    // Option 2: Change renderer to have a general queue of things to be rendered, not requiring them all to necessarily be actions
    //    - could create a simple wrapper that could contain an action, or just a list of animations
    //    - Doesn't seem fundamentally different from the action approach, but maybe more future proof
}

public class TestStatusEffect : StatusEffect
{
    //TODO: if this were to do damage, should it create an attack transaction? should status effects have an instigator that would be used here?
    // To start, should support null instigators in the attack transaction if not already

    // First test: Bleed
    // damages flat amount (for now?) OnTick. Puts blood onto ground
    public override void OnTick()
    {
        base.OnTick();
        var animAction = new AnimAction();
        animAction.relatedEntities.Add(owner);
        animAction.animations.Add(new AbilityAnimation(owner));
        GameRenderer.instance.AddAction(animAction);
    }
}