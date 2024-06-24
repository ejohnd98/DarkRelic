using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DR_Ability
{
    public bool triggeredByPlayer = true;
    public string abilityName = "";
    public Sprite sprite;
    public List<ActionInput> actionInputs;

    public virtual bool CanBePerformed(){
        return true;
    }

    public virtual void OnTrigger(DR_Event e){
        Debug.LogFormat("OnTrigger: {0} ({1}) from event type: {2}", abilityName, this.GetType().Name, e.GetType().Name);
    }

    public void ResetInputs(){
        actionInputs = new();
        SetupInputs();
    }

    protected virtual void SetupInputs(){

    }

    //TODO: need to think about how this can be driven by scriptable objects as that's where the sprite, name, and description would be
    // Don't necessarily want to make some elaborate generic system though, but can maybe just specify those things, and then see
    // if a dropdown can be created with the ability child class
    // Then when creating the ability at runtime it will just assign the data from the scriptable object to the ability instance

    // Where do entity specific events live?
    // They shouldn't necessarily live on DR_Entity
    // Perhaps they are added to each individual component?
    // What about stuff not tied to components, such as door opening?
    
    // Possibly it does live on the DR_Entity, and then the components trigger them
    // Then things can subscribe to them and they can be triggered without worry.

    // CURRENT TODO:
    // Move ability to scriptable object (initially can be default type?)
    // Create an ability action for those which are player triggered. Can be pretty barebones, but will have the "waiting for input" stage if needed

    // Future:
    // Have relic grant ability
    // Add events for basic stuff on Entity and some way for abilities to subscribe (or just hardcode that)
}

public class TestAbility : DR_Ability
{
    public TestAbility(){
    }
}

public class TestAbility2 : DR_Ability
{
    public TestAbility2(){
    }

    public override void OnTrigger(DR_Event e){
        base.OnTrigger(e);

        Debug.Log("Extra override ability code!");
    }
}

// TODO: generalize to projectile (or further to targeted?) ability
public class BloodBoltAbility : DR_Ability
{
    public bool killed = false;
    public DR_Entity target;
    public BloodBoltAbility(){
    }

    public override bool CanBePerformed(){
        //TODO: blood cost
        return true;
    }

    protected override void SetupInputs(){
        //TODO: cap range? Eventually will want to precompute targets so they can shown through UI
        actionInputs.Add(new ActionInput((Vector2Int pos) => {return DR_GameManager.instance.CurrentMap.GetActorAtPosition(pos) != null;}));
    }

    public override void OnTrigger(DR_Event e){
        Debug.Log("BloodBoltAbility Triggered with input: " + actionInputs[0].inputValue);

        DR_Entity owner = e.owner;
        target = DR_GameManager.instance.CurrentMap.GetActorAtPosition(actionInputs[0].inputValue);

        int baseDamage = owner.GetComponent<LevelComponent>().stats.strength;
        var damageEvent = DamageSystem.HandleAttack(DR_GameManager.instance, owner, target, baseDamage);

        killed = damageEvent.killed;
    }
}