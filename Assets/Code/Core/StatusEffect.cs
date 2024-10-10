using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: very similar to abilities, create a shared base class
public abstract class StatusEffect : DR_EffectBase
{
    public float tickRate = 4.0f;
    public float counter = 0.0f;
    protected int timesTicked = 0;
    protected int tickDuration = -1;

    // Possible types of statuses:
    // on added/removed (does not tick)
    // on tickRate
    // on some event (moved, attacked, attack, opened door, ability used, etc)


    public void Tick(float amount, out bool removed){
        removed = false;
        counter += amount;
        if (counter >= tickRate){
            counter -= tickRate;
            OnTick();
            timesTicked++;
        }
        if (tickDuration >= 0 && timesTicked >= tickDuration){
            owner.GetComponent<HealthComponent>().RemoveStatusEffect(this);
            removed = true;
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

public class BleedStatusEffect : StatusEffect
{
    public BleedStatusEffect(){
        tickDuration = 3;
    }

    public override void OnTick()
    {
        base.OnTick();

        DamageSystem.CreateAttackTransaction(new(){owner}, 1);

        //TODO: try out dropping 1:1 health lost to blood on ground instead of doing it manually
        DR_GameManager.instance.CurrentMap.GetCell(owner.Position).AddBlood(1);

        var animAction = new AnimAction();
        animAction.relatedEntities.Add(owner);
        var anim = new AbilityAnimation(owner);
        ColorUtility.TryParseHtmlString("#b8253f", out anim.color); //TODO: define colors used in palette somewhere
        animAction.animations.Add(anim);
        GameRenderer.instance.AddAction(animAction);
    }
}